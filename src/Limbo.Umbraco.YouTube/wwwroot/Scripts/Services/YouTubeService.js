angular.module("umbraco.services").factory("youTubeService", function ($http) {

    // Get relevant settings from Umbraco's server variables
    const cacheBuster = Umbraco.Sys.ServerVariables.application.cacheBuster;
    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    // Fetches information about the video from our underlying API
    function getVideo(source) {

        const data = {
            source: source
        };

        return $http({
            method: "POST",
            url: `${umbracoPath}/backoffice/Limbo/YouTube/GetVideo`,
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            data: $.param(data)
        });

    }

    // Returns a thumbnail object for the video
    function getThumbnail(video) {
        if (!video) return null;

        if (video.snippet.thumbnails.medium) {
            return {
                url: video.snippet.thumbnails.medium.url,
                width: video.snippet.thumbnails.medium.width / 1.25,
                height: video.snippet.thumbnails.medium.height / 1.25
            };
        }

        if (video.snippet.thumbnails.default) {
            return {
                url: video.snippet.thumbnails.default.url,
                width: video.snippet.thumbnails.default.width,
                height: video.snippet.thumbnails.default.height
            };
        }

        return null;

    }
    
    return {
        getVideo: getVideo,
        getThumbnail: getThumbnail
    }
});