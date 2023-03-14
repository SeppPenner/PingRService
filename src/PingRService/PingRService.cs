// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingRService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The PingR service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PingRService;

/// <seealso cref="BackgroundService"/>
/// <inheritdoc cref="BackgroundService"/>
/// <summary>
/// The PingR service.
/// </summary>
internal sealed class PingRService : BackgroundService
{
    /// <summary>
    /// The HTTP client.
    /// </summary>
    private readonly HttpClient httpClient = new();

    /// <summary>
    /// The stopwatch for the application lifetime.
    /// </summary>
    private readonly Stopwatch uptimeStopWatch = Stopwatch.StartNew();

    /// <summary>
    /// Gets or sets the last heartbeat timestamp.
    /// </summary>
    private DateTimeOffset LastHeartbeatAt { get; set; }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    private ILogger Logger { get; set; } = Log.Logger;

    /// <summary>
    /// Gets or sets the service configuration.
    /// </summary>
    private PingRConfiguration ServiceConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingRService"/> class.
    /// </summary>
    /// <param name="configuration">The PingR configuration.</param>
    public PingRService(PingRConfiguration configuration)
    {
        // Load the configuration.
        this.ServiceConfiguration = configuration;

        // Create the logger.
        this.Logger = LoggerConfig.GetLoggerConfiguration(nameof(PingRService))
            .WriteTo.Sink((ILogEventSink)Log.Logger)
            .CreateLogger();

        // Adds the handler and the HTTP client.
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = this.ValidateServerCertificate
        };

        this.httpClient = new(handler);
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        this.Logger.Information("Starting PingR service");
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        this.Logger.Information("Stopping PingR service");
        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        this.Logger.Information("Executing PingR service");

