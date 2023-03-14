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
    /// Gets or sets the domains to check.
    /// </summary>
    public List<DomainToCheck> DomainsToCheck { get; set; } = new();

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
}
