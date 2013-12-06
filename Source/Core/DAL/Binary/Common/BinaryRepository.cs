using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using Core.Entities;
using Core.Utils;
using System.IO;
using Core.DAL.API;
using Core.DAL.Common;

namespace Core.DAL.Binary.Common
{
    public abstract class BinaryRepository<TEntity, TKey> :IRepository<TEntity>
        where TEntity: Entity<TKey>
    {
        protected object _lockObject = new object();


        private string FileName 
        {
            get
            {
                return string.Format("{0}.dat", (typeof(TEntity)).Name);
            }
        }

        private Dictionary<TKey, TEntity> _entities;
        protected Dictionary<TKey, TEntity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    lock (_lockObject)
                    {
                        if (_entities == null)
                        {
                            _entities = (new EntitiesSerializer<TKey, TEntity>()).Load(FileName);
                            if (_entities == null)
                            {
                                _entities = new Dictionary<TKey, TEntity>();
                            }
                        }
                    }
                }
                return _entities;
            }
        }

        protected virtual void ExecuteDbOperation(Action action)
        {
            lock (_lockObject)
            {
                action();
            }
            FlushBuffered();
        }

        protected virtual IEnumerable<TEntity> ApplyFilter(IEnumerable<TEntity> entities, List<Filter> filters)
        {
            throw new NotImplementedException("For using filters you need to override ApplyFilter method.");
        }

        protected virtual IEnumerable<TEntity> ApplyOrder(IEnumerable<TEntity> entities, List<Sort> sorts)
        {
            string sortExpression = sorts[0].Name + (sorts[0].SortOrder == SortOrder.Ascending ? " ASC" : " DESC");
            for (int i = 1; i < sorts.Count; i++)
            {
                sortExpression += ", " + sorts[i] + (sorts[i].SortOrder == SortOrder.Ascending ? " ASC" : " DESC");
            }
            return entities.AsQueryable().OrderBy(sortExpression);
        }

        public QueryResult<TEntity> GetList(Query query)
        {
            lock (_lockObject)
            {
                var result = Entities.Select(kvp => kvp.Value);
                int? totalCount = null;
                if (query != null)
                {
                    if (query.OptionalFields != null && query.OptionalFields.Count > 0)
                    {
                        result = FillOptionalFields(result, query.OptionalFields);
                    }
                    if (query.Filters != null && query.Filters.Count > 0)
                    {
                        result = ApplyFilter(result, query.Filters);
                    }
                    if (query.Sorts != null && query.Sorts.Count > 0)
                    {
                        result = ApplyOrder(result, query.Sorts);
                    }
                    if (query.Start.HasValue && query.Limit.HasValue)
                    {
                        totalCount = result.Count();
                        if (query.Start > 0)
                        {
                            result = result.Skip(query.Start.Value);
                        }
                        result = result.Take(query.Limit.Value);
                    }
                }
                return new QueryResult<TEntity>(result.ToList(), totalCount);
            }
        }

        protected virtual IEnumerable<TEntity> FillOptionalFields(IEnumerable<TEntity> entities, List<string> optionalFields)
        {
            throw new NotImplementedException("For using optional fields you need to override FillOptionalFields method.");
        }

        private void FlushBuffered()
        {
            BufferedAction.DelayAction(Flush, 1000);
        }

        private void Flush()
        {
            lock (_lockObject)
            {
                if (File.Exists(FileName))
                {
                    string backupName = string.Format("{0}.bak", Path.GetFileNameWithoutExtension(FileName));
                    if (File.Exists(backupName))
                    {
                        File.Delete(backupName);
                    }
                    File.Move(FileName,  backupName);
                }
                (new EntitiesSerializer<TKey, TEntity>()).Save(FileName, Entities);
            }
        }

        public void AddList(List<TEntity> entities)
        {
            if (entities.Count > 0)
            {
                lock (_lockObject)
                {
                    if (typeof(TKey) == typeof(int))
                    {
                        int lastId = Entities.Count > 0 ? (int)(object)Entities.Max(kvp => kvp.Key) : 0;
                        foreach (var entity in entities)
                        {
                            if ((int)(object)entity.Id == 0)
                            {
                                lastId++;
                                entity.Id = (TKey)(object)lastId;
                            }
                            Entities.Add(entity.Id, entity);
                        }
                    }
                    else
                    {
                        foreach (var entity in entities)
                        {
                            Entities.Add(entity.Id, entity);
                        }
                    }
                }
                FlushBuffered();
            }
        }

        public void AddItem(TEntity item)
        {
            this.AddList(new List<TEntity>() { item });
        }

        public void UpdateItem(TEntity entity)
        {
            lock (_lockObject)
            {
                Entities[entity.Id] = entity;
            }
            FlushBuffered();
        }

        public TEntity GetItem(TKey id)
        {
            lock (_lockObject)
            {
                return Entities[id];
            }
        }

        public bool DeleteItemById(TKey id)
        {
            bool result;
            lock (_lockObject)
            {
                result = Entities.Remove(id);
            }

            FlushBuffered();
            return result;
        }

        public void DeleteItems(List<TKey> ids)
        {
            lock (_lockObject)
            {
                foreach (var id in ids)
                {
                    Entities.Remove(id);
                }
            }

            FlushBuffered();
        }

        public BinaryRepository()
        {
        }
    }
}
