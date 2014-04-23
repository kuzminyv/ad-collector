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
            Console.WriteLine("Reading...");
            Query query = new Query(0, 10000);
            var result = Repositories.AdsRepository.GetList(query);
            var uniqueWords = new Dictionary<string, int>();

            Console.WriteLine("Analyzing...");
            foreach (var ad in result.Items)
            {
                AdRealty adRealty = (AdRealty)ad;
                var words = GetWordsAfter(ad.Description, "ЖСК");
                foreach (var word in words)
                {
                    var upperWord = word.ToUpper();
                    if (uniqueWords.ContainsKey(upperWord))
                    {
                        uniqueWords[upperWord]++;
                    }
                    else
                    {
                        uniqueWords.Add(upperWord, 1);
                    }
                }
            }

            foreach (var word in uniqueWords.OrderByDescending(kvp => kvp.Value))
            {
                Console.WriteLine("{0} - {1}", word.Value, word.Key);
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        public static string[] GetWordsAfter(string text, string wordBefore)
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
