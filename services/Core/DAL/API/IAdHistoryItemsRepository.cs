using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DAL.Common;
using Core.Entities;

namespace Core.DAL.API
{
    public interface IAdHistoryItemsRepository
    {
        List<AdHistoryItem> GetList(int adId);
        QueryResult<AdHistoryItem> GetList(Query query);
        void AddItem(AdHistoryItem item);
        void AddList(List<AdHistoryItem> list);
        void UpdateItem(AdHistoryItem historyItem);
    }
}
