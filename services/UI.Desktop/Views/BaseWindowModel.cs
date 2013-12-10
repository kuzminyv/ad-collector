using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using UI.Desktop.Commands;
using System.Windows.Input;

namespace UI.Desktop.Views
{
	public abstract class BaseWindowModel : ViewModel, IWindowModel
	{
        private DelegateCommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand(new Action<object>(Save), new Func<object, bool>(CanSave));
                }
                return _saveCommand;
            }
        }

        private DelegateCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new DelegateCommand(new Action<object>(Cancel), p => true);
                }
                return _cancelCommand;
            }
        }

        protected virtual void Save(object parameter)
        {
        }

        protected virtual bool CanSave(object parameter)
        {
            return true;
        }

        protected virtual void Cancel(object parameter)
        {
            CloseWindow();
        }

		public EventHandler WindowCloseRequest
		{
			get;
			set;
		}

		public virtual bool WindowClosing()
		{
			return true;
		}

		public void CloseWindow()
		{
			OnWindowCloseRequest();
		}

		private void OnWindowCloseRequest()
		{
			if (WindowCloseRequest != null)
			{
				WindowCloseRequest(this, EventArgs.Empty);
			}
		}
	}
}
