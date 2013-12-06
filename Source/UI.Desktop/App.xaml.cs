using Core.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Core.Utils;

namespace UI.Desktop
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private AppContext _appContext;
        public static AppContext AppContext
        {
            get
            {
                return ((App)App.Current)._appContext;
            }
        }

		protected override void OnStartup(StartupEventArgs e)
		{
            _appContext = new AppContext();
			MainWindow w = new MainWindow();
			base.OnStartup(e);
		}

        protected override void OnExit(ExitEventArgs e)
        {
            BufferedAction.WaitAll();
            base.OnExit(e);
        }
	}
}
