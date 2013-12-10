using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace UI.Desktop.Views
{
	/// <summary>
	/// Interaction logic for LogView.xaml
	/// </summary>
	public partial class LogView : BaseWindow
	{
		public LogView()
		{
			InitializeComponent();
		}

        public LogView(LogViewModel model)
			:this()
		{
			DataContext = model;
		}
	}
}
