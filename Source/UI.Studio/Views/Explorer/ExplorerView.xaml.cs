using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace UI.Studio.Views
{
	/// <summary>
    /// Interaction logic for ExplorerView.xaml
	/// </summary>
	public partial class ExplorerView : UserControl
	{
        public ExplorerViewModel ViewModel
        {
            get
            {
                return this.DataContext as ExplorerViewModel;
            }
        }

        public ExplorerView()
        {
            InitializeComponent();
		}

        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OpenConnectorCommand.Execute(((TreeViewItem)sender).DataContext);
        }
	}
}
