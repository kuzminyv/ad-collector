using Core.BLL;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.DataVirtualization;

namespace UI.Desktop.Views
{
    public class AdListProvider : IItemsProvider<AdItemViewModel>
    {
        private Query _query;
        private QueryResult<Ad> _fetchCountResult;
        public int FetchCount()
        {
            _query.Start = 0;
            _query.Limit = 100;
            _fetchCountResult = Managers.AdManager.GetAds(_query);
            return _fetchCountResult.TotalCount.Value;
        }

        public IList<AdItemViewModel> FetchRange(int startIndex, int count)
        {
            if (_fetchCountResult != null && startIndex == 0 && count == 100)
            {
                var result = _fetchCountResult.Items.Select(i => new AdItemViewModel(i)).ToList();
                _fetchCountResult = null;
                return result;
            }
            _query.Start = startIndex;
            _query.Limit = count;
            return Managers.AdManager.GetAds(_query).Items.Select(i => new AdItemViewModel(i)).ToList();
        }

        public AdListProvider(Query query)
        {
            _query = query;
        }
    }
}
