using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Web;
using UI.Desktop.Utils;
using Core.BLL;

namespace UI.Desktop.Views
{
	public class AdItemViewModel : ViewModel
    {
        #region Properties
        private Ad _model;
        public Ad Model
        {
            get
            {
                return _model;
            }
        }

        public int Id
        {
            get
            {
                return _model.Id;
            }
        }

        private MetadataViewModel _metadata;
        public MetadataViewModel Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new MetadataViewModel(this, _model.Metadata ?? new Metadata() { AdId = _model.Id, UserId = 1 });
                }
                return _metadata;
            }
        }

		public string Text
		{
			get
			{
                if (_model is AdRealty)
                {
                    AdRealty ad = (AdRealty)_model;
                    return string.Format(
                        "Price: {0}\n" +
                        "Address: {1}\n" +
                        "Floor: {2} из {3}\n" +
                        "Size: {4}",
                        string.Format("{0:C}", ad.Price) + (ad.LivingSpace > 0 ? string.Format(" {0:C}/м", ad.Price / ad.LivingSpace) : ""),
                        ad.Address,
                        ad.Floor, ad.FloorsCount,
                        ad.LivingSpace);
                }
				return _model.Description;
			}
		}

		public string Header
		{
			get
			{
                return string.Format("{0}, {1}, {2} ", _model.Price, ((AdRealty)_model).Address, ((AdRealty)_model).LivingSpace);
			}
		}

        public string Description
        {
            get
            {
                return _model.Description;
            }
        }

		public DateTime Date
		{
			get
			{
				return _model.PublishDate;
			}
		}

        public DateTime CollectDate
        {
            get
            {
                return _model.CollectDate;
            }
        }

		public string SourceUrl
		{
			get
			{
				return _model.ConnectorId;
			}
		}

		public string DetailsUrl
		{
			get
			{
				return _model.Url;
			}
		}

        public bool Suspect
        {
            get
            {
                return _model.IsSuspicious;
            }
        }

        private bool _isLastHistoryItemCalculated;
        private AdHistoryItem _lastHistoryItem;
        public AdHistoryItem LastHistoryItem
        {
            get
            {
                if (!_isLastHistoryItemCalculated)
                {
                    if (_model.History != null)
                    {
                        _lastHistoryItem = _model.History.OrderByDescending(item => item.AdPublishDate).Where(item => item.Price != _model.Price).FirstOrDefault();
                    }
                    _isLastHistoryItemCalculated = true;
                }
                return _lastHistoryItem;
            }
        }

        private double? _priceDynamic;
        public double? PriceDynamic
        {
            get
            {
                if (_priceDynamic == null)
                {
                    if (LastHistoryItem != null && LastHistoryItem.Price != 0)
                    {
                        _priceDynamic = Math.Round(100d * (_model.Price - LastHistoryItem.Price) / LastHistoryItem.Price, 1);
                    }
                    else
                    {
                        _priceDynamic = 0;
                    }
                }
                return _priceDynamic;
            }
        }

        public string PriceDynamicInfo
        {
            get
            {
                if (PriceDynamic.HasValue && PriceDynamic.Value != 0)
                {
                    return string.Format("{0}% ({1})", PriceDynamic, LastHistoryItem.AdPublishDate.ToString("dd MMM yyyy"));
                }
                return null;
            }
        }

        private bool? _isPriceChanged;
        public bool? IsPriceChanged
        {
            get
            {
                if (_isPriceChanged == null)
                {
                    if (LastHistoryItem != null && _model.History.OrderByDescending(item => item.AdPublishDate).First().Price != _model.Price)
                    {
                        _isPriceChanged = true;
                    }
                    else
                    {
                        _isPriceChanged = false;
                    }
                }
                return _isPriceChanged;
            }
        }

        public bool IsPriceRising
        {
            get
            {
                return PriceDynamic.HasValue && PriceDynamic.Value > 0d;
            }
        }

        public bool IsNewBuilding
        {
            get
            {
                AdRealty adRealty = _model as AdRealty;
                if (adRealty != null)
                {
                    return adRealty.IsNewBuilding;
                }
                return false;
            }
        }

		public bool IsNew
		{
			get
			{
				return _model.IsNew;
			}
			set
			{
				if (_model.IsNew != value)
				{
					_model.IsNew = value;
					OnPropertyChanged("IsNew");
				}
			}
		}

        public string LinkedAdsInfo
        {
            get
            {
                return _model.LinkedAdsCount > 0 ? ("Links: " + _model.LinkedAdsCount.Value) : "";
            }
        }

        public string AdHistoryInfo
        {
            get
            {
                return _model.HistoryLength > 0 ? ("History: " + _model.HistoryLength.Value) : "";
            }
        }

        public string YandexMapsUrl
        {
            get
            {
                return "http://maps.yandex.ru/?text=" + HttpUtility.HtmlEncode("Россия, Саратовская область, Саратов, " + ((AdRealty)_model).Address);
            }
        }

        public List<AdImage> Images
        {
            get
            {
                return _model.Images;
            }
        }
        #endregion

        #region Commands
        public ICommand OpenUrlCommand
		{
			get
			{
				return AppCommands.OpenUrl;
			}
		}

        public ICommand ShowLinkedAdsCommand
        {
            get
            {
                return AppCommands.ShowLinkedAdsCommand;
            }
        }

        public ICommand ShowAdHistoryCommand
        {
            get
            {
                return AppCommands.ShowAdHistoryCommand;
            }
        }
        #endregion

        public AdItemViewModel(Ad model)
		{
			_model = model;
		}
	}
}
