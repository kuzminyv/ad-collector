angular.module('ac.system').controller('HeaderController', ['$scope', 'Global', function ($scope, Global) {
    $scope.global = Global;

    $scope.menu = [
//        {
//            "title": "Articles",
//            "link": "articles"
//        },
//        {
//            "title": "Create New Article",
//            "link": "articles/create"
//        }
    ];

    $scope.isCollapsed = false;

    $scope.search = function () {
        Global.realty.search.query = $scope.query;
    };

    $scope.menuItems = [
        {
            field: 'PublishDate',
            direction: 0,
            title: 'Newest first'
        },
        {
            type: 'divider'
        },
        {
            type: 'header',
            title: 'Price'
        },
        {
            field: 'Price',
            direction: 0,
            title: 'Low to high'
        },
        {
            field: 'Price',
            direction: 1,
            title: 'High to low'
        }
    ];

    $scope.selectedItem = $scope.menuItems[0];

    $scope.realtySearch = Global.realty.search;

    $scope.$watch('selectedItem', function(item) {
        Global.realty.search.sortBy = item.field;
        Global.realty.search.sortDirection = item.direction;
    });

    $scope.reset = function(){
        $scope.query = "";
        $scope.search();
    };
}]);