using System.Collections.Generic;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.YouTube.Manifests {

    /// <inheritdoc />
    public class YouTubeManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                PackageName = YouTubePackage.Alias.ToKebabCase(),
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