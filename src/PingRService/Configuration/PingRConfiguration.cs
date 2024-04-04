// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingRConfiguration.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class containing the PingR configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PingRService.Configuration;

/// <summary>
/// A class containing the PingR configuration.
/// </summary>
public sealed class PingRConfiguration
{
    /// <summary>
    /// The constant HTTP url.
    /// </summary>
    internal const string HttpUrlConstant = "http://0.0.0.0:5000";

    /// <summary>
    /// Gets or sets the HTTP url.
    /// </summary>
    public string HttpUrl { get; set; } = HttpUrlConstant;

    /// <summary>
    /// Gets or sets the domains to check.
    /// </summary>
    public List<DomainToCheck> DomainsToCheck { get; set; } = [];

    /// <summary>
    /// Gets or sets the service delay in milliseconds.
    /// </summary>
    public int ServiceDelayInMilliSeconds { get; set; } = 3000;

    /// <summary>
    /// Gets or sets the heartbeat interval in milliseconds.
    /// </summary>
    public int HeartbeatIntervalInMilliSeconds { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the Telegram bot token.
    /// </summary>
    public string TelegramBotToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Telegram chat identifier.
    /// </summary>
    public string TelegramChatId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the HTTP endpoint.
    /// </summary>
    [JsonIgnore]
    public IPEndPoint HttpEndPoint => GetEndpoint(this.HttpUrl, "Http");

    /// <summary>
    /// Gets a value indicating whether the configuration is valid or not.
    /// </summary>
    /// <returns>A <see cref="bool"/> value indicating whether the configuration is valid or not.</returns>
    public bool IsValid()
    {
        if (this.DomainsToCheck.IsEmptyOrNull())
        {
            throw new ConfigurationException("The domains to check are empty.");
        }

        if (this.DomainsToCheck.Any(d => !d.IsValid()))
        {
            throw new ConfigurationException("At least one domain is invalid.");
        }

        if (this.ServiceDelayInMilliSeconds <= 0)
        {
            throw new ConfigurationException("The service delay is invalid.");
        }

        if (this.HeartbeatIntervalInMilliSeconds <= 0)
        {
            throw new ConfigurationException("The heartbeat interval is invalid.");
        }

        if (string.IsNullOrWhiteSpace(this.TelegramBotToken))
        {
            throw new ConfigurationException("The Telegram bot token is not set.");
        }

        if (string.IsNullOrWhiteSpace(this.TelegramChatId))
        {
            throw new ConfigurationException("The Telegram chat identifier is not set.");
        }

        return true;
    }

    /// <summary>
    /// Converts the given URL <see cref="string"/> to a <see cref="IPEndPoint"/>.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <param name="endpointName">The endpoint.</param>
    /// <returns>A new <see cref="IPEndPoint"/>.</returns>
    private static IPEndPoint GetEndpoint(string url, string endpointName)
    {
        var httpUrlParts = url.Replace("//", string.Empty).Split(":");

        if (httpUrlParts.Length != 3)
        {
            throw new ConfigurationException($"Url for {endpointName} endpoint is invalid.");
        }

        if (!IPAddress.TryParse(httpUrlParts[1], out var ipAddress))
        {
            throw new ConfigurationException($"Url for {endpointName} endpoint is invalid.");
        }

        if (int.TryParse(httpUrlParts[2], out var port))
        {
            return new IPEndPoint(ipAddress, port);
        }

        throw new ConfigurationException($"Url for {endpointName} endpoint is invalid.");
    }
}
