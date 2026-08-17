// [CHANGE: Umbraco 17 upgrade - adds backoffice auth requirements to this package's Swagger document]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.YouTube.Api;

public class YouTubeSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    // [CHANGE: code review fix - the base class compares ApiName against the controller's [MapToApi] value (and
    // hence the Swagger document name), not against the document title, so this must be the alias]
    // Related: wwwroot/Elements/Video.js, PropertyEditors/YouTubeVideoValueConverter.cs, documentation/UPGRADE-UMBRACO-17.md
    protected override string ApiName => YouTubeApiConstants.Alias;

}
