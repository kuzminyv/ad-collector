//Global service for global variables
angular.module('ac.system').factory("Global", [
    function() {
        var _this = this;
        _this._data = {
            user: window.user,
            authenticated: !! window.user,
            realty: {
                search: {
                    query: ''
                }
            }
        };

        return _this._data;
    }
]);
