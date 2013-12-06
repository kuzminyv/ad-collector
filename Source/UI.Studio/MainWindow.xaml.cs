using Core.Connectors;
using Core.Entities;
using Core.Expressions;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Desktop.Views;
using UI.Studio.Views;
using Xceed.Wpf.AvalonDock.Layout;
using UI.Desktop.Utils;

namespace UI.Studio
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectorPath = @"..\..\..\Core\Connectors\Realty\CnIrr.cs";
        private AsyncOperation<object, object> _runConnectorOperation;

        private class OperationOptions
        { 
            public bool IsDetailsPage;
            public bool FillEntity;
            public bool ThrowError;
        }


        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("studio.txt"))
            {
                sourceText.Text = File.ReadAllText("studio.txt");
            }
            connectorDefinition.Text = File.ReadAllText(connectorPath);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(sourceText.Text))
            {
                File.WriteAllText("studio.txt", sourceText.Text);
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
            ClearErrors();
            try
            {
                string text = sourceText.Text;
                BasicConnector connector = (new RuntimeCompiler()).CreateInstance<BasicConnector>(connectorDefinition.Text);
                var selector = options.IsDetailsPage ? connector.CreateDetailsSelector() : connector.CreateSelector();

                var model = new MatchListViewModel();
                model.Items = new ObservableCollection<MatchItemViewModel>(
                    selector.Match(text).SelectMany(m => FlatMatch(m))
                                        .Select(m => new MatchItemViewModel(m)));
                resultView.DataContext = model;

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
                AddError(ex.Message);
                AddError(ex.StackTrace);
                if (options.ThrowError)
                {
                    throw;
                }
            }
            return null;
        }

        public IEnumerable<Match> FlatMatch(Match match)
        {
            if (match.Count() > 0)
            {
                foreach (var child in match)
                {
                    foreach (var m in FlatMatch(child))
                    {
                        yield return m;
                    }
                }
            }
            else
            {
                yield return match;
            }
        }

        private void ClearErrors()
        {
            txtErrors.Text = string.Empty;
        }

        private void AddError(string error)
        {
            txtErrors.Text += error + "\n";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(connectorPath, connectorDefinition.Text);
        }
    }
}
