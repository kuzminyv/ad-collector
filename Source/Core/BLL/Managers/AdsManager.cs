using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Connectors;
using Core.DAL;
using System.Linq;
using Core.DAL.Common;
using System.Threading;
using System.Threading.Tasks;
using Core.BLL.Common;
using Core.Entities.Enums;

namespace Core.BLL
{

	public class AdsManager
	{
        private enum AdAcceptance
        {
            Rejected,
            History,
            Accepted
        }

        public List<Ad> GetAds()
        {
            return this.GetAds(null).Items;
        }

        public QueryResult<Ad> GetAds(Query query)
        {
            return Repositories.AdsRepository.GetList(query);
        }

        public Ad GetItem(int id)
        {
            return Repositories.AdsRepository.GetItem(id);
        }

        public List<Ad> GetLinkedAds(int adId)
        {
            return Repositories.AdsRepository.GetLinkedAds(adId);
        } 

        public CancellationTokenSource CheckForNewAdsAsync(Action<CheckForNewAdsState> stateChangedCallback, Action<List<Ad>> completedCallback)
        {
            var tokenSource = new CancellationTokenSource();
            var t = Task<List<Ad>>.Factory.StartNew(() => CheckForNewAds(stateChangedCallback, completedCallback, tokenSource.Token));
            return tokenSource;
        }

        protected List<Ad> CheckForNewAds(Action<CheckForNewAdsState> stateChangedCallback, Action<List<Ad>> completedCallback, CancellationToken cancelationToken)
		{
            var state = new CheckForNewAdsState();
            var result = new List<Ad>();
            int totalProcessed = 0;

            int maxAds = Managers.SettingsManager.GetSettings().CheckForNewAdsMaxAdsCount;
            int frameSize = 100;
            int minAds = 200;
            double minNewAdsInFrame = 0.1;

            foreach (var connector in Managers.ConnectorsManager.GetConnectors())
            {
                Managers.LogEntriesManager.AddItem(SeverityLevel.Information, string.Format("Starting download from {0}.", connector.Id));
                int adsCount = 0;

                List<Ad> connectorResult = new List<Ad>();
                List<Ad> lastAds = Repositories.AdsRepository.GetLastAds(connector.Id, maxAds);
                Ad lastAd = lastAds.FirstOrDefault();
                DateTime lastCollectionDate = lastAd == null ? DateTime.Now.Date : lastAd.CollectDate.Date;


                Queue<bool> frame = new Queue<bool>(frameSize);/*true for new or republished ad, otherwise false*/

                try
                {
                    foreach (var ad in connector.GetAds())
                    {
                        adsCount++;
                        totalProcessed++;
                        var isNewAd = IsNewOrRepublishedAd(ad, lastAds);

                        state.Progress = totalProcessed;
                        state.SourceUrl = connector.Id;
                        state.Canceled = cancelationToken.IsCancellationRequested;
                        state.Description = "Processing...";
                        stateChangedCallback(state);

                        connectorResult.Add(ad);
                        frame.Enqueue(isNewAd);
                        if (frame.Count > frameSize)
                        {
                            frame.Dequeue();
                        }

                        if ((adsCount > minAds && frame.Count == frameSize && ((double)frame.Where(item => item).Count()/(double)frameSize) < minNewAdsInFrame) ||
                            adsCount >= maxAds || cancelationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }

                    ProcessCollectDate(connectorResult, DateTime.Now);

                    state.Description = "Analyzing...";
                    stateChangedCallback(state);

                    var acceptance = AddNewOrCreateHistory(connectorResult);
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Information,
                        string.Format("{0} Collection from {1} finished. Processed: {2}; New ads: {3}; Added to History: {4}; Rejected: {5};",
                        this.GetType().Name, connector.Id,
                        acceptance.Count(),
                        acceptance.Where(a => a == AdAcceptance.Accepted).Count(),
                        acceptance.Where(a => a == AdAcceptance.History).Count(),
                        acceptance.Where(a => a == AdAcceptance.Rejected).Count()));

                    result.AddRange(connectorResult);
                }
                catch (Exception ex)
                {
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Error,
                        string.Format("{0} Download error. {1}", this.GetType().Name, ex.Message), ex.StackTrace);
                }

                if (cancelationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            
            completedCallback(result);
            return result;
		}

        public CancellationTokenSource FillDetailsAsync(Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            var tokenSource = new CancellationTokenSource();
            var t = Task.Factory.StartNew(() => FillDetails(stateChangedCallback, completedCallback, tokenSource.Token));
            return tokenSource;
        }

        protected void FillDetails(Action<OperationState> stateCallback, Action completedCallback, CancellationToken cancelationToken)
        {
            Query query = new Query();
            query.AddFilter("DetailsDownloadStatus", DetailsDownloadStatus.NotDownloaded);
            query.AddSort("CollectDate", SortOrder.Descending);
            var ads = Repositories.AdsRepository.GetList(query).Items;
            FillDetails(stateCallback, completedCallback, cancelationToken, ads);
        }

        protected void FillDetails(Action<OperationState> stateCallback, Action completedCallback, CancellationToken cancelationToken, List<Ad> ads)
        {
            Dictionary<string, IConnector> connectorsCache = new Dictionary<string, IConnector>();
            OperationState state = new OperationState();
            state.ProgressTotal = ads.Count;
            foreach (var ad in ads)
            {
                IConnector connector;
                if (!connectorsCache.TryGetValue(ad.ConnectorId, out connector))
                {
                    connector = Managers.ConnectorsManager.GetById(ad.ConnectorId);
                    connectorsCache.Add(ad.ConnectorId, connector);
                }

                try
                {
                    if (connector.FillDetails(ad))
                    {
                        if (cancelationToken.IsCancellationRequested)
                        {
                            state.Canceled = true;
                            stateCallback(state);
                            break;
                        }

                        ad.DetailsDownloadStatus = DetailsDownloadStatus.Downloaded;
                        Repositories.AdsRepository.UpdateItem(ad);
                        if (ad.Images != null && ad.Images.Count > 0)
                        {
                            Repositories.AdImagesRepository.AddList(ad.Images);
                        }
                    }
                    state.Description = string.Format("Fill details {0}.", ad.ConnectorId);
                    state.ProgressTotal++;
                    stateCallback(state);
                }
                catch (Exception ex)
                {
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Error, 
                        string.Format("Fill details error. Connector: {0}; Ad: {1}.", connector.Id, ad.Id), 
                        string.Format("Exception: {0}\n Stack: {1}\n Url: {2}", ex.Message, ex.StackTrace, ad.Url));
                }                
            }
            completedCallback();
        }

