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
				return _model.Title;
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
                if (_model.Images != null && _model.Images.Count > 0)
                {
                    throw new Exception("Images");
                }
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
