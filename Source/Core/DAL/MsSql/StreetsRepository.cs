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
    public class StreetsRepository : MsSqlRepository<DbStreet, Street, int>, IStreetsRepository
    {
        #region Abstract methods implementation
        protected override DbSet<DbStreet> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.DbStreets;
        }

        protected override IQueryable<DbStreet> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbStreet> entities, List<DAL.Common.Filter> filters)
        {
            return entities;
        }

        protected override IQueryable<DbStreet> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbStreet> entities, List<DAL.Common.Sort> sorts)
        {
            return entities;
        }

        protected override DbStreet CreateDbEntity(int id)
        {
            return new DbStreet() { Id = id };
        }

        protected override void UpdateEntityId(Street entity, DbStreet dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion

        public List<Street> GetList(int locationId)
        {
            List<Street> result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertAllToEntity(
                    context.DbStreets.Where(h => h.LocationId == locationId)).ToList();
            });
            return result;
        }
    }
}
