(function() {
    var transporterViewer = angular.module("transporterViewer",
        ["ngRoute", "tasksController", "transporterService", "bannerController", "ui.bootstrap", "statsController"]);

    transporterViewer.directive('banner', function () {
        return {
            controller:"bannerController",
            controllerAs: "ban",
            restrict: 'E',
            templateUrl: "/AngularApp/banner.html"
        };
    });

    transporterViewer.directive('tasksgrid', function () {
        return {
            restrict: 'E',
            templateUrl: "/AngularApp/tasksgrid.html"
        };
    });

    transporterViewer.directive('filters', function () {
        return {
            restrict: 'E',
            templateUrl: "/AngularApp/filters.html"
        };
    });

    transporterViewer.directive('footer', function () {
        return {
            restrict: 'E',
            templateUrl: "/AngularApp/footer.html"
        };
    });

    transporterViewer.directive('statistics', function () {
        return {
            controller: "statsController",
            controllerAs: "stats",
            restrict: 'E',
            templateUrl: "/AngularApp/stats.html"
        };
    });

    // When the html with 'toggle-headers' directive is clicked (banner.html), the mousedown event is fired.
    transporterViewer.directive('toggleHeaders', function () {
        return function(scope, element) {
            element.on('mousedown', function () {
                $("#spinner").removeClass("form-group gauge-loader");
                $("#spinner").removeClass("form-group throbber-loader");
                $("#bannerNav").removeClass("xmasBody");

                $("#normalBanner").slideToggle();
                $("#tackyBanner").slideToggle(function() {
                    if ($("#normalBanner").css('display') === "block") {
                        $("#spinner").addClass("form-group throbber-loader");
                    } else {
                        $("#spinner").addClass("form-group gauge-loader");
                        $("#bannerNav").addClass("xmasBody");
                    }
                });
            });
        }
    });
    transporterViewer.filter('pagination', function () {
        return function (input, start) {
            start = +start;
            if (angular.isUndefined(input)) return input;
            return input.slice(start);
        };
    });
}());