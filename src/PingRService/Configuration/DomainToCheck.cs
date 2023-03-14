// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainToCheck.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The class configure the ping domains.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PingRService.Configuration;

/// <summary>
/// The class configure the ping domains.
/// </summary>
public sealed class DomainToCheck
{
    /// <summary>
    /// Gets or sets the domain to check.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    public string LogMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the instance key.
    /// </summary>
    public string InstanceKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the certificate will be checked for expiry or not.
    /// </summary>
    public bool CheckCertificateExpiry { get; set; }

    /// <summary>
    /// Gets or sets the certificate expiry check interval.
    /// </summary>
    public TimeSpan CertificateExpiryCheckInterval { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the last certificate expiry check timestamp.
    /// </summary>
    public DateTimeOffset LastCertificateExpiryCheckTimestamp { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Gets a value indicating whether the configuration is valid or not.
    /// </summary>
    /// <returns>A <see cref="bool"/> value indicating whether the configuration is valid or not.</returns>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(this.Domain))
        {
            throw new ConfigurationException("The domain is empty.");
        }

        if (string.IsNullOrWhiteSpace(this.LogMessage))
        {
            throw new ConfigurationException("The log message is empty.");
        }

        if (string.IsNullOrWhiteSpace(this.InstanceKey))
        {
            throw new ConfigurationException("The instance key is empty.");
        }

        if (this.CheckCertificateExpiry && this.CertificateExpiryCheckInterval == TimeSpan.Zero)
        {
            throw new ConfigurationException("The certificate expiry check interval is not set.");
        }

        return true;
    }
}
