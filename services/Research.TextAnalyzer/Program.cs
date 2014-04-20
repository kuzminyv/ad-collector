using Core.BLL;
using Core.DAL;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research.TextAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Query query = new Query(0, 10000);
            var result = Repositories.AdsRepository.GetList(query);
            var uniqueWords = new HashSet<string>();

            foreach (var ad in result.Items)
            {
                AdRealty adRealty = (AdRealty)ad;
                uniqueWords.UnionWith(GetWordAfter(ad.Description, "ЖК"));
            }

            foreach (var word in uniqueWords)
            {
                Console.WriteLine(word);
            }
            Console.ReadKey();
        }

        public static string[] GetWordAfter(string text, string wordBefore)
        {
            var words = TextHelper.GetWords(text);
            var result = new List<string>();
            for (int i = 1; i < words.Length; i++)
            {
                if (string.Compare(words[i - 1], wordBefore, true) == 0)
                {
                    result.Add(words[i]);
                }
            }

            return result.ToArray();
        }


    }
}
