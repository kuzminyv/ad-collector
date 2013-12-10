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

namespace UI.Studio.Views
{
	public class MainViewModel : ViewModel
	{
        private ObservableCollection<ConnectorViewModel> _connectors;
        public ObservableCollection<ConnectorViewModel> Connectors
        {
            get
            {
                if (_connectors == null)
                {
                    _connectors = new ObservableCollection<ConnectorViewModel>();
                }
                return _connectors;
            }
        }

        private ExplorerViewModel _explorer;
        public ExplorerViewModel Explorer
        {
            get
            {
                return _explorer;
            }
        }

        private ErrorsViewModel _errors;
        public ErrorsViewModel Errors
        {
            get
            {
                return _errors;
            }
        }

        private MatchListViewModel _matchList;
        public MatchListViewModel MatchList
        {
            get
            {
                return _matchList;
            }
        }

        public MainViewModel()
        {
            _explorer = new ExplorerViewModel(this);
            _errors = new ErrorsViewModel(this);
            _matchList = new MatchListViewModel(this);
        }
	}
}
