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
        private readonly string _attributeName;
        private readonly bool _innerHtml;

        public override IEnumerable<Match> MatchAtom(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                HtmlNodeCollection nodes;
                if (_isHtmlFragment)
                {
                    nodes = HtmlNode.CreateNode(input). SelectNodes(_xPathExpression);
                }
                else
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(input);
                    nodes = doc.DocumentNode.SelectNodes(_xPathExpression);
                }

                if (nodes != null)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        string nodeValue = null;
                        if (!string.IsNullOrEmpty(_attributeName))
                        {
                            nodeValue = nodes[i].Attributes[_attributeName].Value;
                        }
                        else if (_innerHtml)
                        {
                            nodeValue = nodes[i].InnerHtml;
                        }
                        else
                        {
                            nodeValue = nodes[i].OuterHtml;
                        }
                        yield return new Match(this.Name, nodeValue);
                    }
                }
            }
        }

        public HtmlPathSelector(string name, string xPathExpression, bool isHtmlFragment, bool innerHtml, string attributeName, params Selector[] selectors)
            : base(name, selectors)
        {
            _xPathExpression = xPathExpression;
            _isHtmlFragment = isHtmlFragment;
            _innerHtml = innerHtml;
            _attributeName = attributeName;
        }

        public HtmlPathSelector(string name, string xPathExpression, bool isHtmlFragment, params Selector[] selectors)
            : this(name, xPathExpression, isHtmlFragment, false, null, selectors)
        { 
        }
    }
}
