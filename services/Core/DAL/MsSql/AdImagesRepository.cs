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
    public class AdImagesRepository : MsSqlRepository<DbAdImage, AdImage, int>, IAdImagesRepository
    {
        #region Abstract methods implementation
        protected override DbSet<DbAdImage> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.DbAdImages;
        }

        protected override IQueryable<DbAdImage> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbAdImage> entities, List<DAL.Common.Filter> filters)
        {
            return entities;
        }

        protected override IQueryable<DbAdImage> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbAdImage> entities, List<DAL.Common.Sort> sorts)
        {
            return entities;
        }

        protected override DbAdImage CreateDbEntity(int id)
        {
            return new DbAdImage() { Id = id };
        }

        protected override IQueryable<DbAdImage> ApplyDefaultOrder(AdCollectorDBEntities context, IQueryable<DbAdImage> dbEntities)
        {
            return dbEntities.OrderBy(entity => entity.Id);
        }

        protected override void UpdateEntityId(AdImage entity, DbAdImage dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion

        public List<AdImage> GetList(int adId)
        {
            List<AdImage> result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertAllToEntity(
                    context.DbAdImages.Where(h => h.AdId == adId)).ToList();
            });
            return result;
        }

        public void SetList(int adId, List<AdImage> images)
        {
            ExecuteDbOperation(context =>
            {
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.AdImages WHERE AdId = " + adId);

                for (int i = 0; i < images.Count; i++)
                {
                    images[i].AdId = adId;
                }
                AddList(images);
            });
        }
    }
}
