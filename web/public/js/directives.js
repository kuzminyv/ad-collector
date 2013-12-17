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
    });