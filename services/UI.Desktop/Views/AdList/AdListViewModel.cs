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
using Utils.DataVirtualization;
using Core.Utils;
using System.Windows.Threading;

namespace UI.Desktop.Views
{
    public class AdListViewModel : BaseWindowModel
	{
		private AsyncVirtualizingCollection<AdItemViewModel> _items;
		public AsyncVirtualizingCollection<AdItemViewModel> Items
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

        private string _processedAdsCount;
        public string ProcessedAdsCount
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

        private string _frameStatus;
        public string FrameStatus
        {
            get
            {
                return _frameStatus;
            }
            set
            {
                if (value != _frameStatus)
                {
                    _frameStatus = value;
                    OnPropertyChanged("FrameStatus");
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
                        new KeyValuePair<string, string>("Price", "Price"),
                        new KeyValuePair<string, string>("PricePerMeter", "Price per Meter")
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

        private int? _priceMin;
        public int? PriceMin
        {
            get
            {
                return _priceMin;
            }
            set
            {
                if (_priceMin != value)
                {
                    _priceMin = value;
                    ApplyFilter();
                    OnPropertyChanged("PriceMin");
                }
            }
        }

        private int? _priceMax;
        public int? PriceMax
        {
            get
            {
                return _priceMax;
            }
            set
            {
                if (_priceMax != value)
                {
                    _priceMax = value;
                    ApplyFilter();
                    OnPropertyChanged("PriceMax");
                }
            }
        }

        private int? _pricePerMeterMax;
        public int? PricePerMeterMax
        {
            get
            {
                return _pricePerMeterMax;
            }
            set
            {
                if (_pricePerMeterMax != value)
                {
                    _pricePerMeterMax = value;
                    ApplyFilter();
                    OnPropertyChanged("PricePerMeterMax");
                }
            }
        }

        private int? _pricePerMeterMin;
        public int? PricePerMeterMin
        {
            get
            {
                return _pricePerMeterMin;
            }
            set
            {
                if (_pricePerMeterMin != value)
                {
                    _pricePerMeterMin = value;
                    ApplyFilter();
                    OnPropertyChanged("PricePerMeterMin");
                }
            }
        }

        private int? _livingSpaceMin;
        public int? LivingSpaceMin
        {
            get
            {
                return _livingSpaceMin;
            }
            set
            {
                if (_livingSpaceMin != value)
                {
                    _livingSpaceMin = value;
                    ApplyFilter();
                    OnPropertyChanged("LivingSpaceMin");
                }
            }
        }

        private int? _livingSpaceMax;
        public int? LivingSpaceMax
        {
            get
            {
                return _livingSpaceMax;
            }
            set
            {
                if (_livingSpaceMax != value)
                {
                    _livingSpaceMax = value;
                    ApplyFilter();
                    OnPropertyChanged("LivingSpaceMax");
                }
            }
        }
       
        private int? _floorMin;
        public int? FloorMin
        {
            get
            {
                return _floorMin;
            }
            set
            {
                if (_floorMin != value)
                {
                    _floorMin = value;
                    ApplyFilter();
                    OnPropertyChanged("FloorMin");
                }
            }
        }

        private bool _isFavoriteFilter;
        public bool IsFavoriteFilter
        {
            get
            {
                return _isFavoriteFilter;
            }
            set
            {
                if (_isFavoriteFilter != value)
                {
                    _isFavoriteFilter = value;
                    ApplyFilter();
                    OnPropertyChanged("IsFavoriteFilter");
                }
            }
        }

        private DateTime? _publishDateMin;
        public DateTime? PublishDateMin
        {
            get
            {
                return _publishDateMin;
            }
            set
            {
                if (_publishDateMin != value)
                {
                    _publishDateMin = value;
                    ApplyFilter();
                    OnPropertyChanged("PublishDateMin");
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
            App.AppContext.CheckForNewAdsComplete += AppContext_CheckForNewAdsComplete;
            App.AppContext.CheckForNewAdsStateChanged += AppContext_CheckForNewAdsStateChanged;
            App.AppContext.CheckForNewAdsStart += AppContext_CheckForNewAdsStart;
            StatusText = "Ready";

            _sortBySelectedItem = SortByItems.First();
            _sortOrderSelectedItem = SortOrderItems.First();
            RestoreUserSession();
        }

        public override bool WindowClosing()
        {
            SaveUserSession();
            return base.WindowClosing();
        }

        private void RestoreUserSession()
        {
            var userProfile = Managers.UserProfileManager.GetItem(1);
            if (userProfile != null && userProfile.AdsQuery != null)
            {
                AdsQuery lastQuery = userProfile.AdsQuery;
                _textFilter = lastQuery.SearchExpression;
                _priceMin = (int?)lastQuery.PriceMin;
                _priceMax = (int?)lastQuery.PriceMax;
                _pricePerMeterMin = (int?)lastQuery.PricePerMeterMin;
                _pricePerMeterMax = (int?)lastQuery.PricePerMeterMax;
                _floorMin = lastQuery.FloorMin;
                _sortOrderSelectedItem = SortOrderItems.Where(item => item.Key == lastQuery.SortOrder).First();
            }
        }

        private void SaveUserSession()
        {
            var userProfile = Managers.UserProfileManager.GetItem(1) ?? new UserProfile() { UserId = 1 };
            if (userProfile.AdsQuery == null)
            {
                userProfile.AdsQuery = new AdsQuery();
            }
            AdsQuery lastQuery = userProfile.AdsQuery;
            lastQuery.SearchExpression = TextFilter;
            lastQuery.PriceMin = PriceMin;
            lastQuery.PriceMax = PriceMax;
            lastQuery.PricePerMeterMin = PricePerMeterMin;
            lastQuery.PricePerMeterMax = PricePerMeterMax;
            lastQuery.FloorMin = FloorMin;
            lastQuery.SortOrder = SortOrderSelectedItem.Key;

            Managers.UserProfileManager.SaveItem(userProfile);
        }

        private void ApplyFilter()
        {
            BufferedAction.DelayAction(() => App.AppContext.Dispatcher.BeginInvoke(new Action(StartLoadItems), DispatcherPriority.Normal), 1000);
        }

        private void AppContext_CheckForNewAdsStart(object sender, EventArgs e)
        {
            StatusText = "Downloading...";
            ProcessedAdsCount = "";
            FrameStatus = "";
        }

        private void AppContext_CheckForNewAdsStateChanged(object sender, EventArgs<CheckForNewAdsState> e)
        {
            ProcessedAdsCount = e.Value.ProgressTotal > 0 ? string.Format("{0}/{1}", (int)e.Value.Progress, (int)e.Value.ProgressTotal) : e.Value.Progress.ToString();
            StatusText = e.Value.Description + " " + e.Value.SourceUrl;

            FrameStatus = string.Format("{0}/{1} {2,7:f}%", e.Value.FrameNewAds, e.Value.FrameTotalAds, e.Value.FrameProgress * 100);

        }

        private void AppContext_CheckForNewAdsComplete(object sender, EventArgs<List<Core.Entities.Ad>> e)
        {
            StatusText = "Complete";
            NewAdsCount = e.Value.Count;
            Items = null;
        }

        private Query BuildQuery()
        {
            Query query = new Query(0, 10000);
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
            if (PricePerMeterMax.HasValue && PricePerMeterMax.Value > 0)
            {
                query.AddFilter("PricePerMeterMax", PricePerMeterMax.Value);
            }
            if (PricePerMeterMin.HasValue && PricePerMeterMin.Value > 0)
            {
                query.AddFilter("PricePerMeterMin", PricePerMeterMin.Value);
            }
            if (LivingSpaceMin.HasValue && LivingSpaceMin.Value > 0)
            {
                query.AddFilter("LivingSpaceMin", LivingSpaceMin.Value);
            }
            if (LivingSpaceMax.HasValue && LivingSpaceMax.Value > 0)
            {
                query.AddFilter("LivingSpaceMax", LivingSpaceMax.Value);
            }
            if (FloorMin.HasValue && FloorMin.Value > 0)
            {
                query.AddFilter("FloorMin", FloorMin.Value);
            }
            if (PublishDateMin.HasValue)
            {
                query.AddFilter("PublishDateMin", PublishDateMin.Value);
            }

            if (IsFavoriteFilter)
            {
                query.AddFilter("IsFavorite", true);
            }
            return query;
        }

        private void StartLoadItems()
        {
            Items = new AsyncVirtualizingCollection<AdItemViewModel>(new AdListProvider(BuildQuery()), 10, int.MaxValue);
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                TotalAdsCount = Items.Count;
            }
        }
	}
}
