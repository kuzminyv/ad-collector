using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Entities
{
    [Serializable]
    public class Setting : Entity<int>
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
