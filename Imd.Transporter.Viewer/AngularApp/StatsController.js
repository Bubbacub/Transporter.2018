(function () {

    var statistics = angular.module("statsController", []);

    var statsController = function ($interval, transporterServiceFactory) {

        var stats = this;
        stats.buttonText = "Show";
        stats.isLoading = false;
        stats.showStats = false;
        stats.selectedServer = "";
        stats.showDetail = false;

        var refreshStats;
        var onSummaryFinished = function (showStats) {
            stats.isLoading = false;
            stats.showStats = showStats;
            stats.buttonText = stats.showStats ? "Hide" : "Show";
            stats.setButtonClass();
            
            stats.serverStats = null;
            stats.showDetail = false;
        }

        var onDetailFinished = function () {
            stats.isLoading = false;
        }

        var onSummaryComplete = function (response) {
            stats.summaryData = response;
            //stats.showDetail = true;
            onSummaryFinished(true);
        };

        var onSummaryError = function () {
            stats.summaryData = "unknown";
            stats.showDetail = false;
        };

        var onDetailComplete = function (response) {
            stats.serverStats = response;
            onDetailFinished(true);
        };

        var onDetailError = function () {
            stats.serverStats = "unknown";
        };

        var getSummaryStats = function () {
            stats.isLoading = true;
            transporterServiceFactory.getTodaysStatsSummary().then(onSummaryComplete, onSummaryError);
        }

        stats.getSummaryByServer = function (serverName) {
            stats.showDetail = true;
            stats.selectedServer = serverName;
            stats.isLoading = true;
            transporterServiceFactory.getTodaysStatsDetail(serverName).then(onDetailComplete, onDetailError);
        };

        stats.onShowStatsSummaryClick = function (showStats) {
            if (showStats) {
                getSummaryStats();
                refreshStats = $interval(getSummaryStats, 60000);
            } else {
                $interval.cancel(refreshStats);
                onSummaryFinished(showStats);
            }
        };

        stats.setButtonClass = function() {
            return stats.isLoading ? "fa fa-spin" : "";
        }
    };
    statistics.controller("statsController", statsController);
}());