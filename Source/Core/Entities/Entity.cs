using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContract]
    [Serializable]
    public class Entity<TId>
    {
        [DataMember(Name="id")]
        public TId Id
        {
            get;
            set;
        }
    }
}
