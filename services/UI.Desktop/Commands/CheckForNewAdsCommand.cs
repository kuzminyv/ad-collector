using Core.BLL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Desktop.Utils;

namespace UI.Desktop.Commands
{
    public class CheckForNewAdsCommand : Command
    {
        public override void Execute(object parameter)
        {
            App.AppContext.CheckForNewAds();
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return !App.AppContext.IsCheckForNewAdsInProgress;
        }

        public CheckForNewAdsCommand()
        {
            App.AppContext.CheckForNewAdsComplete += AppContext_CheckForNewAdsComplete;
        }

        private void AppContext_CheckForNewAdsComplete(object sender, EventArgs<List<Ad>> e)
        {
            OnCanExecuteChanged();
        }
    }
}
