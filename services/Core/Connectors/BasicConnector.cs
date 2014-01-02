using Core.Entities;
using Core.Expressions;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Core.BLL;

namespace Core.Connectors
{
    public abstract class BasicConnector : IConnector
    {
        private Selector _detailsSelector;
        private IVerificator _verificator;

        public abstract string Id { get; }
        public abstract string PageUrlFormat { get; }
        
        public abstract Selector CreateSelector();
        public abstract Ad CreateAd(Match match);

        public virtual int GetAvailablePagesCount(string pageUrlFormat)
        {
            return int.MaxValue;
        }

        public virtual WebClientOptions GetWebClientOptions()
        {
            return WebClientOptions.Default;
        }

        protected virtual IEnumerable<string> GetPageUrlFormats()
        {
            yield return PageUrlFormat;
        }

        protected virtual IVerificator GetVerificator()
        {
            if (_verificator == null)
            {
                _verificator = new RealtyVerificator();
            }
            return _verificator;
        }

        protected virtual string VerifyAd(Ad ad)
        {
            IVerificator verificator = GetVerificator();
            return verificator.Verify(ad);
        }

        public IEnumerable<Ad> GetAds()
        {
            Selector slector = CreateSelector();
            int adsCountOnLastPage = 0;
            int maxErrorsPerPage = 10;
            
            foreach (var pageUrlFormat in GetPageUrlFormats())
            {
                int maxPages = GetAvailablePagesCount(pageUrlFormat);
                for (int i = 1; i <= maxPages; i++)
                {
                    string content = WebHelper.GetStringFromUrl(string.Format(pageUrlFormat, i));
                    int errorsCount = 0;
                    var matches = slector.Match(content).ToList();
                    if (i > 1 && matches.Count != adsCountOnLastPage)
                    {
                        Managers.LogEntriesManager.AddItem(SeverityLevel.Warning,
                            string.Format("{0} Different ads count on different pages. Last Page: {1}; Current Page: {2}", this.GetType().Name, adsCountOnLastPage, matches.Count));
                    }
                    adsCountOnLastPage = matches.Count;
                    if (matches.Count > 0)
                    {
                        foreach (var match in matches)
                        {
                            Ad ad = null;
                            try
                            {
                                ad = CreateAd(match);
                                if (ad == null)
                                {
                                    continue;
                                }
                                string verificationResult = VerifyAd(ad);
                                if (verificationResult != null)
                                {
                                    throw new Exception(verificationResult);
                                }
                            }
                            catch (Exception ex)
                            {
                                Managers.LogEntriesManager.AddItem(SeverityLevel.Error,
                                    string.Format("{0} Faild to parse page {1}. Match: {2}. Exception: {3}", this.GetType().Name, i, match, ex.Message), ex.StackTrace);
                                errorsCount++;
                                if (errorsCount > maxErrorsPerPage)
                                {
                                    throw;
                                }
                                continue;
                            }
                            yield return ad;
                        }
                    }
                    else
                    {
                        Managers.LogEntriesManager.AddItem(SeverityLevel.Warning,
                            string.Format("{0} No matches found on page {1}", this.GetType().Name, i), content);
                        break;
                    }
                }
            }
        }

        public virtual Selector CreateDetailsSelector()
        {
            return null;
        }

        public virtual void FillAdDetails(Ad ad, Match match)
        {
        }

        public virtual bool FillDetails(Ad ad)
        {
            if (_detailsSelector != null)
            {
                WebClientResult webResult = WebHelper.GetStringFromUrl(ad.Url, GetWebClientOptions());
                var detailsMatches = _detailsSelector.Match(webResult.Content).ToList();
                if (detailsMatches.Count == 1)
                {
                    FillAdDetails(ad, detailsMatches[0]);
                    IVerificator verificator = new RealtyVerificator();
                    if (verificator.Verify(ad) != null)
                    {
                        throw new Exception(verificator.Verify(ad));
                    }

                    return true;
                }
                else
                {
                    Managers.LogEntriesManager.AddItem(SeverityLevel.Warning,
                        string.Format("{0} No matches found on details page {1}", this.GetType().Name, ad.Url), webResult.Content);
                }
            }
            return false;
        }

        public BasicConnector()
        {
            _detailsSelector = CreateDetailsSelector();
        }
    }
}
