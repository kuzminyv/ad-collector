using Core.DAL.Binary.Common;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Binary
{
    public class MetadataRepository : BinaryRepository<Metadata, int>, IMetadataRepository
    {
        protected bool IsNew(Metadata entity)
        {
            lock (_lockObject)
            {
                return !Entities.Where(kvp => kvp.Value.UserId == entity.UserId && kvp.Value.AdId == entity.AdId).Any();
            }
        }

        protected bool IsEmpty(Metadata entity)
        {
            return string.IsNullOrEmpty(entity.Note) && !entity.IsFavorite;
        }

        public void SaveItem(Metadata entity)
        {
            if (IsEmpty(entity))
            {
                if (!IsNew(entity))
                {
                    DeleteItemById(entity.Id);
                }
            }
            else
            {
                if (IsNew(entity))
                {
                    AddItem(entity);
                }
                else
                {
                    UpdateItem(entity);
                }
            }
        }

        public Metadata GetItem(int userId, int adId)
        {
            lock (_lockObject)
            {
                return Entities.Where(kvp => kvp.Value.UserId == userId && kvp.Value.AdId == adId).Select(kvp => kvp.Value).FirstOrDefault();
            }
        }


    }
}
