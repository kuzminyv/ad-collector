using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UI.Desktop.Commands
{
	public class ShutdownApplicationCommand : Command
	{
		public override void Execute(object parameter)
		{
			Application.Current.Shutdown();
		}
	}
}
