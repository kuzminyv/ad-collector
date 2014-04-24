angular.module('ac.realty').factory('Realty', ['$resource', '$http', 'DatetimeTransformer', function ($resource, $http, DatetimeTransformer) {
    var serviceUrl = 'http://109.195.19.113:8733/AdCollectorServicesDEV/AdsService.svc';

    return $resource(serviceUrl + '/ads/:adId',
        { adId: '@_id' },
        {
            query: { method: 'GET', isArray: false, transformResponse: DatetimeTransformer.transformResponse },
            update: { method: 'PUT' }
        });

//    function transformResponse() {
//        var transformers = $http.defaults.transformResponse.slice(0);
//        transformers.push(function(data) {
//            if (data.items) {
//                angular.forEach(data.items, function(item) {
//                    if (item.publishDate) {
//                        item.publishDate = new Date(parseInt(item.publishDate.substr(6)));
//                    }
//                });
//            }
//            return data;
//        });
//
//        return transformers;
//    }
}]);