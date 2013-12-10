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
            if (!string.IsNullOrEmpty(input))
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
            return new Match[0];
        }

        public override IEnumerable<Match> Match(Match match)
        {
            if (!string.IsNullOrEmpty(match.Value) && (string.IsNullOrEmpty(MatchFilter) || match.Name == MatchFilter))
            {
                var thisMatch = MatchAtom(match.Value);
                foreach (Match m in thisMatch)
                {
                    foreach (var matchItem in m)
                    {
                        foreach (var selector in Selectors)
                        {
                            var childMatch = selector.Match(matchItem);
                            matchItem.AddMatchRange(childMatch);
                        }
                    }
                }
                return thisMatch;
            }
            return new Match[0];
        }

        public BoundExpressionSelector(string name, string matchFilter, BoundExpression expression, params Selector[] selectors)
            : base(name, matchFilter, selectors)
        {
            _expression = expression;
        }

        public BoundExpressionSelector(string name, BoundExpression expression, params Selector[] selectors)
            : this(name, null, expression, selectors)
        { 
        }
    }
}
