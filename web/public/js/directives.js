angular.module('mean')
    .directive('dropdownMenu', function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                menuItems: '=',
                selectedItem: '='
            },
            link: function (scope, element, attrs) {
                scope.select = function (item) {
                    scope.selectedItem = item;
                };
            },
            templateUrl: "/views/common/dropdown-menu.html"
        };
    });