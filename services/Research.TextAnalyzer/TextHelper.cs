using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research.TextAnalyzer
{
    public class TextHelper
    {
        public static string[] GetWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new string[0];
            }

            return text.Split(new char[] { ' ', '.', ',', '\"', '\'' }, StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 1).ToArray();
        }
    }
}
