angular.module('mean.realty').directive('search', function(){
    return {
        restrict: 'E',
        templateUrl: '/views/realty/search.html',
        scope: {
            search: '&'
        },
        controller: function ($scope) {
            $scope.submit = function() {
                var searchData = {
                    query: $scope.query
                };

                $scope.search({ searchData: searchData });
            };
        }
    };
});