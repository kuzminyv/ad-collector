using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UI.Desktop.Commands
{
	public class AppCommands
	{
		private static Command _showAdList = new ShowAdListCommand();
		public static Command ShowAdList
		{
			get
			{
				return _showAdList;
			}
		}

		private static Command _applicationShudown = new ShutdownApplicationCommand();
		public static Command ApplicationShutdown
		{
			get
			{
				return _applicationShudown;
			}
		}

		private static Command _openUrl = new OpenUrlCommand();
		public static Command OpenUrl
		{
			get
			{
				return _openUrl;
			}
		}

        private static Command _checkForNewAds = new CheckForNewAdsCommand();
        public static Command CheckForNewAds
        {
            get
            {
                return _checkForNewAds;
            }
        }

        private static Command _showSettingsCommand = new ShowSettingsCommand();
        public static Command ShowSettingsCommand
        {
            get
            {
                return _showSettingsCommand;
            }
        }

        private static Command _showLogCommand = new ShowLogCommand();
        public static Command ShowLogCommand
        {
            get
            {
                return _showLogCommand;
            }
        }

        private static Command _recreateAutoLinksCommand = new RecreateAutoLinksCommand();
        public static Command RecreateAutoLinksCommand
        {
            get
            {
                return _recreateAutoLinksCommand;
            }
        }

        private static Command _showLinkedAdsCommand = new ShowLinkedAdsCommand();
        public static Command ShowLinkedAdsCommand
        {
            get
            {
                return _showLinkedAdsCommand;
            }
        }

        private static Command _exportCommand = new ExportCommand();
        public static Command ExportCommand
        {
            get
            {
                return _exportCommand;
            }
        }

        private static Command _showAdHistoryCommand = new ShowAdHistoryCommand();
        public static Command ShowAdHistoryCommand
        {
            get
            {
                return _showAdHistoryCommand;
            }
        }        
	}
}
