namespace Limbo.Umbraco.YouTube.Models.Api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ServerVariables {

    public required string Version { get; init; }

    public required string CacheBuster { get; init; }

}