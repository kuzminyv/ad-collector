using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Core.Utils;
using RGX = System.Text.RegularExpressions;

namespace Core.Expressions
{
    public class RegexSelector: Selector
    {
        private readonly string _regexExpression;
        public override IEnumerable<Match> MatchAtom(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                RGX.Regex rgx = new RGX.Regex(_regexExpression);
                var matches = rgx.Matches(input);

                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var group = match.Groups[this.Name];
                    if (group.Success)
                    {
                        yield return new Match(this.Name, group.Value);
                    }
                }
            }
        }

        public RegexSelector(string name, string regexExpression, params Selector[] selectors)
            : base(name, selectors)
        {
            _regexExpression = regexExpression;
        }
    }
}
