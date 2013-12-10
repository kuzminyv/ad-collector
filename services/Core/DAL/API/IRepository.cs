using Core.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL.API
{
    public interface IRepository<TEntity>
    {
        QueryResult<TEntity> GetList(Query query);
        void AddList(List<TEntity> list);
        void AddItem(TEntity item);
    }
}
