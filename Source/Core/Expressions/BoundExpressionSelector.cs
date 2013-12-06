using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Expressions
{
    public class BoundExpressionSelector : Selector
    {
        private readonly BoundExpression _expression;

        public override IEnumerable<Match> MatchAtom(string input)
        {
            var boundMatches = _expression.Matches(input);
            var result = new List<Match>(boundMatches.Count);
            foreach (var boundMatch in boundMatches)
            {
                var match = new Match(this.Name, "");
                result.Add(match);
                foreach (var matchItem in boundMatch)
                {
                    match.AddMatch(new Match(matchItem.Key, matchItem.Value));
                }
            }
            return result;
        }

        public override IEnumerable<Match> Match(string input)
        {
            var thisMatch = MatchAtom(input);
            foreach (Match match in thisMatch)
            {
                foreach (var matchItem in match)
                {                    
                    foreach (var selector in Selectors)
                    {
                        var childMatch = selector.Match(matchItem.Value);
                        matchItem.AddMatchRange(childMatch);
                    }
                }
            }
            return thisMatch;
        }

        public BoundExpressionSelector(string name, BoundExpression expression, params Selector[] selectors)
            : base(name, selectors)
        {
            _expression = expression;
        }
    }
}
