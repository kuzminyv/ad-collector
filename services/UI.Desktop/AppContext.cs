using Core.BLL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using UI.Desktop.Utils;
using System.Threading;

namespace UI.Desktop
{
    public class AppContext
    {
        public event EventHandler<EventArgs<CheckForNewAdsState>> CheckForNewAdsStateChanged;
        public event EventHandler<EventArgs<List<Ad>>> CheckForNewAdsComplete;
        public event EventHandler CheckForNewAdsStart;

        private Timer _checkForAdsTimer;

        private bool _isCheckForNewAdsInProgress;
        public bool IsCheckForNewAdsInProgress
        {
            get
            {
                return _isCheckForNewAdsInProgress;
            }
        }

        private Dispatcher _dispatcher;
        public Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        public void CheckForNewAds()
        {
            if (_isCheckForNewAdsInProgress)
            {
                return;
            }
            
            Managers.AdManager.CheckForNewAdsAsync(
                (state) => Dispatcher.BeginInvoke(new Action<CheckForNewAdsState>(OnCheckForNewAdsStateChanged), state), 
                (result) => Dispatcher.BeginInvoke(new Action<List<Ad>>(OnCheckForNewAdsComplete), result));
            OnCheckForNewAdsStart();
        }

        protected void OnCheckForNewAdsStateChanged(CheckForNewAdsState state)
        {
            if (CheckForNewAdsStateChanged != null)
            {
                CheckForNewAdsStateChanged(this, new EventArgs<CheckForNewAdsState>(state));
            }
        }

        protected void OnCheckForNewAdsComplete(List<Ad> result)
        {
            _isCheckForNewAdsInProgress = false;
            if (CheckForNewAdsComplete != null)
            {
                CheckForNewAdsComplete(this, new EventArgs<List<Ad>>(result));
            }
        }

        protected void OnCheckForNewAdsStart()
        {
            _isCheckForNewAdsInProgress = true;
            if (CheckForNewAdsStart != null)
            {
                CheckForNewAdsStart(this, EventArgs.Empty);
            }
        }

        public AppContext()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            int checkAdsInterval = Managers.SettingsManager.GetSettings().CheckForNewAdsIntervalMinutes * 60 * 1000;
            _checkForAdsTimer = new Timer(new TimerCallback(checkForAdsTimer_Elapsed), null, checkAdsInterval, checkAdsInterval);
        }

        private void checkForAdsTimer_Elapsed(object o)
        {
            Dispatcher.BeginInvoke(new Action(CheckForNewAds));
        }
    }
}
