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
	public class ConnectorViewModel : ViewModel<object, MainViewModel>
	{
        public ConnectorView View
        {
            get;
            set;
        }

        private string _pathToConnectorSource;
        public string PathToConnectorSource
        {
            get
            {
                return _pathToConnectorSource;
            }
        }

        public string ContentId
        {
            get
            {
                return Path.GetFileNameWithoutExtension(PathToConnectorSource);
            }

        }

        public string DetailsPageFileName
        {
            get
            {
                return string.Format("{0}_details.txt", ContentId);
            }
        }

        public string ListPageFileName
        {
            get
            {
                return string.Format("{0}_list.txt", ContentId);
            }
        }

        public string LoadDetailsPage()
        {
            if (File.Exists(DetailsPageFileName))
            {
                return File.ReadAllText(DetailsPageFileName);
            }
            return String.Empty;
        }

        public string LoadListPage()
        {
            if (File.Exists(ListPageFileName))
            {
                return File.ReadAllText(ListPageFileName);
            }
            return String.Empty;
        }

        public string LoadConnectorSources()
        {
            return File.ReadAllText(PathToConnectorSource);
        }

        public void SaveDetailsPage(string contents)
        {
            File.WriteAllText(DetailsPageFileName, contents);
        }

        public void SaveListPage(string contents)
        {
            File.WriteAllText(ListPageFileName, contents);
        }

        public void SaveConnectorSources(string contents)
        {
            File.WriteAllText(PathToConnectorSource, contents);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand;
            }
        }

        public ConnectorViewModel(MainViewModel parent, string pathToConnectorSource)
            : base(parent)
        {
            _pathToConnectorSource = pathToConnectorSource;
            _closeCommand = new DelegateCommand(Close);
        }

        private void Close()
        {
            Parent.Connectors.Remove(this);
        }
	}
}
