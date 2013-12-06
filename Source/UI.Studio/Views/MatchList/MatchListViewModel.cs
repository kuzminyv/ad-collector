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
using UI.Desktop.Views;

namespace UI.Studio.Views
{
	public class MatchListViewModel : ViewModel
	{
        private ObservableCollection<MatchItemViewModel> _items;
        public ObservableCollection<MatchItemViewModel> Items
		{
			get
			{
				return _items;
			}
            set
            {
                if (value != _items)
                {
                    _items = value;
                    OnPropertyChanged("Items");
                }
            }
		}

        public MatchListViewModel()
        {
        }
	}
}
