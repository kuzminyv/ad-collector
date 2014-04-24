angular.module('ac.system')
    .factory('DatetimeTransformer', ['$http', function ($http) {
        var transformers = $http.defaults.transformResponse.slice(0);
        transformers.push(function (data) {
            walk(data);
            return data;
        });

        return {
            transformResponse: transformers
        };

        function walk(obj) {
            if (obj) {
                Object.keys(obj).forEach(function (key) {
                    if (typeof obj[key] != 'object') {
                        obj[key] = tryConvertDate(obj[key]);
                    } else {
                        walk(obj[key]);
                    }
                });
            }
        }

        function tryConvertDate(value) {
            if (typeof value == 'string' &&
                /Date/.test(value)) {
                value = new Date(parseInt(value.substr(6)));
            }
            return value;
        }
    }])
;