using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UI.Desktop.Utils
{
	public class HyperlinkHelper
	{
		public static void OpenHyperlink(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch (Win32Exception)
			{
			}
		}
	}
}
