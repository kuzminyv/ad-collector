using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
	public class BoundMatch : IEnumerable, IEnumerable<KeyValuePair<string, string>>
	{
		private Dictionary<string, string> _matchValues;

		public string this[string groupName]
		{
			get
			{
                string result;
                if (_matchValues.TryGetValue(groupName, out result))
                {
                    return result;
                }
				return null;
			}
		}

		public BoundMatch(Dictionary<string, string> matchValues)
		{
			_matchValues = matchValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

        public override string ToString()
        {
            return string.Concat(_matchValues.Select(kvp => string.Format("{0} - {1};", kvp.Key, kvp.Value))); 
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _matchValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _matchValues.GetEnumerator();
        }
    }
}
