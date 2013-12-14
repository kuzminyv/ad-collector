angular.module('mean.realty').controller('RealtyController', [ '$scope', 'Realty', function ($scope, Realty) {
    $scope.ads = [];

    $scope.find = function () {
        Realty.query(function(queryResult) {
            $scope.ads = queryResult.items;
        });
    };

    $scope.message = 'List of realty';
}]);