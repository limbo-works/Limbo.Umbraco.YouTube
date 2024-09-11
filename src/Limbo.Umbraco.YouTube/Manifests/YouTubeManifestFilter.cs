using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.YouTube.Manifests;

/// <inheritdoc />
public class YouTubeManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = YouTubePackage.Alias,
            PackageName = YouTubePackage.Name,
            Version = YouTubePackage.InformationalVersion,
            BundleOptions = BundleOptions.Independent,
            Scripts = [
                $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Controllers/CacheLevel.js",
                $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Services/YouTubeService.js",
                $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Controllers/ButtonList.js",
                $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Controllers/Video.js"
            ],
            Stylesheets = [
                $"/App_Plugins/{YouTubePackage.Alias}/Styles/Default.css"
            ]
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}