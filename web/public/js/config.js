//Setting up route
angular.module('ac').config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/', {
                templateUrl: 'views/realty/list.html'
            }).
            otherwise({
                redirectTo: '/'
            });
    }
]);

//Setting HTML5 Location Mode
angular.module('ac').config(['$locationProvider',
    function ($locationProvider) {
        $locationProvider.hashPrefix("!");
    }
]);