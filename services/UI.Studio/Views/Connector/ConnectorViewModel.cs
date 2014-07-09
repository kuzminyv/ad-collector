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
using Core.Connectors;
using Core.Utils;
using Core.Expressions;
using System.Reflection;

namespace UI.Studio.Views
{
	public class ConnectorViewModel : ViewModel<object, MainViewModel>
    {
        private class OperationOptions
        {
            public bool IsDetailsPage;
            public bool ThrowOnError;
            public bool MatchOnly;
            public string ConnectorSource;
            public string PageSource;
        }

        #region Properties
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

        private bool _isDetailsPage;
        public bool IsDetailsPage
        {
            get
            {
                return _isDetailsPage;
            }
            set
            {
                if (_isDetailsPage != value)
                {
                    SavePage();
                    _isDetailsPage = value;
                    OnPropertyChanged("IsDetailsPage");
                    LoadPage();
                }
            }
        }

        private bool _throwOnError;
        public bool ThrowOnError
        {
            get
            {
                return _throwOnError;
            }
            set
            {
                if (_throwOnError != value)
                {
                    _throwOnError = value;
                    OnPropertyChanged("ThrowOnError");
                }
            }
        }

        private bool _debug;
        public bool Debug
        {
            get
            {
                return _debug;
            }
            set
            {
                if (_debug != value)
                {
                    _debug = value;
                    OnPropertyChanged("Debug");
                }
            }
        }

        private bool _matchOnly;
        public bool MatchOnly
        {
            get
            {
                return _matchOnly;
            }
            set
            {
                if (_matchOnly != value)
                {
                    _matchOnly = value;
                    OnPropertyChanged("MatchOnly");
                }
            }
        }

        #endregion

        #region Commands

        private ICommand _loadConnectorSourcesCommand;
        public ICommand LoadConnectorSourcesCommand
        {
            get
            {
                return _loadConnectorSourcesCommand;
            }
        }


        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand;
            }
        }

        private ICommand _runConnectorCommand;
        public ICommand RunConnectorCommand
        {
            get
            {
                return _runConnectorCommand;
            }
        }

        private ICommand _saveConnectorSourcesCommand;
        public ICommand SaveConnectorSourcesCommand
        {
            get
            {
                return _saveConnectorSourcesCommand;
            }
        }
        #endregion

        public ConnectorViewModel(MainViewModel parent, string pathToConnectorSource)
            : base(parent)
        {
            _pathToConnectorSource = pathToConnectorSource;
            _closeCommand = new DelegateCommand(Close);
            _runConnectorCommand = new DelegateCommand(o => RunConnector(GetOperationOptions(), new CancelationToken()));
            _saveConnectorSourcesCommand = new DelegateCommand(SaveConnectorSources);
            _loadConnectorSourcesCommand = new DelegateCommand(LoadConnectorSources);
        }

        private void Close()
        {
            Parent.Connectors.Remove(this);
        }

        private OperationOptions GetOperationOptions()
        {
            return new OperationOptions()
            {
                ConnectorSource = View.editorConnectorSource.Text,
                MatchOnly = MatchOnly,
                IsDetailsPage = IsDetailsPage,
                PageSource = View.editorPageSource.Text,
                ThrowOnError = ThrowOnError
            };
        }

        public void LoadPage()
        {
            string fileName = IsDetailsPage ? DetailsPageFileName : ListPageFileName;
            if (File.Exists(fileName))
            {
                View.editorPageSource.Text = File.ReadAllText(fileName);
            }
            else
            {
                View.editorPageSource.Text = String.Empty;
            }
        }

        public void LoadConnectorSources()
        {
            View.editorConnectorSource.Text = File.ReadAllText(PathToConnectorSource);
        }

        public void SavePage()
        {
            string fileName = IsDetailsPage ? DetailsPageFileName : ListPageFileName;
            File.WriteAllText(fileName, View.editorPageSource.Text);
        }

        public void SaveConnectorSources()
        {
            File.WriteAllText(PathToConnectorSource, View.editorConnectorSource.Text);
        }

        private object RunConnector(OperationOptions options, CancelationToken cancelationToken)
        {
            Parent.Errors.ClearErrors();
            if (options.ThrowOnError)
            {
                return RunConnectorUnsafe(options, cancelationToken);
            }
            try 
            {
                return RunConnectorUnsafe(options, cancelationToken);
            }
            catch (Exception ex)
            {
                Parent.Errors.AddError(ex.Message);
                Parent.Errors.AddError(ex.StackTrace);
            }
            return null;
        }

        private object RunConnectorUnsafe(OperationOptions options, CancelationToken cancelationToken)
        {
            BasicConnector connector;
            try
            {
                if (this.Debug)
                {

                    connector = (BasicConnector)Activator.CreateInstance(Assembly.GetAssembly(typeof(BasicConnector)).ToString(), "Core.Connectors." + ContentId).Unwrap();
                }
                else
                {
                    connector = (new RuntimeCompiler()).CreateInstance<BasicConnector>(options.ConnectorSource);
                }
            }
            catch (Exception exCompile)
            {
                Parent.Errors.AddError(exCompile.Message);
                Parent.Errors.AddError(exCompile.StackTrace);
                return null;
            }

            var selector = options.IsDetailsPage ? connector.CreateDetailsSelector() : connector.CreateSelector();

            var mmm = selector.Match(options.PageSource);

            Parent.MatchList.Items = new ObservableCollection<MatchItemViewModel>(
                selector.Match(options.PageSource).SelectMany(m => Match.Flat(m))
                                    .Select(m => new MatchItemViewModel(m)));

            if (!options.MatchOnly)
            {
                FillAdsFromMatches(connector, selector.Match(options.PageSource), options.IsDetailsPage);
            }
            return null;
        }

        private void FillAdsFromMatches(BasicConnector connector, IEnumerable<Match> matches, bool isDetailsPage)
        {
            var ads = new List<Ad>();
            if (isDetailsPage)
            {
                foreach (var match in matches)
                {
                    var ad = new AdRealty();
                    connector.FillAdDetails(ad, match);
                    ads.Add(ad);
                }
            }
            else
            {
                
                foreach (var match in matches)
                {
                    ads.Add(connector.CreateAd(match));
                }
            }

            foreach (var ad in ads)
            {
                if (ad != null)
                {
                    Parent.Errors.AddError("\n=============================\n" + ad.ToString());
                    IVerificator verificator = new RealtyVerificator();
                    if (verificator.Verify(ad) != null)
                    {
                        Parent.Errors.AddError(verificator.Verify(ad));
                    }
                }
            }
        }
	}
}
