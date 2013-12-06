using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
    public class BoundExpressionGroup : BoundExpressionToken
	{
		public string Name
		{
			get;
			private set;
		}

        public bool IsOptional
        {
            get;
            private set;
        }

        public BoundSelector[] Selectors
		{
			get;
			private set;
		}

		public BoundSelectorResult GetResult(string input, int startIndex)
		{
			BoundSelectorResult nearestResult = null;
			foreach (var selector in Selectors)
			{
				BoundSelectorResult selectorResult = selector.GetResult(input, startIndex);
				if (selectorResult != null)
				{
					if (nearestResult == null || selectorResult.SelectorRange.Start < nearestResult.SelectorRange.Start)
					{
						nearestResult = selectorResult;
					}
				}
			}
			return nearestResult;
		}

        public BoundExpressionGroup(string name, params BoundSelector[] selectors)
            : this(name, false, selectors)
        { 
        }

        public BoundExpressionGroup(string name, bool isOptional, params BoundSelector[] selectors)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

            IsOptional = isOptional;
			Name = name;
			Selectors = selectors;
		}
	}
}
