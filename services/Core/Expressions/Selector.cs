using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Expressions
{
    public abstract class Selector
    {
        private readonly string _matchFilter;
        public string MatchFilter
        {
            get
            {
                return _matchFilter;
            }
        }

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
            return Match(new Match("", input));
        }

        public virtual IEnumerable<Match> Match(Match match)
        {
            if (string.IsNullOrEmpty(MatchFilter) || match.Name == MatchFilter)
            {
                var thisMatch = MatchAtom(match.Value).ToList();
                foreach (var m in thisMatch)
                {
                    foreach (var selector in Selectors)
                    {
                        var childMatch = selector.Match(m).ToList();
                        m.AddMatchRange(childMatch);
                    }
                }
                return thisMatch;
            }
            return new Match[0];
        }

        public abstract IEnumerable<Match> MatchAtom(string input);

        public Selector(string name, params Selector[] selectors)
            : this(name, null, selectors)
        { 
        }

        public Selector(string name, string matchFilter, params Selector[] selectors)
        {
            _name = name;
            _matchFilter = matchFilter;
            _selectors = selectors;
        }

    }
}
