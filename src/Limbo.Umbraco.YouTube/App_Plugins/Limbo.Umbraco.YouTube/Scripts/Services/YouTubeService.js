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
        getThumbnail: getThumbnail,
        getDuration: function(video) {

            const p = "(([0-9]+)(H|M|S|D)|)";

            const m = video.contentDetails.duration.match(`^P${p}${p}${p}T${p}${p}${p}$`);
            if (!m) return "";

            let seconds = 0;
            let minutes = 0;
            let hours = 0;

            for (let i = 1; i < m.length; i += 3) {
                if (m[i]) {
                    switch (m[i + 2]) {
                        case "D":
                            hours += parseInt(m[i + 1]) * 24;
                            break;
                        case "H":
                            hours += parseInt(m[i + 1]);
                            break;
                        case "M":
                            minutes += parseInt(m[i + 1]);
                            break;
                        case "S":
                            seconds += parseInt(m[i + 1]);
                            break;
                    }
                }
            }

            const duration = [];

            if (hours === 1) {
                duration.push({ value: 1, text: "time", suffix: "t" });
            } else if (hours > 1) {
                duration.push({ value: hours, text: "timer", suffix: "t" });
            }

            if (minutes === 1) {
                duration.push({ value: 1, text: "minut", suffix: "m" });
            } else if (minutes > 1) {
                duration.push({ value: minutes, text: "minutter", suffix: "m" });
            }

            if (seconds === 1) {
                duration.push({ value: 1, text: "sekund", suffix: "s" });
            } else if (seconds > 1) {
                duration.push({ value: Math.floor(seconds), text: "sekunder", suffix: "s" });
            }

            for (let i = 0; i < duration.length - 1; i++) {
                if (i === duration.length - 2) {
                    duration[i].text += " og ";
                    duration[i].suffix += " og ";
                } else {
                    duration[i].text += ",";
                    duration[i].suffix += ",";
                }
            }

            return duration;

        }
    }
});