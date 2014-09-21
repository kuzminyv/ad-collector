using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
    public class ShowLogEntryDetailsCommand : Command
	{
		public override void Execute(object parameter)
		{
            LogEntryDetailsView view = new LogEntryDetailsView(new LogEntryDetailsViewModel((LogEntry)parameter));
            view.Owner = parameter as Window;
			view.ShowDialog();
		}
	}
}
