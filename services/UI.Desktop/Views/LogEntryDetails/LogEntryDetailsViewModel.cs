using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Collections.ObjectModel;
using Core.BLL;
using UI.Desktop.Utils;
using Core.Entities;
using Core.DAL.Common;

namespace UI.Desktop.Views
{
	public class LogEntryDetailsViewModel : BaseWindowModel
	{
        private readonly LogEntry _model;
        public string DetailsInfo
        {
            get
            {
                return _model.Message + "\n" + _model.Details;
            }
        }

        public LogEntryDetailsViewModel(LogEntry model)
        {
            _model = model;
        }
	}
}
