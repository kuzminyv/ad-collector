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
	public class AdListViewModel : ViewModel
	{
        private AsyncOperation<Query, QueryResult<Ad>> _loadItemsOperation;

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

        private int _totalAdsCount;
        public int TotalAdsCount
        {
            get
            {
                return _totalAdsCount;
            }
            set
            {
                if (value != _totalAdsCount)
                {
                    _totalAdsCount = value;
                    OnPropertyChanged("TotalAdsCount");
                }
            }
        }

        private int _processedAdsCount;
        public int ProcessedAdsCount
        {
            get
            {
                return _processedAdsCount;
            }
            set
            {
                if (value != _processedAdsCount)
                {
                    _processedAdsCount = value;
                    OnPropertyChanged("ProcessedAdsCount");
                }
            }
        }

        private int _newAdsCount;
        public int NewAdsCount
        {
            get
            {
                return _newAdsCount;
            }
            set
            {
                if (value != _newAdsCount)
                {
                    _newAdsCount = value;
                    OnPropertyChanged("NewAdsCount");
                }
            }
        }

        private string _statusText;
        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                if (value != _statusText)
                {
                    _statusText = value;
                    OnPropertyChanged("StatusText");
                }
            }
        }

        private List<KeyValuePair<string, string>> _sortByItems;
        public List<KeyValuePair<string, string>> SortByItems
        {
            get
            {
                if (_sortByItems == null)
                {
                    _sortByItems = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("CollectDate", "Collect Date"),
                        new KeyValuePair<string, string>("PublishDate", "Date"),
                        new KeyValuePair<string, string>("Price", "Price")
                    };
                }
                return _sortByItems;
            }
        }

        private KeyValuePair<string, string> _sortBySelectedItem;
        public KeyValuePair<string, string> SortBySelectedItem
        {
            get
            {
                return _sortBySelectedItem;
            }
            set
            {
                if (value.Key != _sortBySelectedItem.Key)
                {
                    _sortBySelectedItem = value;
                    ApplyFilter();
                    OnPropertyChanged("SortBySelectedItem");
                }
            }
        }

        private List<KeyValuePair<SortOrder, string>> _sortOrderItems;
        public List<KeyValuePair<SortOrder, string>> SortOrderItems
        {
            get
            {
                if (_sortOrderItems == null)
                {
                    _sortOrderItems = new List<KeyValuePair<SortOrder, string>>()
                    {
                        new KeyValuePair<SortOrder, string>(SortOrder.Ascending, "Ascending"),
                        new KeyValuePair<SortOrder, string>(SortOrder.Descending, "Descending")
                    };
                }
                return _sortOrderItems;
            }
        }

        private KeyValuePair<SortOrder, string> _sortOrderSelectedItem;
        public KeyValuePair<SortOrder, string> SortOrderSelectedItem
        {
            get
            {
                return _sortOrderSelectedItem;
            }
            set
            {
                if (value.Key != _sortOrderSelectedItem.Key)
                {
                    _sortOrderSelectedItem = value;
                    ApplyFilter();
                    OnPropertyChanged("SortOrderSelectedItem");
                }
            }
        }

        private string _textFilter;
        public string TextFilter
        {
            get
            {
                return _textFilter;
            }
            set
            {
                if (value != _textFilter)
                {
                    _textFilter = value;
                    ApplyFilter();
                    OnPropertyChanged("TextFilter");
                }
            }
        }

        private int? _minPrice;
        public int? PriceMin
        {
            get
            {
                return _minPrice;
            }
            set
            {
                if (_minPrice != value)
                {
                    _minPrice = value;
                    ApplyFilter();
                    OnPropertyChanged("PriceMin");
                }
            }
        }

        private int? _maxPrice;
        public int? PriceMax
        {
            get
            {
                return _maxPrice;
            }
            set
            {
                if (_maxPrice != value)
                {
                    _maxPrice = value;
                    ApplyFilter();
                    OnPropertyChanged("PriceMax");
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

        public ICommand RefreshCommand
        {
            get
            {
                return AppCommands.CheckForNewAds;
            }
        }

        public ICommand ShowSettingsCommand
        {
            get
            {
                return AppCommands.ShowSettingsCommand;
            }
        }
        
        public AdListViewModel()
        {
            _loadItemsOperation = new AsyncOperation<Query, QueryResult<Ad>>((query, token) => Managers.AdManager.GetAds(query), ItemsLoaded, 300, true);

            App.AppContext.CheckForNewAdsComplete += AppContext_CheckForNewAdsComplete;
            App.AppContext.CheckForNewAdsStateChanged += AppContext_CheckForNewAdsStateChanged;
            App.AppContext.CheckForNewAdsStart += AppContext_CheckForNewAdsStart;
            StatusText = "Ready";

            _sortBySelectedItem = SortByItems.First();
            _sortOrderSelectedItem = SortOrderItems.First();
        }

        private void ApplyFilter()
        {
            StartLoadItems();
        }

        private void AppContext_CheckForNewAdsStart(object sender, EventArgs e)
        {
            StatusText = "Downloading...";
        }

        private void AppContext_CheckForNewAdsStateChanged(object sender, EventArgs<CheckForNewAdsState> e)
        {
            ProcessedAdsCount = (int)e.Value.Progress;
            StatusText = e.Value.Description + " " + e.Value.SourceUrl;

        }

        private void AppContext_CheckForNewAdsComplete(object sender, EventArgs<List<Core.Entities.Ad>> e)
        {
            StatusText = "Complete";
            NewAdsCount = e.Value.Count;
            Items = null;
        }

        private void StartLoadItems()
        {
            Query query = new Query();
            query.AddSort(SortBySelectedItem.Key, SortOrderSelectedItem.Key);
            query.AddFields("LinkedAdsCount");
            query.AddFields("HistoryLength");
            query.AddFields("Images");

            if (!string.IsNullOrEmpty(TextFilter))
            {
                query.AddFilter("TextFilter", TextFilter);
            }

            if (PriceMin.HasValue && PriceMin.Value > 0)
            {
                query.AddFilter("PriceMin", PriceMin.Value);
            }
            if (PriceMax.HasValue && PriceMax.Value > 0)
            {
                query.AddFilter("PriceMax", PriceMax.Value);
            }

            _loadItemsOperation.RunAsync(query);
        }

        private void ItemsLoaded(QueryResult<Ad> result)
        {
            Items = new ObservableCollection<AdItemViewModel>(result.Items.Select(ad => new AdItemViewModel(ad)));
            TotalAdsCount = Items.Count;

        }
	}
}
