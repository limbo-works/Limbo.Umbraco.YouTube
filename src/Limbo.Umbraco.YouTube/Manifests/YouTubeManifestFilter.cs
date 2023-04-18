using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.YouTube.Manifests {

    /// <inheritdoc />
    public class YouTubeManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                AllowPackageTelemetry = true,
                PackageName = YouTubePackage.Name,
                Version = YouTubePackage.InformationalVersion,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Services/YouTubeService.js",
                    $"/App_Plugins/{YouTubePackage.Alias}/Scripts/Controllers/Video.js"
                },
                Stylesheets = new [] {
                    $"/App_Plugins/{YouTubePackage.Alias}/Styles/Default.css"
                }
            });
        }

    }

}