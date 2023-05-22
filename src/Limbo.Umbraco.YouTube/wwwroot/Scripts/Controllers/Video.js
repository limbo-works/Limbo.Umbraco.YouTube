angular.module("umbraco").controller("Limbo.Umbraco.YouTube.Video", function ($scope, $element, $timeout, youTubeService) {

    const vm = this;

    vm.value = null;

    let rawVideoData = null;

    // Gets information about the video of the entered URL
    vm.getVideo = function () {

        const source = vm.value && vm.value.source ? vm.value.source.trim() : null;

        if (source) {

            vm.loading = true;
            vm.updateUI();

            youTubeService.getVideo(source).then(function (res) {

                // Update the credentials and embed details
                vm.value.credentials = res.data.credentials;
                vm.value.embed = res.data.embed;

                // As Umbraco/JSON.net will corrupt any timestamps in the JSON, we need to store it as serialized
                vm.value.video = { _data: angular.toJson(res.data.video) };
                rawVideoData = res.data.video;

                // Update the property value
                vm.sync();

                vm.loading = false;
                vm.updateUI();

            });

        } else {

            vm.embed = false;
            rawVideoData = null;

            delete vm.value.credentials;
            delete vm.value.video;
            delete vm.value.embed;

            // Update the property value
            vm.sync();

            vm.updateUI();

        }

    };

    vm.sync = function() {

        // In order to reset the property value, we need to set the value to an empty string rather than null as
        // Umbraco otherwise will save a string with the value "null" instead of an actual null value
        $scope.model.value = vm.value?.source ? vm.value : "";

    };

    // Triggered by the UI when the user changes the URL
    vm.updated = function () {
        vm.sync();
        vm.getVideo();
    };

    vm.refresh = function() {
        vm.getVideo();
    };

    // Updates the video information for the UI
    vm.updateUI = function () {

        const embed = $scope.model.value && $scope.model.value.source && $scope.model.value.source.indexOf("<") >= 0;

        if (embed !== vm.embed) {
            vm.embed = embed;
            const el = $element[0].querySelector(embed ? "textarea" : "input");
            if (el) {
                // Add a bit delay so the element is visible before we try to focus it
                $timeout(function () {
                    el.focus();
                }, 20);
            }
        }

        if (!rawVideoData) {
            vm.videoId = null;
            vm.title = null,
            vm.duration = null;
            vm.thumbnail = null;
            return;
        }

        vm.videoId = rawVideoData.id;
        vm.title = rawVideoData.snippet?.title,
        vm.duration = rawVideoData.contentDetails?.duration;
        vm.thumbnail = youTubeService.getThumbnail(rawVideoData);

    };

    function init() {

        if (!$scope.model.config) $scope.model.config = {};
        $scope.model.hideLabel = $scope.model.config.hideLabel === true;

        if (!$scope.model.value || $scope.model.value === "null") {
            $scope.model.value = null;
            return;
        }

        if (!$scope.model.value.video) return;
        if (!$scope.model.value.video._data) return;

        // Umbraco has some an annoying behaviour when saving null values, so we need to work keep a shadow model to
        // work around this issue
        vm.value = $scope.model.value;

        // Get the Vimeo video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
        rawVideoData = angular.fromJson(vm.value.video._data);

        vm.updateUI();

    }

    init();

});