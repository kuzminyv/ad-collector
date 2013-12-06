using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
    public class BoundSelectorResult
    {
        public BoundSelector Selector
        {
            get;
            private set;
        }

        public StringRange SelectorRange
        {
            get;
            private set;
        }

        public StringRange ValueRange
        {
            get;
            private set;
        }

        public string GetValue(string input)
        {
            if (Selector.Outer)
            {
                return input.Substring(SelectorRange.Start, SelectorRange.Length);
            }
            return input.Substring(ValueRange.Start, ValueRange.Length);
        }

        public BoundSelectorResult(BoundSelector selector, int selectorStart, int selectorEnd, int valueStart, int valueEnd)
        {
            Selector = selector; 
            SelectorRange = new StringRange(selectorStart, selectorEnd);
            ValueRange    = new StringRange(valueStart, valueEnd);
        }
    }
}
