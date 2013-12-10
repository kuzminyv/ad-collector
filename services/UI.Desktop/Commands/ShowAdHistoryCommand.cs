using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
	public class ShowAdHistoryCommand : Command
	{
		public override void Execute(object parameter)
		{
            AdHistoryView view = new AdHistoryView(new AdHistoryViewModel((Ad)parameter));
            view.SetCurrentWindowAsOwner();
            view.ShowDialog();
		}
	}
}
