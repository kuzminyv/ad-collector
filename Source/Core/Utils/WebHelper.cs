using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Core.Utils
{
	public class WebHelper
	{
		private string _sourceUrl;
		private string _htmlContent;


		/// <summary>
		/// returns a name/value collection of all external files referenced in HTML:
		/// 
		/// note that the Key includes the delimiting quotes or parens (if present), but the Value does not
		/// this is important because the delimiters are used for matching and replacement to make the
		/// match more specific!
		/// </summary>
		private NameValueCollection ExternalHtmlFiles()
		{
			NameValueCollection result = new NameValueCollection();

			string expr;
			expr = "(\\ssrc|\\sbackground)\\s*=\\s*((?<Key>'(?<Value>[^']+)')|(?<Key>\"(?<Value>[^\")]+)\")|(?<Key>(?<Value>[^ \\n\\r\\f]+)))";
			Regex r = new Regex(expr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			AddMatchesToCollection(_htmlContent, r, result);

			expr = "(@import\\s|\\S+-image:|background:)\\s*?(url)*\\s*?(?<Key>[\"'(]{1,2}(?<Value>[^\"')]+)[\"')]{1,2})";
			r = new Regex(expr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			AddMatchesToCollection(_htmlContent, r, result);

			expr = "<link[^>]+?href\\s*=\\s*(?<Key>('|\")*(?<Value>[^\">]+)('|\")*)";
			r = new Regex(expr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			AddMatchesToCollection(_htmlContent, r, result);

			expr = "<i*frame[^>]+?src\\s*=\\s*(?<Key>['\"]{0,1}(?<Value>[^\"\\\\>]+)['\"]{0,1})";
			r = new Regex(expr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			AddMatchesToCollection(_htmlContent, r, result);

			return result;
		}

		/// <summary>
		/// Return full local path to file if initialized m_sourceFile, 
		/// else if initialized m_sourceUrl return absolete URL to web resource.
		/// If m_sourceFile and m_sourceUrl not initialized return emty string
		/// </summary>
		private string GetAbsoleteUriByRelativ(string relativUri)
		{
			Uri baseUri = new Uri(_sourceUrl);
			Uri uri = new Uri(baseUri, relativUri);
			return uri.AbsoluteUri;
		}

		/// <summary>
		/// appends key=value named matches in a regular expression
		/// to a target NameValueCollection
		/// </summary>
		private void AddMatchesToCollection(string s, Regex r, NameValueCollection nvc)
		{
			Regex regex = new Regex(@"(^file:///{0,1}\w+)|(^https*://\w+)", RegexOptions.IgnoreCase);
			var matches = r.Matches(s);
			foreach (Match current in matches)
			{
				string name = current.Groups["Key"].ToString();
				string input = current.Groups["Value"].ToString();
				if ((nvc[name] == null) && ((File.Exists(input) && (Path.IsPathRooted(input))) || regex.IsMatch(input)))
				{
					nvc.Add(name, input);
				}
				else
				{
					string absoleteUri = GetAbsoleteUriByRelativ(input);
					if (nvc[name] == null && (File.Exists(absoleteUri) || regex.IsMatch(absoleteUri)))
					{
						nvc.Add(name, absoleteUri);
					}
				}
			}
		}

		/// <summary>
		/// Use it instead of Uri.LocalPath, because
		/// Uri.LocalPath not correct work with '#' char
		/// </summary>
		private string ConvertToLocal(Uri uri)
		{
			string result = String.Empty;
			string original = uri.OriginalString;

			string protocolStrA = "file:///";
			string protocolStrB = "file://";
			if (original.StartsWith(protocolStrA) && original.Length > protocolStrA.Length)
			{
				result = original.Substring(protocolStrA.Length).Replace('/', '\\');
				result = Uri.UnescapeDataString(result);
			}
			else
				if (original.StartsWith(protocolStrB) && original.Length > protocolStrB.Length)
				{
					result = original.Substring(protocolStrB.Length).Replace('/', '\\');
					result = Uri.UnescapeDataString(result);
				}
				else
				{
					result = uri.LocalPath;
				}
			return result;
		}

		/// <summary>
		/// detect file extension by
		/// content type header retrived with http response
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		private string GetExtensionFromContentType(string contentType)
		{
			switch (Regex.Match(contentType, "^[^ ;]+").Value.ToLower())
			{
				case "text/html":
					return ".htm";

				case "image/gif":
					return ".gif";

				case "image/jpeg":
					return ".jpg";

				case "text/javascript":
				case "application/x-javascript":
					return ".js";

				case "image/x-png":
					return ".png";

				case "text/css":
					return ".css";

				case "text/plain":
					return ".txt";
			}
			return String.Empty;
		}


		private Encoding CharsetToEncoding(string charset)
		{
			if (String.IsNullOrEmpty(charset))
			{
				return null;
			}
			try
			{
				return Encoding.GetEncoding(charset);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		/// <summary>
		/// try to determine string encoding using Content-Type HTTP header,
		/// if encoding not detected then return null
		/// </summary>
		private Encoding DetectEncoding(string contentType)
		{
			if (String.IsNullOrEmpty(contentType))
			{
				return null;
			}

			string charset = Regex.Match(contentType,
				"charset=([^;\"'/>]+)", RegexOptions.IgnoreCase).Groups[1].ToString().ToLower();
			Encoding encoding = CharsetToEncoding(charset);
			return encoding;
		}

		/// <summary>
		/// try to determine string encoding using raw bytes,
		/// if encoding not detected then return Encoding.Default
		/// </summary>
		private Encoding DetectEncoding(byte[] htmlContent)
		{
			string htmlString = Encoding.ASCII.GetString(htmlContent);
			string charset = Regex.Match(htmlString,
				"<meta[^>]+content-type[^>]+charset=([^;\"'/>]+)",
				RegexOptions.IgnoreCase).Groups[1].ToString().ToLower();
			Encoding encoding = CharsetToEncoding(charset);
			if (encoding == null)
			{
				return Encoding.Default;
			}
			return encoding;
		}

		private byte[] ReadAllBytesFromUrl(Uri url, out string contentType)
		{
			WebClient client = new WebClient();
			client.Credentials = CredentialCache.DefaultCredentials;
			using (Stream stream = client.OpenRead(url))
			{
				BinaryReaderEx reader = new BinaryReaderEx(stream);
				byte[] data = reader.ReadToEnd();
				contentType = client.ResponseHeaders["Content-Type"];
				return data;
			}
		}


		private WebHelper(string sourceUrl)
		{
			_sourceUrl = sourceUrl;
		}

		private void ImportFromWeb()
		{
			byte[] data = null;
			Encoding encoding = null;

			string contentType;
			Uri url = new Uri(_sourceUrl);
			data = ReadAllBytesFromUrl(url, out contentType);

			encoding = DetectEncoding(contentType);
			if (encoding == null)
			{
				encoding = DetectEncoding(data);
			}
			_htmlContent = encoding.GetString(data);
		}

		public static string GetStringFromUrl(string url)
		{
			var h = new WebHelper(url);
			h.ImportFromWeb();
			return h._htmlContent;
		}

		public static Image GetImageFromUrl(string url)
		{
			var h = new WebHelper(url);
			string contentType;
			byte[] data = h.ReadAllBytesFromUrl(new Uri(url), out contentType);
			return Bitmap.FromStream(new MemoryStream(data));
		}
	}
}
