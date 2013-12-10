using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContract]
    [Serializable]
    public class AdLink : Entity<int>
    {
        public int AdId
        {
            get;
            set;
        }

        public int LinkedAdId
        {
            get;
            set;
        }

        public LinkType LinkType
        {
            get;
            set;
        }
    }
}
