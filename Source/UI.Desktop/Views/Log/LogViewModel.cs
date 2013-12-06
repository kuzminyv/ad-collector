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
	public class LogViewModel : BaseWindowModel
	{
        private ObservableCollection<LogEntryViewModel> _items;
        public ObservableCollection<LogEntryViewModel> Items
		{
			get
			{
                if (_items == null)
                {
                    _items = new ObservableCollection<LogEntryViewModel>(Managers.LogEntriesManager.GetList().Select(l => new LogEntryViewModel(l)));
                }
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

        public LogViewModel()
        {
        }
	}
}
