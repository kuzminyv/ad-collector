using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Web;
using System.Windows;

namespace UI.Desktop.Views
{
	public class LogEntryViewModel : ViewModel
	{
		private LogEntry _model;

        public DateTime Time
        {
            get
            {
                return _model.Time;
            }
        }

        public SeverityLevel Severity
        {
            get
            {
                return _model.Severity;
            }
        }

        public string Message
        {
            get
            {
                return _model.Message;
            }
        }

        private ICommand _viewDetailsCommand;
        public ICommand ViewDetailsCommand
        {
            get
            {
                return _viewDetailsCommand;
            }
        }



        public LogEntryViewModel(LogEntry model)
		{
			_model = model;

            _viewDetailsCommand = new DelegateCommand(() => ViewDetails());
		}

        private void ViewDetails()
        {
            AppCommands.ShowLogEntryDetailsCommand.Execute(_model);
        }
	}
}
