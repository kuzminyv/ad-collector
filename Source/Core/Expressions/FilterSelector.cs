using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utils;

namespace Core.Expressions
{
    public class FilterSelector : Selector
    {
        private Func<string, string> _filterFn;

        public override IEnumerable<Match> MatchAtom(string input)
        {
            return Selectors.First().Match(_filterFn(input));
        }

        public override IEnumerable<Match> Match(string input)
        {
            return MatchAtom(input);
        }

        public override IEnumerable<Match> Match(Match match)
        {
            if (string.IsNullOrEmpty(MatchFilter) || MatchFilter == match.Name)
            {
                return Match(match.Value);
            }
            return new Match[0];
        }

        public FilterSelector(string name, Selector selector, Func<string, string> filterFn)
            : base(name, selector)
        {
            _filterFn = filterFn;
        }

        public static Selector RemoveScripts(Selector selector)
        {
            return new FilterSelector("removeScripts", selector, new Func<string, string>(s => s.RemoveScripts())); 
        }

        public static Selector HtmlToText(Selector selector)
        {
            return new FilterSelector("removeHtml", selector, new Func<string, string>(s => s.HTMLToText()));
        }
    }
}
