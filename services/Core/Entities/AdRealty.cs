using Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContract]
    [Serializable]
    [KnownType(typeof(BuildingType))]
    [KnownType(typeof(DetailsDownloadStatus))]
    [KnownType(typeof(Metadata))]
    [KnownType(typeof(AdImage))]
    public class AdRealty: Ad
    {
        [DataMember(Name = "address")]
        public string Address
        {
            get;
            set;
        }

        [DataMember(Name = "roomsCount")]
        public int RoomsCount
        {
            get;
            set;
        }

        [DataMember(Name = "floor")]
        public int Floor
        {
            get;
            set;
        }

        [DataMember(Name = "floorsCount")]
        public int FloorsCount
        {
            get;
            set;
        }

        [DataMember(Name = "livingSpace")]
        public float LivingSpace
        {
            get;
            set;
        }

        [DataMember(Name = "isNewBuilding")]
        public bool IsNewBuilding
        {
            get;
            set;
        }

        [DataMember(Name = "buildingType")]
        public BuildingType BuildingType
        {
            get;
            set;
        }

        [DataMember(Name = "commissioningDate")]
        public DateTime? CommissioningDate
        {
            get;
            set;
        }

        [DataMember(Name = "agent")]
        public string Agent
        {
            get;
            set;
        }

        public double PricePerMeter
        {
            get
            {
                return LivingSpace > 0 ? Price / LivingSpace : 0;
            }            
        }

        public override bool IsSameAd(Ad ad)
        {
            AdRealty a = (AdRealty)ad;
            return this.Price == a.Price &&
                this.Address == a.Address &&
                this.LivingSpace == a.LivingSpace &&
                this.Floor == a.Floor &&
                this.FloorsCount == a.FloorsCount &&
                this.RoomsCount == a.RoomsCount;
        }

        public override bool IsSimilarObject(Ad ad)
        {
            AdRealty a = (AdRealty)ad;
            return this.Address == a.Address &&
                this.Floor == a.Floor &&
                this.FloorsCount == a.FloorsCount &&
                this.LivingSpace == a.LivingSpace &&
                this.RoomsCount == a.RoomsCount;
        }

        public override string ToString()
        {
            return "ConnectorId: " + this.ConnectorId + "\n" +
                "Description:" + this.Description + "\n" +
                "HistoryLength:" + this.HistoryLength + "\n" +
                "IdOnWebSite:" + this.IdOnWebSite + "\n" +
                "Price:" + this.Price + "\n" +
                "PublishDate:" + this.PublishDate + "\n" +
                "Url:" + this.Url + "\n" +
                "IsNewBuilding:" + this.IsNewBuilding + "\n" +
                "RoomsCount:" + this.RoomsCount + "\n" +
                "LivingSpace:" + this.LivingSpace + "\n" +
                "Floor:" + this.Floor + "\n" +
                "FloorsCount:" + this.FloorsCount + "\n" +
                "CommissioningDate:" + this.CommissioningDate + "\n" +
                "Address:" + this.Address + "\n" +
                "Images:" + (this.Images == null ? "none" : string.Join("\n", this.Images.Select(img => "\t" + img.PreviewUrl + "\t" + img.Url)));        
        }
    }
}
