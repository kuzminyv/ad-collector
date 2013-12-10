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
	public class ExplorerViewModel : ViewModel<object, MainViewModel>
	{
        private string _pathToConnectors;

        private ObservableCollection<FolderViewModel> _folders;
        public ObservableCollection<FolderViewModel> Folders
        {
            get
            {
                if (_folders == null)
                {
                    string[] folders = Directory.GetDirectories(_pathToConnectors);
                    _folders = new ObservableCollection<FolderViewModel>(folders.Select(f => new FolderViewModel(f)));
                }
                return _folders;
            }
        }

        private ICommand _openConnectorCommand;
        public ICommand OpenConnectorCommand
        {
            get
            {
                return _openConnectorCommand;
            }
        }

        public ExplorerViewModel(MainViewModel parent)
            : base(parent)
        {
            _pathToConnectors = @"..\..\..\Core\Connectors";
            _openConnectorCommand = new DelegateCommand(OpenConnector);
        }

        public void OpenConnector(object item)
        {
            if (item is FileViewModel)
            {
                ConnectorViewModel connector = new ConnectorViewModel(Parent, ((FileViewModel)item).FilePath);
                Parent.Connectors.Add(connector);
            }
        }
	}
}
