using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DAL.API;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System.Threading;
using Core.BLL.Common;

namespace Core.DAL.MsSql
{
    public class AdHistoryItemsRepository : MsSqlRepository<DbAdHistoryItem, AdHistoryItem, int>, IAdHistoryItemsRepository
    {
        #region Abstract methods implementation
        protected override DbSet<DbAdHistoryItem> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.DbAdHistoryItems;
        }

        protected override IQueryable<DbAdHistoryItem> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbAdHistoryItem> entities, List<DAL.Common.Filter> filters)
        {
            return entities;
        }

        protected override IQueryable<DbAdHistoryItem> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbAdHistoryItem> entities, List<DAL.Common.Sort> sorts)
        {
            return entities;
        }

        protected override IQueryable<DbAdHistoryItem> ApplyDefaultOrder(AdCollectorDBEntities context, IQueryable<DbAdHistoryItem> dbEntities)
        {
            return dbEntities.OrderBy(entity => entity.Id);
        }

        protected override DbAdHistoryItem CreateDbEntity(int id)
        {
            return new DbAdHistoryItem() { Id = id };
        }

        protected override void UpdateEntityId(AdHistoryItem entity, DbAdHistoryItem dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion

        public List<AdHistoryItem> GetList(int adId)
        {
            List<AdHistoryItem> result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertAllToEntity(
                    context.DbAdHistoryItems.Where(h => h.AdId == adId)).OrderBy(h => h.AdPublishDate).ToList();
            });
            return result;
        }
    }
}
