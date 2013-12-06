using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Common
{
    public class Filter
    {
        public string Name
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)Value;
        }

        public Filter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
