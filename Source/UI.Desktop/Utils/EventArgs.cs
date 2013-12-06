using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Desktop.Utils
{
    public class EventArgs<TValue> : EventArgs
    {
        private readonly TValue _value;
        public TValue Value
        {
            get
            {
                return _value;
            }
        }

        public EventArgs(TValue value)
        {
            this._value = value;
        }
    }
}
