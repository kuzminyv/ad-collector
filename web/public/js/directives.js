angular.module('ac')
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
    })
    .directive('acPagination', function () {
        return {
            restrict: 'E',
            scope: {
                total: '=',
                offset: '=',
                itemsPerPage: '=',
                maxPageCount: '='
            },
            templateUrl: '/views/common/ac-pagination.html',
            link: function (scope, element, attrs) {
                scope.total = scope.total || 0;
                scope.offset = scope.offset || 0;
                scope.itemsPerPage = scope.itemsPerPage || 25;
                scope.maxPageCount = scope.maxPageCount || 5;

                scope.pages = function () {
                    var pages = [];
                    var pagesRange = getPagesRange();
                    for (var i = pagesRange.first; i <= pagesRange.last; i++) {
                        pages.push(i);
                    }
                    return pages;
                };

                scope.isActive = function (page) {
                    var pageOffsetMin = scope.itemsPerPage * (page - 1);
                    var pageOffsetMax = scope.itemsPerPage * page;

                    return scope.offset >= pageOffsetMin && scope.offset < pageOffsetMax;
                };

                scope.showTrail = function () {
                    var pagesRange = getPagesRange();
                    return pagesRange.last < getMaxPage();
                };

                scope.showHead = function () {
                    var pagesRange = getPagesRange();
                    return pagesRange.first > getMinPage();
                };

                scope.navigate = function (page) {
                    scope.offset = (page - 1) * scope.itemsPerPage;
                };

                scope.navigateToHead = function () {
                    scope.offset = (getMinPage() - 1) * scope.itemsPerPage;
                };

                scope.navigateToTrail = function () {
                    scope.offset = (getMaxPage() - 1) * scope.itemsPerPage;
                };

                function getPagesRange() {
                    var minPage = getMinPage();
                    var maxPage = getMaxPage();
                    var active = Math.floor(scope.offset / scope.itemsPerPage) + 1;
                    var first = active - Math.floor(scope.maxPageCount / 2);
                    var last = active + Math.floor(scope.maxPageCount / 2);

                    if (first < 1) {
                        var leftDiff = 1 - first;
                        last = last + leftDiff;
                        first = 1;
                    }

                    if (last > maxPage) {
                        var rightDiff = last - maxPage;
                        last = maxPage;
                        first = first - rightDiff;
                    }

                    first = Math.max(minPage, first);
                    last = Math.min(maxPage, last);

                    return {
                        first: first,
                        last: last
                    };
                }

                function getMinPage() {
                    return 1;
                }

                function getMaxPage() {
                    return Math.floor(scope.total / scope.itemsPerPage) + (scope.total % scope.itemsPerPage ? 1 : 0);
                }
            }
        };
    })
    .directive('acAjaxProgressbar', function ($http) {
        return {
            restrict: 'E',
            templateUrl: '/views/common/ac-ajax-progressbar.html',
            scope: {},
            link: function (scope, element, attrs) {
                scope.visible = false;
                scope.percentage = 0;

                $http.defaults.transformRequest.push(function (data, headers) {
                    scope.percentage = 0;
                    scope.visible = true;
                    return data;
                });

                $http.defaults.transformResponse.push(function (data, headers) {
                    scope.percentage = 100;
                    setTimeout(function () {
                        scope.$apply(function () {
                            scope.visible = false;
                            scope.percentage = 0;
                        });
                    }, 1000);

                    return data;
                });
            }
        };
    })
    .directive('acRealtySearch', function() {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/views/common/ac-realty-search.html',
            controller: function($scope, Global){
                $scope.menuItems = [
                    {
                        field: 'PublishDate',
                        direction: 1,
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

                $scope.search = function () {
                    Global.realty.search.query = $scope.query;
                };

                $scope.selectedItem = $scope.menuItems[0];

                $scope.realty = Global.realty;

                $scope.$watch('selectedItem', function(item) {
                    Global.realty.search.sortBy = item.field;
                    Global.realty.search.sortDirection = item.direction;
                });

                $scope.reset = function(){
                    $scope.query = "";
                    $scope.search();
                };
            }
        };
    });