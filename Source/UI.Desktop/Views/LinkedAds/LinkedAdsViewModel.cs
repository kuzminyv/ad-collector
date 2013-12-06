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
	public class LinkedAdsViewModel : ViewModel
	{
        private AsyncOperation<int, List<Ad>> _loadItemsOperation;
        private Ad _model;

		private ObservableCollection<AdItemViewModel> _items;
		public ObservableCollection<AdItemViewModel> Items
		{
			get
			{
				if (_items == null)
				{
                    StartLoadItems();
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


		private AdItemViewModel _selectedItem;
		public AdItemViewModel SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				if (_selectedItem != value)
				{
					if (_selectedItem != null)
					{
						_selectedItem.IsNew = false;
					}
					_selectedItem = value;
					OnPropertyChanged("SelectedItem");
				}
			}
		}


        public LinkedAdsViewModel(Ad model)
        {
            _model = model;
            _loadItemsOperation = new AsyncOperation<int, List<Ad>>((id, token) => Managers.AdManager.GetLinkedAds(id), ItemsLoaded, 0, true);
        }

        private void ApplyFilter()
        {
            StartLoadItems();
        }

        private void StartLoadItems()
        {
            _loadItemsOperation.RunAsync(_model.Id);
        }

        private void ItemsLoaded(List<Ad> result)
        {
            Items = new ObservableCollection<AdItemViewModel>(result.Select(ad => new AdItemViewModel(ad)));

        }
	}
}
