var path = require('path'),
rootPath = path.normalize(__dirname + '/../..');

module.exports = {
	root: rootPath,
	port: process.env.PORT || 3000,
    db: process.env.MONGOHQ_URL,
    serviceUrl: 'http://109.195.19.113:8733/AdCollectorServicesDEV/AdsService.svc'
}
