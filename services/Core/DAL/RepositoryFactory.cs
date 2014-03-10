using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.DAL.API;
using MsSql = Core.DAL.MsSql;
using Bin = Core.DAL.Binary;

namespace Core.DAL.Common
{
    public class RepositoryFactory
    {
        private DbProvider _dbProvider;
        private Dictionary<Type, Func<object>> _repositoryActivators;

        public RepositoryType CreateRepository<RepositoryType>()
        {
            return (RepositoryType)_repositoryActivators[typeof(RepositoryType)]();
        }

        private void RegisterBinaryRepositories()
        {
            _repositoryActivators = new Dictionary<Type, Func<object>>();

            _repositoryActivators.Add(typeof(IAdsRepository), () => new Bin.AdsRepository());
            _repositoryActivators.Add(typeof(IAdLinksRepository), () => new Bin.AdLinksRepository());
            _repositoryActivators.Add(typeof(ILogEntriesRepository), () => new Bin.LogEntriesRepository());
            _repositoryActivators.Add(typeof(ISettingsRepository), () => new Bin.SettingsRepository());
            _repositoryActivators.Add(typeof(IAdHistoryItemsRepository), () => new Bin.AdHistoryItemsRepository());
            _repositoryActivators.Add(typeof(IMetadataRepository), () => new Bin.MetadataRepository());
            _repositoryActivators.Add(typeof(IAdImagesRepository), () => new Bin.AdImagesRepository());
            _repositoryActivators.Add(typeof(IStreetsRepository), () => new Bin.StreetsRepository());
        }

        private void RegisterMsSqlRepositories()
        {
            _repositoryActivators = new Dictionary<Type, Func<object>>();

            _repositoryActivators.Add(typeof(IAdsRepository), () => new MsSql.AdsRepository());
            _repositoryActivators.Add(typeof(IAdLinksRepository), () => new MsSql.AdLinksRepository());
            _repositoryActivators.Add(typeof(ILogEntriesRepository), () => new MsSql.LogEntriesRepository());
            _repositoryActivators.Add(typeof(ISettingsRepository), () => new MsSql.SettingsRepository());
            _repositoryActivators.Add(typeof(IAdHistoryItemsRepository), () => new MsSql.AdHistoryItemsRepository());
            _repositoryActivators.Add(typeof(IMetadataRepository), () => new MsSql.MetadataRepository());
            _repositoryActivators.Add(typeof(IAdImagesRepository), () => new MsSql.AdImagesRepository());
            _repositoryActivators.Add(typeof(IStreetsRepository), () => new MsSql.StreetsRepository());
            _repositoryActivators.Add(typeof(IUserProfilesRepository), () => new MsSql.UserProfilesRepository());
        }

        public RepositoryFactory(DbProvider dbProvider)
        {
            _dbProvider = dbProvider;

            switch(dbProvider)
            {
                case DbProvider.Binary:
                    RegisterBinaryRepositories();
                    break;
                case DbProvider.MsSql:
                    RegisterMsSqlRepositories();
                    break;
                default:
                    throw new Exception(string.Format("Unknown storage type {0}!", dbProvider));
            }
        }
    }
}
