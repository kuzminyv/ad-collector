using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface IMetadataRepository
    {
        QueryResult<Metadata> GetList(Query query);

        void AddItem(Metadata entity);
        void SaveItem(Metadata entity);
        Metadata GetItem(int userId, int adId);
    }
}
