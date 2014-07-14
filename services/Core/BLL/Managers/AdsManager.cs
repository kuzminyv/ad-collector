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

            List<Task> fillDetailsTasks = new List<Task>();

            foreach (var connector in Managers.ConnectorsManager.GetConnectors())
            {
                var options = connector.GetOptions() ?? new ConnectorOptions();
                Managers.LogEntriesManager.AddItem(SeverityLevel.Information, string.Format("Starting download from {0}.", connector.Id));
                int adsCount = 0;

                List<Ad> connectorResult = new List<Ad>();
                List<Ad> lastAds = Repositories.AdsRepository.GetLastAds(connector.Id, maxAds*2);
                Ad lastAd = lastAds.FirstOrDefault();
                DateTime lastCollectionDate = lastAd == null ? DateTime.Now.Date : lastAd.CollectDate.Date;


                Queue<bool> frame = new Queue<bool>(options.FrameSize);/*true for new or republished ad, otherwise false*/

                try
                {
                    foreach (var ad in connector.GetAds())
                    {
                        adsCount++;
                        totalProcessed++;
                        var isNewAd = IsNewOrRepublishedAd(ad, lastAds, options);

                        state.Progress = totalProcessed;
                        state.SourceUrl = connector.Id;
                        state.Canceled = cancelationToken.IsCancellationRequested;
                        state.FrameTotalAds = frame.Count;
                        state.FrameNewAds = frame.Where(item => item).Count();
                        state.FrameProgress = state.FrameNewAds > 0 ? (1 - ((double)state.FrameNewAds / (double)state.FrameTotalAds)) / (1 - options.MinNewAdsInFrame) : 0;

                        state.Description = "Processing...";
                        stateChangedCallback(state);

                        connectorResult.Add(ad);
                        frame.Enqueue(isNewAd);
                        if (frame.Count > options.FrameSize)
                        {
                            frame.Dequeue();
                        }

                        if ((adsCount > options.MinAds && frame.Count == options.FrameSize && ((double)frame.Where(item => item).Count() / (double)options.FrameSize) < options.MinNewAdsInFrame) ||
                            adsCount >= maxAds || cancelationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }

                    ProcessCollectDate(connectorResult, DateTime.Now);

                    state.Description = "Analyzing...";
                    stateChangedCallback(state);

                    var acceptance = AddNewOrCreateHistory(connectorResult, options, state, stateChangedCallback);
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Information,
                        string.Format("{0} Collection from {1} finished. Processed: {2}; New ads: {3}; Added to History: {4}; Rejected: {5};",
                        this.GetType().Name, connector.Id,
                        acceptance.Count(),
                        acceptance.Where(a => a.Value == AdAcceptance.Accepted).Count(),
                        acceptance.Where(a => a.Value == AdAcceptance.History).Count(),
                        acceptance.Where(a => a.Value == AdAcceptance.Rejected).Count()));

                    var adsToFill = acceptance.Where(kvp => (kvp.Value == AdAcceptance.Accepted || kvp.Value == AdAcceptance.History) && kvp.Key.DetailsDownloadStatus == DetailsDownloadStatus.NotDownloaded)
                            .Select(kvp => kvp.Key).ToList();
                    fillDetailsTasks.Add(Task.Run(() =>
                    {
                        FillDetails(
                            st => { state.Description = st.Description; state.Progress = st.Progress; stateChangedCallback(state); },
                            () => { },
                            cancelationToken,
                            adsToFill);
                    }));

                    result.AddRange(connectorResult);
                }
                catch (Exception ex)
                {
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Error,
                        string.Format("{0} Download error. {1}", this.GetType().Name, ex.Message), ex.StackTrace);
                    state.Description = string.Format("Error {0}", ex.Message);
                    stateChangedCallback(state);
                }

                if (cancelationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            Task.WaitAll(fillDetailsTasks.ToArray());

            FillDetails(
                st => { state.Description = st.Description; state.Progress = st.Progress; stateChangedCallback(state); },
                () => { },
                cancelationToken);
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
            state.Progress = 0;

            Dictionary<string, int> errorsByConnectorId = new Dictionary<string, int>();
            int maxErrors = 3;
            foreach (var ad in ads)
            {
                IConnector connector;
                if (!connectorsCache.TryGetValue(ad.ConnectorId, out connector))
                {
                    connector = Managers.ConnectorsManager.GetById(ad.ConnectorId);
                    connectorsCache.Add(ad.ConnectorId, connector);
                    errorsByConnectorId.Add(ad.ConnectorId, 0);
                }

                if (errorsByConnectorId[connector.Id] >= maxErrors)
                {
                    continue;
                }

                try
                {
                    bool result = false;
                    try
                    {
                        result = connector.FillDetails(ad);
                        errorsByConnectorId[connector.Id] = 0;
                        if (!result)
                        {
                            ad.DetailsDownloadStatus = DetailsDownloadStatus.ParserError;
                            Repositories.AdsRepository.UpdateItem(ad);
                        }
                        Thread.Sleep(5000);
                    }
                    catch (Exception fillEx)
                    {
                        errorsByConnectorId[connector.Id] = errorsByConnectorId[connector.Id] + 1;
                        if (fillEx.Message.Contains("404"))
                        {
                            ad.DetailsDownloadStatus = DetailsDownloadStatus.PageNotAccessable;
                        }
                        else
                        {
                            ad.DetailsDownloadStatus = DetailsDownloadStatus.ParserError;
                        }
                        Repositories.AdsRepository.UpdateItem(ad);
                        throw;
                    }

                    if (result)
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
                            Repositories.AdImagesRepository.SetList(ad.Id, ad.Images);
                        }
                    }
                    else
                    {
                        errorsByConnectorId[connector.Id] = errorsByConnectorId[connector.Id] + 1;
                    }
                    state.Description = string.Format("Fill details {0}.", ad.Url);
                    state.Progress++;
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

        private Dictionary<Ad, AdAcceptance> AddNewOrCreateHistory(List<Ad> ads, ConnectorOptions options, CheckForNewAdsState state, Action<CheckForNewAdsState> stateCallback)
        {
            int historyAcceptanceHours = 5*24;
            var result = new Dictionary<Ad, AdAcceptance>(ads.Count);
            for (int i = ads.Count - 1; i >= 0; i--)
            {
                state.Description = string.Format("Analyzing {0}/{1}", ads.Count - i - 1, ads.Count);
                stateCallback(state);
                var ad = ads[i];
                var adForSameObject = Repositories.AdsRepository.GetAdsForTheSameObject(ad, options.IsSupportedIdOnWebSite).FirstOrDefault();
                if (adForSameObject != null)
                {
                    var history = Repositories.AdHistoryItemsRepository.GetList(adForSameObject.Id).OrderByDescending(hi => hi.AdCollectDate).FirstOrDefault();
                    if ((ad.CollectDate - adForSameObject.CollectDate).Hours < historyAcceptanceHours && ad.Price == adForSameObject.Price)
                    {
                        result.Add(ad, AdAcceptance.Rejected);
                    }
                    else 
                    {
                        var historyItems = Repositories.AdHistoryItemsRepository.GetList(adForSameObject.Id).OrderByDescending(item => item.Id);
                        if (historyItems.Count() > 1 && historyItems.First().Price == adForSameObject.Price)
                        {
                            var historyItem = historyItems.First();
                            historyItem.AdCollectDate = adForSameObject.CollectDate;
                            historyItem.AdPublishDate = adForSameObject.PublishDate;
                            historyItem.Price = adForSameObject.Price;
                            Repositories.AdHistoryItemsRepository.UpdateItem(historyItem);
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
                        }

                        adForSameObject.PublishDate = ad.PublishDate;
                        adForSameObject.CollectDate = ad.CollectDate;
                        adForSameObject.ConnectorId = ad.ConnectorId;
                        adForSameObject.Url = ad.Url;
                        adForSameObject.IdOnWebSite = ad.IdOnWebSite;
                        adForSameObject.Price = ad.Price;
                        Repositories.AdsRepository.UpdateItem(adForSameObject);
                        result.Add(adForSameObject, AdAcceptance.History);                        
                    }
                }
                else
                {
                    Repositories.AdsRepository.AddItem(ad);
                    result.Add(ad, AdAcceptance.Accepted);
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

        protected bool IsNewOrRepublishedAd(Ad ad, List<Ad> lastAds, ConnectorOptions options)
        {
            return !lastAds.AsParallel().Any(a =>
                (a.IsSameAd(ad) || (options.IsSupportedIdOnWebSite && a.IdOnWebSite == ad.IdOnWebSite)) && 
                (ad.PublishDate - a.PublishDate).TotalHours < (5*24)/*if difference in time more than 48 hours we consider that this is republished ad*/);
        }
	}
}
