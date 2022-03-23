angular.module("umbraco").controller("Limbo.Umbraco.YouTube.Video", function ($scope, $element, $timeout, youTubeService) {

    const vm = this;

    let rawVideoData = null;

    // Gets information about the video of the entered URL
    vm.getVideo = function () {

        const source = $scope.model.value && $scope.model.value.source ? $scope.model.value.source.trim() : null;

        if (source) {

            vm.loading = true;
            vm.updateUI();

            youTubeService.getVideo(source).then(function (res) {

                // Update the credentials and embed details
                $scope.model.value.credentials = res.data.credentials;
                $scope.model.value.embed = res.data.embed;

                // As Umbraco/JSON.net will corrupt any timestamps in the JSON, we need to store it as serialized
                $scope.model.value.video = { _data: angular.toJson(res.data.video) };
                rawVideoData = res.data.video;

                vm.loading = false;
                vm.updateUI();

            });

        } else {

            vm.embed = false;
            rawVideoData = null;

            delete $scope.model.value.credentials;
            delete $scope.model.value.video;
            delete $scope.model.value.embed;

            vm.updateUI();

        }

    };

    // Triggered by the UI when the user changes the URL
    vm.updated = function () {
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
        vm.duration = youTubeService.getDuration(rawVideoData);
        vm.thumbnail = youTubeService.getThumbnail(rawVideoData);

    };

    function init() {

        if (!$scope.model.value) {
            $scope.model.value = null;
            return;
        }

        if (!$scope.model.value.video) return;
        if (!$scope.model.value.video._data) return;

        // Get the Vimeo video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
        rawVideoData = angular.fromJson($scope.model.value.video._data);

        vm.updateUI();

    }

    init();

});