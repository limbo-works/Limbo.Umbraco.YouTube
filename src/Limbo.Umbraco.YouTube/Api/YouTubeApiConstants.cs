// [CHANGE: Umbraco 17 upgrade - the backoffice now talks to a versioned Management API instead of
// a PluginController] Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.YouTube.Api;

public static class YouTubeApiConstants {

    public const string Route = "limbo/youtube";

    public const string Alias = "limbo-youtube-v1";

    public const string Name = "Limbo YouTube API v1";

    public const string GroupName = "Limbo YouTube";

}
