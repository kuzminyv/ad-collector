using Core.DAL.Common;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.MsSql
{
    public class MetadataRepository : MsSqlRepository<DbMetadata, Metadata, int>, IMetadataRepository
    {
        #region Abstract methods implementation
        protected override System.Data.Entity.DbSet<DbMetadata> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.DbMetadatas;
        }

        protected override IQueryable<DbMetadata> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbMetadata> entities, List<Filter> list)
        {
            return entities;
        }

        protected override IQueryable<DbMetadata> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbMetadata> entities, List<Sort> list)
        {
            return entities;
        }

        protected override DbMetadata CreateDbEntity(int id)
        {
            return new DbMetadata() { Id = id };
        }

        protected override void UpdateEntityId(Metadata entity, DbMetadata dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion

        protected Metadata GetItem(Metadata entity)
        {
            Metadata result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertToEntity(context.DbMetadatas.Where(m => m.UserId == entity.UserId && m.AdId == entity.AdId).FirstOrDefault());
            });
            return result;
        }

        protected bool IsEmpty(Metadata entity)
        {
            return string.IsNullOrEmpty(entity.Note) && !entity.IsFavorite;
        }

        public void SaveItem(Metadata entity)        
        {
            Metadata m = GetItem(entity);
            if (IsEmpty(entity))
            {
                if (m != null)
                {
                    DeleteItemById(m.Id);
                }
            }
            else
            {
                if (m == null)
                {
                    AddItem(entity);
                }
                else
                {
                    entity.Id = m.Id;
                    UpdateItem(entity);
                }
            }
        }

        public Metadata GetItem(int userId, int adId)
        {
            Metadata result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertToEntity(
                    context.DbMetadatas.Where(m => m.UserId == userId && m.AdId == adId).FirstOrDefault());
            });
            return result;
        }
    }
}
