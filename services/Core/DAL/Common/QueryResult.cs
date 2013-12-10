using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.DAL.Common
{
    [DataContract]
    public class QueryResult<TEntity>
    {
        [DataMember(Name="items")]
        public List<TEntity> Items
        {
            get;
            set;
        }

        [DataMember(Name = "totalCount")]
        public int? TotalCount
        {
            get;
            set;
        }

        public QueryResult(List<TEntity> items, int? totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }

        public QueryResult(List<TEntity> items)
            : this(items, null)
        { 
        }
    }
}
