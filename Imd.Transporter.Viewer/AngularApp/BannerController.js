(function () {

    var banner = angular.module("bannerController", []);

    var bannerController = function (transporterServiceFactory) {

        var ban = this;
        var onComplete = function (response) {
            ban.buildNumber = response;
        };

        var onError = function () {
            ban.buildNumber = "unknown";
        };

        transporterServiceFactory.getBuildNumber().then(onComplete, onError);
    };
    banner.controller("bannerController", bannerController);
}());