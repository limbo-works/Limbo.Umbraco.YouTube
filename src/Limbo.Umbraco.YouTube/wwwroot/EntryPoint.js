// Registers everything this package contributes to the backoffice. In Umbraco 13 this was declared server-side
// by an IManifestFilter; in Umbraco 17 the manifest points at this single "backofficeEntryPoint" module and the
// extensions are registered here at runtime instead.
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { YouTubeAuth } from "@limbo/youtube/auth";
import { YouTubePackage } from "@limbo/youtube/package";
import { YouTubeService } from "@limbo/youtube/service";

const ALIAS = "Limbo.Umbraco.YouTube";

const SCHEMA_ALIAS = `${ALIAS}.Video`;
const VIDEO_UI_ALIAS = `${ALIAS}.Video.Ui`;
const BUTTON_LIST_ALIAS = `${ALIAS}.ButtonList.PropertyEditorUi`;

// Tri-state used by the embed options: "not specified" falls back to whatever the video URL asked for
const NOT_SPECIFIED_YES_NO = [
    { value: null, label: "Not specified" },
    { value: true, label: "Yes" },
    { value: false, label: "No" }
];

// Mirrors Umbraco.Cms.Core.PropertyEditors.PropertyCacheLevel (minus "Unknown")
const CACHE_LEVELS = [
    { value: "Element", label: "Element", title: "Cache for the lifetime of the element" },
    { value: "Elements", label: "Elements", title: "Cache for the lifetime of all elements" },
    { value: "Snapshot", label: "Snapshot", title: "Cache for the lifetime of the snapshot" },
    { value: "None", label: "None", title: "Do not cache" }
];

function buttonList(alias, label, description, items) {
    return {
        alias,
        label,
        description,
        propertyEditorUiAlias: BUTTON_LIST_ALIAS,
        config: [{ alias: "items", value: items }]
    };
}

function onPackageLoaded(extensionRegistry) {

    const cacheBuster = YouTubePackage.cacheBuster;

    extensionRegistry.register({
        type: "localization",
        alias: `${ALIAS}.EnUs`,
        name: "English (US)",
        js: () => import(`./Localization/en-US.js?v=${cacheBuster}`),
        meta: { culture: "en" }
    });

    extensionRegistry.register({
        type: "localization",
        alias: `${ALIAS}.DaDk`,
        name: "Danish",
        js: () => import(`./Localization/da-DK.js?v=${cacheBuster}`),
        meta: { culture: "da" }
    });

    extensionRegistry.register({
        type: "icons",
        alias: `${ALIAS}.Icons`,
        name: "Limbo YouTube Icons",
        js: `/App_Plugins/${ALIAS}/Icons.js?v=${cacheBuster}`
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: BUTTON_LIST_ALIAS,
        name: "Limbo YouTube Button List",
        js: () => import(`./Elements/ButtonList.js?v=${cacheBuster}`),
        elementName: "limbo-youtube-button-list",
        meta: {
            label: "Limbo YouTube Button List",
            icon: "icon-list",
            group: "common"
        }
    });

    // The schema alias must match YouTubeVideoPropertyEditor.EditorAlias on the server, so that existing data
    // types (and the property values they have already saved) keep working.
    extensionRegistry.register({
        type: "propertyEditorSchema",
        alias: SCHEMA_ALIAS,
        name: "Limbo YouTube Video",
        meta: {
            defaultPropertyEditorUiAlias: VIDEO_UI_ALIAS,
            settings: {
                properties: [
                    buttonList(
                        "cacheLevel",
                        "Cache level",
                        "Select the cache level of the underlying property value converter.",
                        CACHE_LEVELS
                    ),
                    buttonList(
                        "autoplay",
                        "Autoplay",
                        "Select whether videos should autoplay when embedded.",
                        NOT_SPECIFIED_YES_NO
                    ),
                    buttonList(
                        "loop",
                        "Loop",
                        "Select whether videos should loop.",
                        NOT_SPECIFIED_YES_NO
                    ),
                    buttonList(
                        "controls",
                        "Show controls",
                        "Select whether the video player controls are displayed.",
                        NOT_SPECIFIED_YES_NO
                    ),
                    buttonList(
                        "rel",
                        "Show related",
                        "Select whether the player should show related videos when the video ends.",
                        NOT_SPECIFIED_YES_NO
                    ),
                    buttonList(
                        "disableCookies",
                        "Cookieless",
                        "When you turn on privacy-enhanced mode, YouTube won't store information about visitors on your website unless they play the video.",
                        NOT_SPECIFIED_YES_NO
                    )
                ],
                defaultData: [
                    { alias: "cacheLevel", value: "Element" }
                ]
            }
        }
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: VIDEO_UI_ALIAS,
        name: "Limbo YouTube Video Property Editor UI",
        js: () => import(`./Elements/Video.js?v=${cacheBuster}`),
        elementName: "limbo-youtube-video",
        meta: {
            label: "Limbo YouTube Video",
            propertyEditorSchemaAlias: SCHEMA_ALIAS,
            icon: "limbo-youtube-alt",
            group: "Limbo",
            supportsReadOnly: true
        }
    });

}

export const onInit = (host, extensionRegistry) => {

    // consumeContext may call back more than once (and with no context at all), so make sure the extensions are
    // only ever registered once
    let initialized = false;

    host.consumeContext(UMB_AUTH_CONTEXT, async (authContext) => {

        if (!authContext || initialized) return;
        initialized = true;

        // Service.js needs a token getter to authorize its calls against the Management API
        YouTubeAuth.TOKEN = authContext.getOpenApiConfiguration().token;

        try {
            YouTubePackage.serverVariables = await YouTubeService.getServerVariables();
        } catch {
            // Fall back to an empty set of server variables, so the extensions are still registered (the cache
            // buster then simply resolves to "undefined")
            YouTubePackage.serverVariables = {};
        }

        onPackageLoaded(extensionRegistry);

    });

};
