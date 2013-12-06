using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Common
{
    public class Sort
    {
        public string Name
        {
            get;
            set;
        }

        public SortOrder SortOrder
        {
            get;
            set;
        }

        public Sort(string name, SortOrder sortOrder)
        {
            Name = name;
            SortOrder = sortOrder;
        }
    }
}
