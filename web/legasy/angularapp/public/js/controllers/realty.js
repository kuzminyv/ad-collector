angular.module('ac.realty').controller('RealtyController', [ '$scope', 'Global', 'Realty', function ($scope, Global, Realty) {
    $scope.ads = [];

    $scope.find = function () {
        Realty.query(Global.realty.search, function (queryResult) {
            $scope.ads = queryResult.items;
            Global.realty.total = queryResult.totalCount;
        });
    };

    $scope.$watch(
        function () {
            return Global.realty.search;
        },
        function () {
            $scope.find();
        }, true);

    $scope.realtySearch = Global.realty.search;
}]);