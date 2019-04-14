(function () {

    var transporterServiceFactory =  function ($http, $q) {

            var isEmptyString = function (value) {
                return (angular.isUndefined(value) || value === null || value === "");
            };

            var buildTasksUri = function (statusText, serverName, fileName) {
                var uri = "api/tasksWithFilters";
                var filterSet = "";
                if (!isEmptyString(statusText)) {
                    filterSet += "statusText=" + statusText;
                }
                if (!isEmptyString(serverName)) {
                    if (!isEmptyString(filterSet)) filterSet += "|";
                    filterSet += "serverName=" + serverName;
                }
                if (!isEmptyString(fileName)) {
                    if (!isEmptyString(filterSet)) filterSet += "|";
                    filterSet += "fileName=" + fileName;
                }
                if (isEmptyString(filterSet)) {
                    return uri;
                } else {
                    return uri + "/" + filterSet;
                }
            };

            var getStatuses = function() {
                return $http.get("api/statuses")
                    .then(function(response) {
                        return response.data;
                    });
            };

            var getTransporters = function () {
                return $http.get("api/transporters")
                    .then(function (response) {
                        return response.data;
                    });
            };

            var getBuildNumber = function() {
                return $http.get("api/banner/buildNumber")
                    .then(function(response) {
                        return response.data;
                    });
            };

            var getTodaysStatsSummary = function () {
                return $http.get("api/stats/summary")
                    .then(function(response) {
                        return response.data;
                    });
            };

            var getTodaysStatsDetail = function (serverName) {
                return $http.get("api/stats/detail/" + serverName)
                    .then(function (response) {
                        return response.data;
                    });
            };

            // using $q.defer() allows the calls to be cancelled, e.g. when a new filter call is made whilst one is already pending.
            var getWithFilterCriteria = function (statusText, serverName, fileName) {
                var uri = buildTasksUri(statusText, serverName, fileName);
                var canceller = $q.defer();
                var cancel = function(reason) {
                    canceller.resolve(reason);
                };

                var promise = $http.get(uri, {timeout: canceller.promise})
                    .then(function(response) {
                        return response.data;
                    });
                return {
                    promise: promise,
                    cancel: cancel
                };
            }
           
            return {
                getStatuses: getStatuses,
                getTransporters: getTransporters,
                getBuildNumber: getBuildNumber,
                getWithFilterCriteria: getWithFilterCriteria,
                getTodaysStatsSummary: getTodaysStatsSummary,
                getTodaysStatsDetail: getTodaysStatsDetail
            };
        };

    var app = angular.module("transporterService", []);
    app.factory("transporterServiceFactory", transporterServiceFactory);
}());
