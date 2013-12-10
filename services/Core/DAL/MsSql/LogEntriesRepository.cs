using Core.DAL.Common;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.MsSql
{
    public class LogEntriesRepository : MsSqlRepository<DbLogEntry, LogEntry, int>, ILogEntriesRepository
    {
        #region Abstract methods implementation
        protected override System.Data.Entity.DbSet<DbLogEntry> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.LogEntries;
        }

        protected override IQueryable<DbLogEntry> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbLogEntry> entities, List<Filter> list)
        {
            return entities;
        }

        protected override IQueryable<DbLogEntry> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbLogEntry> entities, List<Sort> list)
        {
            var result = entities;
            Sort sort = list.First();
            switch (sort.Name)
            {
                case "Time":
                    if (sort.SortOrder == SortOrder.Ascending)
                        result = result.OrderBy(a => a.Time);
                    else
                        result = result.OrderByDescending(a => a.Time);
                    break;
                default:
                    throw new Exception(string.Format("Not supported sort {0}!", sort.Name));
            }
            return result;
        }

        protected override DbLogEntry CreateDbEntity(int id)
        {
            return new DbLogEntry() { Id = id };
        }

        protected override void UpdateEntityId(LogEntry entity, DbLogEntry dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion
    }
}
