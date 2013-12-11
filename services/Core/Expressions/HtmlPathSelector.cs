using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Core.Utils;
using HtmlAgilityPack;

namespace Core.Expressions
{
    public class HtmlPathSelector: Selector
    {
        private readonly bool _isHtmlFragment;
        private readonly string _xPathExpression;
        public override IEnumerable<Match> MatchAtom(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                HtmlNodeCollection nodes;
                if (_isHtmlFragment)
                {
                    nodes = HtmlNode.CreateNode(input).SelectNodes(_xPathExpression);
                }
                else
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(input);
                    nodes = doc.DocumentNode.SelectNodes(input);
                }
                for (int i = 0; i < nodes.Count; i++)
                {
                    yield return new Match(this.Name, nodes[i].InnerText);
                }
            }
        }

        public HtmlPathSelector(string name, string xPathExpression, bool isHtmlFragment, params Selector[] selectors)
            : base(name, selectors)
        {
            _xPathExpression = xPathExpression;
            _isHtmlFragment = isHtmlFragment; 
        }
    }
}
