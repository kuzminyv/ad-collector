using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContractAttribute]
    [Serializable]
    public class Street : Entity<int>
    {
        public int LocationId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
