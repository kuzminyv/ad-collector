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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Desktop.Views;
using System.Windows.Controls.Primitives;
using UI.Desktop.Utils;
using System.IO;
using System.Windows.Threading;

namespace UI.Desktop
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private System.Drawing.Icon[] _downloadIcons;
        private System.Drawing.Icon _downloadCompletedIcon;

        private DispatcherTimer _iconAnimationTimer;
        private int _downloadIconIndex;

		public MainWindow()
		{
            _downloadIcons = new System.Drawing.Icon[] 
            {
                ResourceHelper.GetIcon("/Images/news0.ico"),
                ResourceHelper.GetIcon("/Images/news1.ico"),
                ResourceHelper.GetIcon("/Images/news2.ico"),
            };
            _downloadCompletedIcon = ResourceHelper.GetIcon("/Images/Ready.ico");

            _iconAnimationTimer = new DispatcherTimer();
            _iconAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _iconAnimationTimer.IsEnabled = false;
            _iconAnimationTimer.Tick += _iconAnimationTimer_Tick;

            App.AppContext.CheckForNewAdsStart += AppContext_CheckForNewAdsStart;
            App.AppContext.CheckForNewAdsComplete += AppContext_CheckForNewAdsComplete;


			InitializeComponent();
		}

		private void MyNotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
		{
            MainView view = new MainView(new MainViewModel());
			MyNotifyIcon.ShowCustomBalloon(view, PopupAnimation.None, 5000);
		}

        private void AppContext_CheckForNewAdsComplete(object sender, EventArgs<List<Core.Entities.Ad>> e)
        {
            _iconAnimationTimer.Stop();
            MyNotifyIcon.Icon = _downloadCompletedIcon;
        }

        private void _iconAnimationTimer_Tick(object sender, EventArgs e)
        {
            _downloadIconIndex++;
            if (_downloadIconIndex >= _downloadIcons.Length)
            {
                _downloadIconIndex = 0;
            }
            MyNotifyIcon.Icon = _downloadIcons[_downloadIconIndex];
        }

        private void AppContext_CheckForNewAdsStart(object sender, EventArgs e)
        {
            _iconAnimationTimer.Start();
        }
	}
}
