using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlDocument d = new HtmlDocument();
            HtmlNode node = HtmlNode.CreateNode("<div><span>abc</span></div>");



            DynLinqTest.Run();
        }
    }
}
