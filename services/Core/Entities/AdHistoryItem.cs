using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [DataContract]
    [Serializable]
    public class AdHistoryItem : Entity<int>
    {
        [DataMember(Name = "adId")]
        public int AdId
        {
            get;
            set;
        }

        [DataMember(Name = "adPublishDate")]
        public DateTime AdPublishDate
        {
            get;
            set;
        }

        [DataMember(Name = "adCollectDate")]
        public DateTime AdCollectDate
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
    }
}
