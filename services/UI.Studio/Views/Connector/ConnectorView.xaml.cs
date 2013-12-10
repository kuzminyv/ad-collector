using Core.Connectors;
using Core.Entities;
using Core.Expressions;
using Core.Utils;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Desktop.Utils;

namespace UI.Studio.Views
{
	/// <summary>
    /// Interaction logic for ConnectorView.xaml
	/// </summary>
	public partial class ConnectorView : UserControl
	{
        private class OperationOptions
        {
            public bool IsDetailsPage;
            public bool FillEntity;
            public bool ThrowError;
        }

        public ConnectorViewModel ViewModel
        {
            get
            {
                return this.DataContext as ConnectorViewModel;
            }
        }

        public ConnectorView()
        {
            InitializeComponent();
            this.Loaded += ConnectorView_Loaded;
		}

        private void ConnectorView_Loaded(object sender, RoutedEventArgs e)
        {
            editorConnectorSource.Text = ViewModel.LoadConnectorSources();

            if (chkDetails.IsChecked == true)
            {
                editorPageSource.Text = ViewModel.LoadDetailsPage();
            }
            else
            {
                editorPageSource.Text = ViewModel.LoadListPage();
            }
        }



        private void btnMatch_Click(object sender, RoutedEventArgs e)
        {
            RunConnector(new OperationOptions()
            {
                IsDetailsPage = (chkDetails.IsChecked == true),
                ThrowError = (chkThrowError.IsChecked == true),
                FillEntity = false
            }, new CancelationToken());
        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            RunConnector(new OperationOptions()
            {
                IsDetailsPage = (chkDetails.IsChecked == true),
                ThrowError = (chkThrowError.IsChecked == true),
                FillEntity = true
            }, new CancelationToken());
        }

        private object RunConnector(OperationOptions options, CancelationToken cancelationToken)
        {
            ViewModel.Parent.Errors.ClearErrors();
            try
            {
                string text = editorPageSource.Text;
                BasicConnector connector;

                try
                {
                    connector = (new RuntimeCompiler()).CreateInstance<BasicConnector>(editorConnectorSource.Text);
                }
                catch (Exception exCompile)
                {
                    ViewModel.Parent.Errors.AddError(exCompile.Message);
                    ViewModel.Parent.Errors.AddError(exCompile.StackTrace);
                    return null;
                }

                var selector = options.IsDetailsPage ? connector.CreateDetailsSelector() : connector.CreateSelector();

                ViewModel.Parent.MatchList.Items = new ObservableCollection<MatchItemViewModel>(
                    selector.Match(text).SelectMany(m => Match.Flat(m))
                                        .Select(m => new MatchItemViewModel(m)));

                if (options.FillEntity)
                {
                    if (options.IsDetailsPage)
                    {
                        foreach (var match in selector.Match(text))
                        {
                            connector.FillDetails(new AdRealty());
                        }
                    }
                    else
                    {
                        var ads = new List<Ad>();
                        foreach (var match in selector.Match(text))
                        {
                            ads.Add(connector.CreateAd(match));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewModel.Parent.Errors.AddError(ex.Message);
                ViewModel.Parent.Errors.AddError(ex.StackTrace);
                if (options.ThrowError)
                {
                    throw;
                }
            }
            return null;
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(ViewModel.PathToConnectorSource, editorConnectorSource.Text);
        }
    }
}
