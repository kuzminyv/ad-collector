angular.module('ac.realty').factory('Realty', ['$resource', function ($resource) {
    var serviceUrl = 'http://109.195.19.113:8733/AdCollectorServicesDEV/AdsService.svc';

    return $resource(serviceUrl + '/ads/:adId',
        { adId: '@_id' },
        {
            query: { method: 'GET', isArray: false },
            update: { method: 'PUT' }
        });
}]);