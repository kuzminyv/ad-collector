using Core.BLL.Common;
using Core.DAL.Common;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.DAL.MsSql
{
    public class AdLinksRepository : MsSqlRepository<DbAdLink, AdLink, int>, IAdLinksRepository
    {
        #region Abstract methods implementation
        protected override System.Data.Entity.DbSet<DbAdLink> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.AdLinks;
        }

        protected override IQueryable<DbAdLink> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbAdLink> entities, List<Filter> filters)
        {
            return entities;
        }

        protected override IQueryable<DbAdLink> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbAdLink> entities, List<Sort> sorts)
        {
            return entities;
        }

        protected override DbAdLink CreateDbEntity(int id)
        {
            return new DbAdLink() { Id = id };
        }

        protected override void UpdateEntityId(AdLink entity, DbAdLink dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion

        public List<AdLink> GetList(int adId)
        {
            return ExecuteDbOperation(context =>
            {
                return ConvertAllToEntity(context.AdLinks.Where(adl => adl.AdId == adId)).ToList();
            });
        }

        public void DropAllAutoLinks()
        {
            ExecuteDbOperation(context =>
            {
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.AdLinks");
            });
        }

        public void CreateAutoLinks(Ad ad)
        {
            throw new NotImplementedException();
        }
    }
}
