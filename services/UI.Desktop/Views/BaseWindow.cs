using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace UI.Desktop.Views
{
	public class BaseWindow : Window
	{
        private static BaseWindow _lastActiveWindow;
		public BaseWindow()
		{
            Activated += BaseWindow_Activated;
			DataContextChanged += new DependencyPropertyChangedEventHandler(BaseWindow_DataContextChanged);
			Closing += BaseWindow_Closing;
		}

        private void BaseWindow_Activated(object sender, EventArgs e)
        {
            _lastActiveWindow = this;
        }

		private void BaseWindow_Closing(object sender, CancelEventArgs e)
		{
			IWindowModel model = DataContext as IWindowModel;
			if (model != null)
			{
				e.Cancel = !model.WindowClosing();
			}
		}

		private void BaseWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			IWindowModel oldModel = e.OldValue as IWindowModel;
			if (oldModel != null)
			{
				oldModel.WindowCloseRequest = null;
			}

			IWindowModel newModel = e.NewValue as IWindowModel;
			if (newModel != null)
			{
				newModel.WindowCloseRequest = new EventHandler(ProcessWindowCloseRequest);
			}
		}

        public void SetCurrentWindowAsOwner()
        {
            this.Owner = _lastActiveWindow;
        }

		private void ProcessWindowCloseRequest(object sender, EventArgs e)
		{
			Close();
		}
	}
}
