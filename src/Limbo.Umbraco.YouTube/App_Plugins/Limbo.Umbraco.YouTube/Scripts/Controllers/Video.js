angular.module("umbraco").controller("Limbo.Umbraco.YouTube.Video", function ($scope, $element, $timeout, youTubeService) {

    const vm = this;

    if (!$scope.model.value) {
        $scope.model.value = { source: "" };
    }

    // Gets information about the video of the entered URL
    vm.getVideo = function() {

        const source = $scope.model.value.source.trim();

        if (source) {

            vm.loading = true;
            vm.updateUI();

            youTubeService.getVideo(source).then(function (res) {
                $scope.model.value.credentials = res.data.credentials;
                $scope.model.value.video = res.data.video;
                vm.loading = false;
                vm.updateUI();
            });

        } else {

            vm.embed = false;

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

        const embed = $scope.model.value.source && $scope.model.value.source.indexOf("<") >= 0;

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

        const video = $scope.model.value.video;

        if (!video) {
            vm.thumbnail = null;
            return;
        }

        vm.duration = youTubeService.getDuration(video);
        vm.thumbnail = youTubeService.getThumbnail(video);

    };

    vm.updateUI();

});