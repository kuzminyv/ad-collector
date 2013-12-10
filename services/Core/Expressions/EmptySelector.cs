using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Expressions
{
    public class EmptySelector : Selector
    {
        public override IEnumerable<Match> MatchAtom(string input)
        {
            yield return new Match(this.Name, input); 
        }

        public EmptySelector(string name, params Selector[] selectors)
            : base(name, selectors)
        { 
        }
    }
}
