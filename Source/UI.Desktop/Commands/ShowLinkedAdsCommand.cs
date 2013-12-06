using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
	public class ShowLinkedAdsCommand : Command
	{
		public override void Execute(object parameter)
		{
            LinkedAdsView view = new LinkedAdsView(new LinkedAdsViewModel((Ad)parameter));
            view.ShowDialog();
		}
	}
}
