using Core.DAL;
using Core.Entities;
using Core.Utils;
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
            Ad ad = Repositories.AdsRepository.GetItem(259818);
            WebClientResult content = WebHelper.GetStringFromUrl("http://realty.sarbc.ru/board/350002.html", new WebClientOptions() { IgnoreError404 = true });
            Console.WriteLine(content.Content);
            Console.ReadLine();
            //DynLinqTest.Run();
        }
    }
}
