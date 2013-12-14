angular.module('mean.realty').factory('Realty', ['$resource', function ($resource) {
    var serviceUrl = 'http://109.195.19.113:8733/AdCollectorServicesDEV/AdsService.svc';

    return $resource(serviceUrl + '/ads/:adId', {
        adId: '@_id'
    }, {
        update: { method: 'PUT' }
    });
}]);