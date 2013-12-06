using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Core.Utils;

namespace Core.Expressions
{
    public class XPathSelector: Selector
    {
        private readonly string _xPathExpression;
        public override IEnumerable<Match> MatchAtom(string input)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(new StringReader(input));
            var nodes = xDoc.SelectNodes(_xPathExpression);
            for (int i = 0; i < nodes.Count; i++)
            {
                yield return new Match(this.Name, nodes.Item(i).InnerText);
            }
        }

        public XPathSelector(string name, string xPathExpression, params Selector[] selectors)
            : base(name, selectors)
        {
            _xPathExpression = xPathExpression;
        }
    }
}