        private void FillDetails(IConnector connector, List<Ad> connectorResult)
        {
            foreach (var ad in connectorResult)
            {
                connector.FillDetails(ad);
            }
        }

        private List<AdAcceptance> AddNewOrCreateHistory(List<Ad> ads)
        {
            int historyAcceptanceHours = 168;
            var result = new List<AdAcceptance>(ads.Count);
            for (int i = ads.Count - 1; i >= 0; i--)
            {
                var ad = ads[i];
                var adForSameObject = Repositories.AdsRepository.GetAdsForTheSameObject(ad).FirstOrDefault();
                if (adForSameObject != null)
                {
                    var history = Repositories.AdHistoryItemsRepository.GetList(adForSameObject.Id).OrderByDescending(hi => hi.AdCollectDate).FirstOrDefault();
                    if ((ad.CollectDate - adForSameObject.CollectDate).Hours < historyAcceptanceHours && ad.Price == adForSameObject.Price)
                    {
                        result.Add(AdAcceptance.Rejected);
                    }
                    else 
                    {
                        AdHistoryItem historyItem = new AdHistoryItem()
                        {
                            AdCollectDate = adForSameObject.CollectDate,
                            AdPublishDate = adForSameObject.PublishDate,
                            AdId = adForSameObject.Id,
                            Price = adForSameObject.Price
                        };

                        Repositories.AdHistoryItemsRepository.AddItem(historyItem);

                        adForSameObject.PublishDate = ad.PublishDate;
                        adForSameObject.CollectDate = ad.CollectDate;
                        adForSameObject.Url = ad.Url;
                        adForSameObject.IdOnWebSite = ad.IdOnWebSite;
                        adForSameObject.Price = ad.Price;
                        Repositories.AdsRepository.UpdateItem(adForSameObject);
                        result.Add(AdAcceptance.History);
                    }
                }
                else
                {
                    Repositories.AdsRepository.AddItem(ad);
                    result.Add(AdAcceptance.Accepted);
                }
            }
            return result;
        }

        protected void ProcessCollectDate(List<Ad> ads, DateTime collectDate)
        {
            for (int i = 0; i < ads.Count; i++)
            {
                ads[i].CollectDate = collectDate.AddTicks(ads.Count - i - 1);
            }
        }

        protected bool IsNewOrRepublishedAd(Ad ad, List<Ad> lastAds)
        {
            return !lastAds.Any(a => a.IsSameAd(ad) && (ad.PublishDate - a.PublishDate).Hours < 48/*if difference in time more than 48 hours we consider that this is republished ad*/);
        }
	}
}
