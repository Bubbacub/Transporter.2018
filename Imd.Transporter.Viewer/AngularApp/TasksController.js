(function () {

    var tasks = angular.module("tasksController", []);

    var tasksController = function ($scope, $interval, $timeout, transporterServiceFactory, $log) {

        var onStatusComplete = function (response) {
            $scope.statusCodes = response;
        };

        var onStatusErrorError = function () {
            $scope.statusCodes = [
                    "Active",
                    "In Progress",
                    "Pending",
                    "Completed",
                    "Error",
                    "Cancelled"
                    ];
        };

        // Refresh the view every 15 secs, using any filters which have been selected.
        //var tasksRefresh = $interval(function () {
        //    $scope.onGetTasksWithFilters();
        //}.bind(this), 15000);
        var refreshTasks = function() {
            $scope.onGetTasksWithFilters();
        };

        var countdownInterval;

        var decrementCountdown = function () {
            $scope.remaining--;
            if ($scope.remaining < 1) {
                $interval.cancel(countdownInterval);
                refreshTasks();
            };
        };

        var startCountdown = function () {
            $interval.cancel(countdownInterval);
            $scope.remaining = 15;
            countdownInterval = $interval(decrementCountdown, 1000, $scope.remaining);
        };
       
        var onTasksComplete = function (data) {
            $scope.transporterTasks = data;
        };
        var onTasksError = function(reason) {
            if(reason.data)$scope.error = reason.data.message;
        };
        var onFinishedSearch = function () {
            $scope.showSpinner = false;
            startCountdown();
        };

        transporterServiceFactory.getStatuses().then(onStatusComplete, onStatusErrorError);

        transporterServiceFactory.getTransporters().then(function(servers) {
            $scope.transporters = servers;
        });
        
        $scope.status = "";
        $scope.serverName = "";
        $scope.fileName = "";
        $scope.showSpinner = false;
        $scope.currentHttpRequest = null;

        $scope.CancelCurrentRequest = function() {
            if ($scope.currentHttpRequest) {
                $scope.currentHttpRequest.cancel();
            };
        };

        $scope.onGetTasksWithFilters = function () {
            $scope.showSpinner = true;
            $scope.CancelCurrentRequest();
            $scope.currentHttpRequest = transporterServiceFactory.getWithFilterCriteria($scope.status, $scope.serverName, $scope.fileName);
            // promise =  the API call has not been cancelled.
            $scope.currentHttpRequest.promise.then(onTasksComplete, onTasksError).finally(onFinishedSearch);
        }

        $scope.onClearSearch = function () {
            $scope.fileName = "";
            $scope.serverName = "";
            $scope.status = "";
            $scope.currentPage = 0;
            $scope.onGetTasksWithFilters();
        };

        // When no filter criteria are set, the API returns active tasks (including those tasks completed in the last 5 minutes).
        $scope.onGetTasksWithFilters();
        
        $scope.taskCounter = null;
        $scope.isNormalBanner = true;

        $scope.setRowColour = function(status) {
            var colourClass = "";
            switch (status) {
            case "Error":
                colourClass = "text-error";
                break;
            case "Pending":
                colourClass = "text-warning";
                break;
            case "In Progress":
                colourClass = "text-primary";
                break;
            case "Completed":
                    colourClass = "text-success";
                    break;
                case "Cancelled":
                    colourClass = "text-muted";
                    break;
            default:
                break;
            }
            return colourClass;
        };

        $scope.currentPage = 0;
        $scope.pageSize = 20;
        $scope.numberOfPages = function () {
            if ($scope.transporterTasks && $scope.transporterTasks.length > 0) {
                return Math.ceil($scope.transporterTasks.length / $scope.pageSize);
            };
            return 1;
        };

        // Clean up
        $scope.$on('$destroy', function() {
            $interval.cancel(refreshTasks);
        });
    };

    tasks.controller("tasksController", tasksController);

}());