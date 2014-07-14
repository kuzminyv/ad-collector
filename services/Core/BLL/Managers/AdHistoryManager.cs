using Core.BLL.Common;
using Core.DAL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.BLL
{
    public class AdHistoryManager
    {
        public CancellationTokenSource CreateHistoryAsync(Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            return TaskHelper.ExecuteAsync(CreateHistory, stateChangedCallback, completedCallback);
        }

        public List<AdHistoryItem> GetAdHistory(int adId)
        {
            return Repositories.AdHistoryItemsRepository.GetList(adId);
        }

        protected void CreateHistory(Action<OperationState> stateChangedCallback, Action completedCallback, CancellationToken cancellationToken)
        {
            //1. Group By Objects
            //2. For Every Group retrive group items
            //3. Select from group all but newest
            //4. Drop all but newest
            //5. CreateHistory items
            OperationState state = new OperationState();

            var objects = Repositories.AdsRepository.GetAdsObjects<AdRealty>();

            state.ProgressTotal = objects.Count();
            stateChangedCallback(state);

            foreach (var obj in objects)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    state.Canceled = true;
                    stateChangedCallback(state);
                    break;
                }

                var objAds = Repositories.AdsRepository.GetAdsForTheSameObject(obj, false).OrderByDescending(a => a.CollectDate);
                Repositories.AdsRepository.DeleteItems(objAds.Skip(1).Select(a => a.Id).ToList());
                AdRealty newestAd = (AdRealty)objAds.First();

                Repositories.AdHistoryItemsRepository.AddList(objAds.Skip(1)
                    .Select(a => new AdHistoryItem()
                    {
                        AdId = newestAd.Id,
                        AdCollectDate = a.CollectDate,
                        AdPublishDate = a.PublishDate,
                        Price = a.Price
                    })
                    .ToList());
                state.Progress++;
                stateChangedCallback(state);
            }

            completedCallback();
        }

    }
}