        while (!cancellationToken.IsCancellationRequested)
        {
            // Runs the main task of the service.
            await this.TryRunServiceTask();

            // Run the heartbeat and log some memory information.
            this.LogMemoryInformation(this.ServiceConfiguration.HeartbeatIntervalInMilliSeconds, Program.ServiceName);
            await Task.Delay(this.ServiceConfiguration.ServiceDelayInMilliSeconds, cancellationToken);
        }
    }

    /// <summary>
    /// Runs the main task of the service.
    /// </summary>
    private async Task TryRunServiceTask()
    {
        try
        {
            // Start a new stop watch.
            var stopwatch = Stopwatch.StartNew();
            this.Logger.Information("Started cyclic PingR task");

            foreach (var domain in this.ServiceConfiguration.DomainsToCheck)
            {
                await this.PingDomain(domain);
            }

            // All tasks are finished, cyclic PingR task is done.
            this.Logger.Information("Finished cyclic PingR task after: {Duration}", stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Service task failed");
        }
    }

    /// <summary>
    /// Pings the domain and sends an error if needed.
    /// </summary>
    /// <param name="domainToCheck">The domain to check.</param>
    private async Task PingDomain(DomainToCheck domainToCheck)
    {
        try
        {
            this.Logger.Information("Checking domain {Domain} for instance key {InstanceKey}", domainToCheck.Domain, domainToCheck.InstanceKey);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(domainToCheck.Domain),
                Headers =
                {
                    { HeaderKeys.DomainToken, domainToCheck.InstanceKey }
                }
            };

            var response = await this.httpClient.SendAsync(httpRequestMessage);

            if (response?.IsSuccessStatusCode is false)
            {
                using var instanceKeyContext = LogContext.PushProperty(LoggingKeys.InstanceKey, domainToCheck.InstanceKey);
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
                this.Logger.Error(domainToCheck.LogMessage);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
            }
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Domain query failed");
        }
    }

    /// <summary>
    /// Validates the server certificate.
    /// </summary>
    /// <param name="requestMessage">The HTTP request message.</param>
    /// <param name="certificate">The certificate.</param>
    /// <param name="certificateChain">The certificate chain.</param>
    /// <param name="sslErrors">The SSL errors.</param>
    /// <returns>A value indicating whether the certificate is valid or not.</returns>
    private bool ValidateServerCertificate(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? certificateChain, SslPolicyErrors sslErrors)
    {
        // Skip if no certificate is set.
        if (certificate is null)
        {
            return true;
        }

        // Check the headers.
        var headerValues = requestMessage.Headers.GetValues(HeaderKeys.DomainToken);

        if (headerValues.IsEmptyOrNull())
        {
            return sslErrors == SslPolicyErrors.None;
        }

        // Check the first header.
        var firstHeader = headerValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(firstHeader))
        {
            return sslErrors == SslPolicyErrors.None;
        }

        // Check the header value for the domain to check.
        var foundDomain = this.ServiceConfiguration.DomainsToCheck.FirstOrDefault(d => d.InstanceKey == firstHeader);

        if (foundDomain is null)
        {
            return sslErrors == SslPolicyErrors.None;
        }

        if (foundDomain.CheckCertificateExpiry)
        {
            // If the last check was done before the check interval is expired, do nothing.
            if (!foundDomain.LastCertificateExpiryCheckTimestamp.IsExpired(foundDomain.CertificateExpiryCheckInterval))
            {
                return sslErrors == SslPolicyErrors.None;
            }

            // Check certificate for expiry.
            if (certificate.NotAfter.IsExpired())
            {
                this.Logger.Error("The certificate with issuer {Issuer} is expired at {ExpiryDate}", certificate.Issuer, certificate.NotAfter);
                foundDomain.LastCertificateExpiryCheckTimestamp = DateTimeOffset.Now;
                return sslErrors == SslPolicyErrors.None;
            }

            // If the certificate is short before expiry, warn.
            if ((certificate.NotAfter - DateTime.Now) < TimeSpan.FromDays(30))
            {
                this.Logger.Warning("The certificate with issuer {Issuer} is about to expiry: {ExpiryDate}", certificate.Issuer, certificate.NotAfter);
                foundDomain.LastCertificateExpiryCheckTimestamp = DateTimeOffset.Now;
            }
            else
            {
                // Do nothing, but set the timestamp.
                foundDomain.LastCertificateExpiryCheckTimestamp = DateTimeOffset.Now;
            }
        }

        return sslErrors == SslPolicyErrors.None;
    }

    /// <summary>
    /// Logs the memory information.
    /// </summary>
    /// <param name="heartbeatIntervalInMilliSeconds">The heartbeat interval in milliseconds.</param>
    /// <param name="serviceName">The service name.</param>
    private void LogMemoryInformation(int heartbeatIntervalInMilliSeconds, string serviceName)
    {
        // Log memory information if the heartbeat is expired.
        if (this.LastHeartbeatAt.IsExpired(TimeSpan.FromMilliseconds(heartbeatIntervalInMilliSeconds)))
        {
            // Run the heartbeat and log some memory information.
            this.LogMemoryInformation(serviceName);
            this.LastHeartbeatAt = DateTimeOffset.Now;
        }
    }

    /// <summary>
    /// Logs the memory information.
    /// </summary>
    /// <param name="serviceName">The service name.</param>
    private void LogMemoryInformation(string serviceName)
    {
        var totalMemory = GC.GetTotalMemory(false);
        var memoryInfo = GC.GetGCMemoryInfo();
        var totalMemoryFormatted = SystemGlobals.GetValueWithUnitByteSize(totalMemory);
        var heapSizeFormatted = SystemGlobals.GetValueWithUnitByteSize(memoryInfo.HeapSizeBytes);
        var memoryLoadFormatted = SystemGlobals.GetValueWithUnitByteSize(memoryInfo.MemoryLoadBytes);
        this.Logger.Information(
            "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}, uptime {Uptime}",
            serviceName, totalMemoryFormatted, heapSizeFormatted, memoryLoadFormatted, this.uptimeStopWatch.Elapsed);
    }
}
