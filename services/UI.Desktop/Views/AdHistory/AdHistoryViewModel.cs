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
	public class AdHistoryViewModel : ViewModel
	{
        private Ad _ad;

		private ObservableCollection<AdHistoryItemViewModel> _items;
        public ObservableCollection<AdHistoryItemViewModel> Items
		{
			get
			{
				if (_items == null)
				{
                    _items = new ObservableCollection<AdHistoryItemViewModel>(
                        Managers.AdHistoryManager.GetAdHistory(_ad.Id).Select(h => new AdHistoryItemViewModel(h)));
				}
				return _items;
			}
		}


	
        public AdHistoryViewModel(Ad ad)
        {
            _ad = ad;
        }
	}
}
