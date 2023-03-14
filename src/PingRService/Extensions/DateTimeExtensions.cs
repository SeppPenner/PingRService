// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class that contains extension methods for the DateTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PingRService.Extensions;

/// <summary>
/// A class that contains extension methods for the DateTime type.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Checks the <see cref="DateTime"/> for expiration.
    /// </summary>
    /// <param name="timestamp">The timestamp to check.</param>
    /// <param name="duration">The timespan to check.</param>
    /// <returns>True when expired otherwise false.</returns>
    public static bool IsExpired(this DateTime timestamp, TimeSpan duration)
    {
        return timestamp.Add(duration) < DateTime.Now;
    }

    /// <summary>
    /// Checks the <see cref="DateTime"/> for expiration.
    /// </summary>
    /// <param name="timestamp">The timestamp to check.</param>
    /// <returns>True when expired otherwise false.</returns>
    public static bool IsExpired(this DateTime timestamp)
    {
        return timestamp.IsExpired(TimeSpan.Zero);
    }
}
