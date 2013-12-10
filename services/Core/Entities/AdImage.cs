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
    public class AdImage : Entity<int>
    {
        [DataMember(Name = "adId")]
        public int AdId
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

        [DataMember(Name = "previewUrl")]
        public string PreviewUrl
        {
            get;
            set;
        }
    }
}
