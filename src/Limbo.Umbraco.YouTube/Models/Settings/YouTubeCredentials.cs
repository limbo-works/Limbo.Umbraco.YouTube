using System;
using Limbo.Umbraco.Video.Models.Credentials;

namespace Limbo.Umbraco.YouTube.Models.Settings;

/// <summary>
/// Class with information about the credentials used for accessing the YouTube API.
/// </summary>
public class YouTubeCredentials : ICredentials {

    /// <summary>
    /// Gets the key of the credentials.
    /// </summary>
    public required Guid Key { get; set; }

    /// <summary>
    /// Gets the friendly name of the credentials.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets the description of the credentials.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// If configured, gets the Google server key. Server keys allow accessing the YouTube API without a user context.
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public YouTubeCredentials() { }

}