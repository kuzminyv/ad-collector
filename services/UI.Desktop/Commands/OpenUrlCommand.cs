using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Desktop.Utils;

namespace UI.Desktop.Commands
{
	public class OpenUrlCommand : Command
	{
		public override void Execute(object parameter)
		{
			HyperlinkHelper.OpenHyperlink((string)parameter);
		}
	}
}
