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
            this.Unloaded += ConnectorView_Unloaded;
		}

        private void ConnectorView_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SavePage();
        }

        private void ConnectorView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.View = this;
            ViewModel.LoadConnectorSources();
            ViewModel.LoadPage();
        }
    }
}
