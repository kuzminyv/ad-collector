using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
	public class ShowSettingsCommand : Command
	{
		public override void Execute(object parameter)
		{
            SettingsView view = new SettingsView(new SettingsViewModel());
            view.Owner = parameter as Window;
			view.ShowDialog();
		}
	}
}
