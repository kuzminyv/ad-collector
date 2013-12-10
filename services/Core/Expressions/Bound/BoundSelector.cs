using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
	public class BoundSelector
	{
        public bool Superposition
        {
            get;
            set;
        }

        public bool HoldPosition
        {
            get;
            set;
        }

        public bool Outer
        {
            get;
            set;
        }

        public StringComparison StringComparison
        {
            get;
            set;
        }

		public string[] BeforeMarkers
		{
			get;
			private set;
		}

		public string[] AfterMarkers
		{
			get;
			private set;
		}

		public BoundSelector(string[] beforeMarkers, string[] afterMarkers)
		{
			if (beforeMarkers == null)
			{
				throw new ArgumentNullException("beforeMarkers");
			}

			if (afterMarkers == null)
			{
				throw new ArgumentNullException("beforeMarkers");
			}

			if (beforeMarkers.Length == 0 || afterMarkers.Length == 0)
			{
				throw new ArgumentException("Length of beforeMarkers and afterMarkers must more than zero!");
			}

			BeforeMarkers = beforeMarkers;
			AfterMarkers = afterMarkers;
		}

		public BoundSelector(string textBefore, string textAfter)
			: this (new string[] {textBefore}, new string[] {textAfter})
		{
		}

		public BoundSelector(string textBefore1, string textBefore2, string textAfter)
			: this(new string[] { textBefore1, textBefore2 }, new string[] { textAfter })
		{
		}

        public BoundSelector(string textBefore1, string textBefore2, string textBefore3, string textAfter)
            : this(new string[] { textBefore1, textBefore2, textBefore3 }, new string[] { textAfter })
        {
        }

		private StringRange GetMarkersRange(string input, int startIndex, string[] markers)
		{
			int first = input.IndexOf(markers.First(), startIndex, this.StringComparison);
			if (first < 0)
			{
				return null;
			}

			int next = first + markers.First().Length;
			for (int i = 1; i < markers.Length; i++)
			{
				string marker = markers[i];
                next = input.IndexOf(marker, next, this.StringComparison);
				if (next < 0)
				{
					return null;
				}
				next += marker.Length;
			}
			return new StringRange(first, next - 1);
		}

		public BoundSelectorResult GetResult(string input, int startIndex)
		{
			StringRange beforeRange = GetMarkersRange(input, startIndex, BeforeMarkers);
			if (beforeRange == null)
			{
				return null;
			}

			StringRange afterRange = GetMarkersRange(input, beforeRange.End + 1, AfterMarkers);
			if (afterRange == null)
			{
				return null;
			}

			return new BoundSelectorResult(this, beforeRange.Start, afterRange.End, beforeRange.End + 1, afterRange.Start - 1);
		}
	}
}
