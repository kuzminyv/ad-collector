using Core.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.DAL.API;

namespace Core.DAL
{
    public static class Repositories
    {
        public static DbProvider Provider
        {
            get
            {
                return (DbProvider)Enum.Parse(typeof(DbProvider), System.Configuration.ConfigurationManager.AppSettings.Get("DbProvider"));
            }
        }

        private static RepositoryFactory _factory;
        private static RepositoryFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new RepositoryFactory(Provider);                    
                }
                return _factory;
            }
        }

        private static IAdsRepository _adsRepository;
        public static IAdsRepository AdsRepository
        {
            get
            {
                if (_adsRepository == null)
                {
                    _adsRepository = Factory.CreateRepository<IAdsRepository>();
                }
                return _adsRepository;
            }
        }

        private static IAdLinksRepository _adLinksRepository;
        public static IAdLinksRepository AdLinksRepository
        {
            get
            {
                if (_adLinksRepository == null)
                {
                    _adLinksRepository = Factory.CreateRepository<IAdLinksRepository>();
                }
                return _adLinksRepository;
            }
        }

        private static ILogEntriesRepository _logEntriesRepository;
        public static ILogEntriesRepository LogEntriesRepository
        {
            get
            {
                if (_logEntriesRepository == null)
                {
                    _logEntriesRepository = Factory.CreateRepository<ILogEntriesRepository>();
                }
                return _logEntriesRepository;
            }
        }

        private static ISettingsRepository _settingsRepository;
        public static ISettingsRepository SettingsRepository
        {
            get
            {
                if (_settingsRepository == null)
                {
                    _settingsRepository = Factory.CreateRepository<ISettingsRepository>();
                }
                return _settingsRepository;
            }
        }

        private static IAdHistoryItemsRepository _adHistoryItemsRepository;
        public static IAdHistoryItemsRepository AdHistoryItemsRepository
        {
            get
            {
                if (_adHistoryItemsRepository == null)
                {
                    _adHistoryItemsRepository = Factory.CreateRepository<IAdHistoryItemsRepository>();
                }
                return _adHistoryItemsRepository;
            }
        }

        private static IMetadataRepository _metadataRepository;
        public static IMetadataRepository MetadataRepository
        {
            get
            {
                if (_metadataRepository == null)
                {
                    _metadataRepository = Factory.CreateRepository<IMetadataRepository>();
                }
                return _metadataRepository;
            }
        }

        private static IStreetsRepository _streetsRepository;
        public static IStreetsRepository StreetsRepository
        {
            get
            {
                if (_streetsRepository == null)
                {
                    _streetsRepository = Factory.CreateRepository<IStreetsRepository>();
                }
                return _streetsRepository;
            }
        }

        private static IAdImagesRepository _adImagesRepository;
        public static IAdImagesRepository AdImagesRepository
        {
            get
            {
                if (_adImagesRepository == null)
                {
                    _adImagesRepository = Factory.CreateRepository<IAdImagesRepository>();
                }
                return _adImagesRepository;
            }
        }
    }
}
