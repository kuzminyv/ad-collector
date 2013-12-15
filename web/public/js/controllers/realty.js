angular.module('mean.realty').controller('RealtyController', [ '$scope', 'Realty', function ($scope, Realty) {
    $scope.ads = [];

    $scope.find = function () {
//        Realty.query(function(queryResult) {
//            $scope.ads = queryResult.items;
//        });
    };

    $scope.search = function(searchData) {
        console.log(searchData);
    };

    $scope.message = 'List of realty';
}]);