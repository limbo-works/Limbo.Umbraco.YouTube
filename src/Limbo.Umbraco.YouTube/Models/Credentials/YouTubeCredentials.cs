﻿using System;
using Limbo.Umbraco.Video.Models.Credentials;

namespace Limbo.Umbraco.YouTube.Models.Credentials;

/// <summary>
/// Class with information about the credentials used for accessing the YouTube API.
/// </summary>
public class YouTubeCredentials : ICredentials {

    /// <summary>
    /// Gets the key of the credentials.
    /// </summary>
    public Guid Key { get; internal set; }

    /// <summary>
    /// Gets the friendly name of the credentials.
    /// </summary>
    public string Name { get; internal set; } = null!;

    /// <summary>
    /// Gets the description of the credentials.
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// If configured, gets the Google server key. Server keys allow accessing the YouTube API without a user context.
    /// </summary>
    public string ApiKey { get; internal set; } = null!;

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public YouTubeCredentials() { }

}