using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Expressions
{
    public abstract class Selector
    {
        private readonly string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private readonly Selector[] _selectors;
        public IEnumerable<Selector> Selectors
        {
            get
            {
                return _selectors;
            }
        }

        public virtual IEnumerable<Match> Match(string input)
        {
            var thisMatch = MatchAtom(input);
            foreach (var match in thisMatch)
            {
                foreach (var selector in Selectors)
                {
                    var childMatch = selector.Match(match.Value);
                    match.AddMatchRange(childMatch);
                }
            }
            return thisMatch;
        }

        public abstract IEnumerable<Match> MatchAtom(string input);

        public Selector(string name, params Selector[] selectors)
        {
            _name = name;
            _selectors = selectors;
        }

    }
}
