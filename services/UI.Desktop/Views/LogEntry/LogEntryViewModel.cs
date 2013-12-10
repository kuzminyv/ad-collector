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

        private ICommand _copyDetailsToClipboardCommand;
        public ICommand CopyDetailsToClipboardCommand
        {
            get
            {
                return _copyDetailsToClipboardCommand;
            }
        }



        public LogEntryViewModel(LogEntry model)
		{
			_model = model;

            _copyDetailsToClipboardCommand = new DelegateCommand(() => CopyDetailsToClipboard());
		}

        private void CopyDetailsToClipboard()
        {
            Clipboard.SetText(_model.Message + "\n" + _model.Details);
        }
	}
}
