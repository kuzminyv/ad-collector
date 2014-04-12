using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using Core.Entities.Enums;
using Core.Entities.Common;

namespace Core.Entities
{
    [DataContract]
	[Serializable]
    [KnownType(typeof(AdRealty))]
    [KnownType(typeof(BuildingType))]
    [KnownType(typeof(DetailsDownloadStatus))]
    [KnownType(typeof(Metadata))]
    [KnownType(typeof(AdImage))]
	public class Ad : Entity<int>
	{
        [DataMember(Name="title")]
		public string Title
		{
			get;
			set;
		}

        [DataMember(Name = "description")]
        public string Description
		{
			get;
			set;
		}

        [DataMember(Name = "publishDate")]
        public DateTime PublishDate
		{
			get;
			set;
		}

        [DataMember(Name = "collectDate")]
        public DateTime CollectDate
        {
            get;
            set;
        }

        [DataMember(Name = "creationDate")]
        public DateTime? CreationDate
        {
            get;
            set;
        }

        [DataMember(Name = "url")]
        public string Url
		{
			get;
			set;
		}

        [DataMember(Name = "connectorId")]
		public string ConnectorId
		{
			get;
			set;
		}

        [DataMember(Name = "isNew")]
		public bool IsNew
		{
			get;
			set;
		}

        [DataMember(Name = "price")]
        public double Price
        {
            get;
            set;
        }

        [DataMember(Name = "idOnWebSite")]
        public string IdOnWebSite
        {
            get;
            set;
        }

        [DataMember(Name = "isSuspicious")]
        public bool IsSuspicious
        {
            get;
            set;
        }

        [DataMember(Name = "detailsDownloadStatus")]
        public DetailsDownloadStatus DetailsDownloadStatus
        {
            get;
            set;
        }

        #region Optional properties
        [Optional]
        [DataMember(Name = "linkedAdsCount")]
        public int? LinkedAdsCount
        {
            get;
            set;
        }

        [NonSerialized]
        private int? _historyLength;

        [Optional]
        [DataMember(Name = "historyItemsCount")]
        public int? HistoryLength
        {
            get
            {
                return _historyLength;
            }
            set
            {
                _historyLength = value;
            }
        }

        [NonSerialized]
        private Metadata _metadata;

        [Optional]
        [DataMember(Name = "metadata")]
        public Metadata Metadata
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        [NonSerialized]
        private List<AdHistoryItem> _history;

        [Optional]
        [DataMember(Name = "History")]
        public List<AdHistoryItem> History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        [NonSerialized]
        private List<AdImage> _images;

        [Optional]
        [DataMember(Name = "images")]
        public List<AdImage> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
            }
        }
        #endregion

        public virtual bool IsSameAd(Ad ad)
        {
            return 
                this.Description == ad.Description && 
                this.Title == ad.Title && 
                this.Price == ad.Price &&
                this.IdOnWebSite == ad.IdOnWebSite;
        }

        public virtual bool IsSimilarObject(Ad ad)
        {
            return false;
        }
	}
}
