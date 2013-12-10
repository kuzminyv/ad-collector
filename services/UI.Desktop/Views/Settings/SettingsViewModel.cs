using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Collections.ObjectModel;
using Core.BLL;
using UI.Desktop.Utils;
using Core.Entities;
using Core.DAL.Common;

namespace UI.Desktop.Views
{
    public class SettingsViewModel : BaseWindowModel
	{
        private Settings _model;

        public int CheckForNewAdsMaxAdsCount
        {
            get
            {
                return _model.CheckForNewAdsMaxAdsCount;
            }
            set
            {
                if (_model.CheckForNewAdsMaxAdsCount != value)
                {
                    _model.CheckForNewAdsMaxAdsCount = value;
                    OnPropertyChanged("CheckForNewAdsMaxAdsCount");
                }
            }
        }

        public int CheckForNewAdsIntervalMinutes
        {
            get
            {
                return _model.CheckForNewAdsIntervalMinutes;
            }
            set
            {
                if (_model.CheckForNewAdsIntervalMinutes != value)
                {
                    _model.CheckForNewAdsIntervalMinutes = value;
                    OnPropertyChanged("CheckForNewAdsIntervalMinutes");
                }
            }
        }

        public Command ShowLogCommand
        {
            get
            {
                return AppCommands.ShowLogCommand;
            }
        }

        public Command RecreateAutoLinksCommand
        {
            get
            {
                return AppCommands.RecreateAutoLinksCommand;
            }
        }

        public Command ExportCommand
        {
            get
            {
                return AppCommands.ExportCommand;
            }
        }

        protected override void Save(object parameter)
        {
            Managers.SettingsManager.SaveSettings(_model);
            CloseWindow();
        }

        public SettingsViewModel()
        {
            _model = Managers.SettingsManager.GetSettings();
        }
	}
}
