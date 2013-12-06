using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
	public class ShowAdListCommand : Command
	{
        private AdListView _view;
		public override void Execute(object parameter)
		{
            if (_view == null)
            {
                _view = new AdListView(new AdListViewModel());
                _view.Closed += view_Closed;
                _view.Show();
            }
            else
            {
                _view.Activate();
            }
		}

        private void view_Closed(object sender, EventArgs e)
        {
            _view = null;
        }
	}
}
