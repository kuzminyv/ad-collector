using Core.DAL.Common;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.MsSql
{
    public class SettingsRepository : MsSqlRepository<DbSetting, Setting, int>, ISettingsRepository
    {
        #region Abstract methods implementation
        protected override System.Data.Entity.DbSet<DbSetting> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.Settings;
        }

        protected override IQueryable<DbSetting> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbSetting> entities, List<Filter> list)
        {
            return entities;
        }

        protected override IQueryable<DbSetting> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbSetting> entities, List<Sort> list)
        {
            return entities;
        }

        protected override DbSetting CreateDbEntity(int id)
        {
            return new DbSetting() { Id = id };
        }

        protected override void UpdateEntityId(Setting entity, DbSetting dbEntity)
        {
            entity.Id = dbEntity.Id;
        }

        #endregion

        public Setting GetItem(string settingName)
        {
            Setting result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertToEntity(context.Settings.Where(s => s.Name == settingName).FirstOrDefault());
            });
            return result;
        }
    }
}
