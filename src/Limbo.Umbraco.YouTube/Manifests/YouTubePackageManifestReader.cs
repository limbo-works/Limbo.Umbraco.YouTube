// [CHANGE: Umbraco 17 upgrade - replaces the removed IManifestFilter. The new backoffice loads a single
// "backofficeEntryPoint" module, and the importmap gives our own modules stable bare specifiers.]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.YouTube.Manifests;

public class YouTubePackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = YouTubePackage.Alias;
        string cacheBuster = YouTubePackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> temp = [
            new () {
                Id = alias,
                Name = YouTubePackage.Name,
                AllowTelemetry = true,
                Version = YouTubePackage.InformationalVersion,
                Extensions = [
                    new {
                        name = $"{alias}.EntryPoint",
                        alias = $"{alias}.EntryPoint",
                        type = "backofficeEntryPoint",
                        js = $"/App_Plugins/{alias}/EntryPoint.js?v={cacheBuster}"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        { "@limbo/youtube/auth", $"/App_Plugins/{alias}/Auth.js?v={cacheBuster}" },
                        { "@limbo/youtube/package", $"/App_Plugins/{alias}/Package.js?v={cacheBuster}" },
                        { "@limbo/youtube/service", $"/App_Plugins/{alias}/Service.js?v={cacheBuster}" }
                    }
                }
            }
        ];

        return await Task.FromResult(temp);

    }

}
