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
	/// Interaction logic for LogEntryDetailsView.xaml
	/// </summary>
	public partial class LogEntryDetailsView : BaseWindow
	{
		public LogEntryDetailsView()
		{
			InitializeComponent();
		}

        public LogEntryDetailsView(LogEntryDetailsViewModel model)
			:this()
		{
			DataContext = model;
		}
	}
}
