using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UI.Desktop.Commands
{
	public abstract class Command : ICommand
	{
		#region ICommand Members

		public event EventHandler CanExecuteChanged;

		public abstract void Execute(object parameter);

		public void Execute()
		{
			Execute(null);
		}

		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		protected void OnCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
			{ 
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		#endregion

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }
	}
}
