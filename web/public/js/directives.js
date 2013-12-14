angular.module('mean.realty').directive('search', function(){
    return {
        restrict: 'E',
        templateUrl: '/views/realty/search.html',
        scope: {},
        controller: function ($scope) {
            $scope.query = "Query";
        }
    };
});