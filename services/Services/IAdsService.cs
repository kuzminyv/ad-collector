using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAdsService
    {
        [OperationContract]
        string GetData(string id);

        [OperationContract]
        QueryResult<AdRealty> GetAds(string query, string priceMin, string priceMax, string sortBy, string sortDirection, string offset, string limit);

        [OperationContract]
        QueryResult<AdHistoryItem> GetAdHistory(string adId);
    }
}
