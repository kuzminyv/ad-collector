using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContract]
    [Serializable]
    public class Metadata : Entity<int>
    {
        [DataMember(Name = "adId")]
        public int AdId
        {
            get;
            set;
        }

        [DataMember(Name = "userId")]
        public int UserId
        {
            get;
            set;
        }

        [DataMember(Name = "isFavorite")]
        public bool IsFavorite
        {
            get;
            set;
        }

        [DataMember(Name = "note")]
        public string Note
        {
            get;
            set;
        }
    }
}
