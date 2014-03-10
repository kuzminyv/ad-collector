using System;
using System.Collections.Generic;
using System.Text;

namespace Core.BLL
{
	public class Managers
	{
		private static AdsManager _adManager = new AdsManager();
		public static AdsManager AdManager
		{
			get
			{
				return _adManager;
			}
		}

        private static AdLinksManager _adLinksManager = new AdLinksManager();
        public static AdLinksManager AdLinksManager
        {
            get
            {
                return _adLinksManager;
            }
        }

        private static LogEntriesManager _logEntriesManager = new LogEntriesManager();
        public static LogEntriesManager LogEntriesManager
        {
            get
            {
                return _logEntriesManager;
            }
        }

        private static SettingsManager _settingsManager = new SettingsManager();
        public static SettingsManager SettingsManager
        {
            get
            {
                return _settingsManager;
            }
        }

        private static ConnectorsManager _connectorsManager = new ConnectorsManager();
        public static ConnectorsManager ConnectorsManager
        {
            get
            {
                return _connectorsManager;
            }
        }

        private static ExportManager _exportManager = new ExportManager();
        public static ExportManager ExportManager
        {
            get
            {
                return _exportManager;
            }
        }

        private static AdHistoryManager _adHistoryManager = new AdHistoryManager();
        public static AdHistoryManager AdHistoryManager
        {
            get
            {
                return _adHistoryManager;
            }
        }

        private static MetadataManager _metadataManager = new MetadataManager();
        public static MetadataManager MetadataManager
        {
            get
            {
                return _metadataManager;
            }
        }

        private static StreetsManager _streetsManager = new StreetsManager();
        public static StreetsManager StreetsManager
        {
            get
            {
                return _streetsManager;
            }
        }

        private static UserProfileManager _userProfileManager = new UserProfileManager();
        public static UserProfileManager UserProfileManager
        {
            get
            {
                return _userProfileManager;
            }
        }
	}
}
