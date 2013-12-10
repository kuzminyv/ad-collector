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
using UI.Desktop.Views;
using System.IO;

namespace UI.Studio.Views
{
	public class ErrorsViewModel : ViewModel<object, MainViewModel>
	{
        private string _errors;
        public string Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                if (_errors != value)
                {
                    _errors = value;
                    OnPropertyChanged("Errors");
                }
            }
        }

        public void ClearErrors()
        {
            Errors = string.Empty;
        }

        public void AddError(string error)
        {
            Errors += error + "\n";
        }

        public ErrorsViewModel(MainViewModel parent)
            : base(parent)
        {
        }
	}
}
