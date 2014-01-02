using Core.BLL;
using Core.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Commands;

namespace UI.Desktop.Views
{
	public class MainViewModel : ViewModel
	{
		private int _newAdsCount;
		public int NewAdsCount
		{
			get
			{
				return _newAdsCount;
			}
			set
			{
				if (_newAdsCount != value)
				{
					_newAdsCount = value;
					OnPropertyChanged("NewAdsCount");
				}
			}
		}

		private int _totalAdsCount;
		public int TotalAdsCount
		{
			get
			{
				return _totalAdsCount;
			}
			set
			{
				if (_totalAdsCount != value)
				{
					_totalAdsCount = value;
					OnPropertyChanged("TotalAdsCount");
				}
			}
		}

		public ICommand ShowAdListCommand
		{
			get
			{
				return AppCommands.ShowAdList;
			}
		}

		public ICommand ShutdownApplicationCommand
		{
			get
			{
				return AppCommands.ApplicationShutdown;
			}
		}

        public MainViewModel()
        {
            TotalAdsCount = 0;

            Query query = new Query(0, 1);
            query.AddFilter("IsNew", true);
            NewAdsCount = Managers.AdManager.GetAds(query).TotalCount.Value;
        }
	}
}
