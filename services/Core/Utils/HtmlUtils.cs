using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Utils
{
	public static class HtmlUtils
	{
        public static string RemoveScripts(this string htmlCode)
        {
            if (string.IsNullOrEmpty(htmlCode))
            {
                return string.Empty;
            }
            return Regex.Replace(htmlCode, "<script.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

		// This function converts HTML code to plain text
		// Any step is commented to explain it better
		// You can change or remove unnecessary parts to suite your needs
		public static string HTMLToText(this string htmlCode)
		{
			if (string.IsNullOrEmpty(htmlCode))
			{
				return string.Empty;
			}

			// Remove new lines since they are not visible in HTML
			htmlCode = htmlCode.Replace("\n", " ");

			// Remove tab spaces
			htmlCode = htmlCode.Replace("\t", " ");

			// Remove multiple white spaces from HTML
			htmlCode = Regex.Replace(htmlCode, "\\s+", " ");

			// Remove HEAD tag
			htmlCode = Regex.Replace(htmlCode, "<head.*?</head>", ""
								, RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Remove any JavaScript
			htmlCode = Regex.Replace(htmlCode, "<script.*?</script>", ""
			  , RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Replace special characters like &, <, >, " etc.
			StringBuilder sbHTML = new StringBuilder(htmlCode);
			// Note: There are many more special characters, these are just
			// most common. You can add new characters in this arrays if needed
			string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;", 
   "&gt;", "&reg;", "&copy;", "&bull;", "&trade;"};
			string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢" };
			for (int i = 0; i < OldWords.Length; i++)
			{
				sbHTML.Replace(OldWords[i], NewWords[i]);
			}

			// Check if there are line breaks (<br>) or paragraph (<p>)
			sbHTML.Replace("<br>", "\n<br>");
			sbHTML.Replace("<br ", "\n<br ");
			sbHTML.Replace("<p ", "\n<p ");

			// Finally, remove all HTML tags and return plain text
			return System.Text.RegularExpressions.Regex.Replace(
			  sbHTML.ToString(), "<[^>]*>", "");
		}

		public static string ToText(this Match match, string groupName)
		{
			return match.Groups[groupName].Value.HTMLToText();
		}

	}
}
