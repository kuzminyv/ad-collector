using AutoMapper;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db = Core.DAL.MsSql.Common;
using System.Threading;
using Core.DAL.API;
using SQL = System.Data.SqlClient;

namespace Core.DAL.MsSql
{
    public abstract class MsSqlRepository<TDbModelEntity, TEntity, TKey>: IRepository<TEntity>
        where TDbModelEntity : class
    {

        protected abstract DbSet<TDbModelEntity> GetDbEntities(Db.AdCollectorDBEntities context);
        protected abstract IQueryable<TDbModelEntity> ApplyFilter(Db.AdCollectorDBEntities context, IQueryable<TDbModelEntity> entities, List<Filter> filters);
        protected abstract IQueryable<TDbModelEntity> ApplyOrder(Db.AdCollectorDBEntities context, IQueryable<TDbModelEntity> entities, List<Sort> sorts);
        protected abstract TDbModelEntity CreateDbEntity(TKey id);
        protected abstract void UpdateEntityId(TEntity entity, TDbModelEntity dbEntity);

        private Db.AdCollectorDBEntities CreateDbContext()
        {
            var context = new Db.AdCollectorDBEntities();
            //context.Configuration.ValidateOnSaveEnabled = false;
            return context;

        }

        private object _connectionStringSync = new object();
        private string _connectionString;
        private string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    lock (_connectionStringSync)
                    {
                        if (_connectionString == null)
                        {
                            var builder = new System.Data.Common.DbConnectionStringBuilder();
                            builder.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AdCollectorDBEntities"].ConnectionString;
                            _connectionString = builder["provider connection string"].ToString();
                        }
                    }
                }
                return _connectionString;
            }
        }

        protected TResult ExecuteDbOperation<TResult>(Func<Db.AdCollectorDBEntities, TResult> operation)
        {
            using (var context = CreateDbContext())
            {
                try
                {
                    return operation(context);
                }
                catch (DbEntityValidationException ex)
                {
                    var sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    throw new DbEntityValidationException(
                        "Entity Validation Failed - errors follow:\n" +
                        sb.ToString(), ex
                        );
                }
            }
        }

        protected void ExecuteDbOperation(Action<Db.AdCollectorDBEntities> operation)
        {
            ExecuteDbOperation<object>(context =>
            {
                operation(context);
                return null;
            });
        }

        protected void ExecuteSqlOperation(Action<SQL.SqlConnection> operation)
        {
            using (var conn = new SQL.SqlConnection(ConnectionString))
            {
                conn.Open();
                operation(conn);
            }
        }

        protected virtual void ConfigureEntityMapping()
        {
            Mapper.CreateMap<TDbModelEntity, TEntity>();
            Mapper.CreateMap<TEntity, TDbModelEntity>();
        }

        public IEnumerable<TEntity> ConvertAllToEntity(IEnumerable<TDbModelEntity> dbEntities)
        {
            foreach (TDbModelEntity dbEntity in dbEntities)
            {
                yield return ConvertToEntity(dbEntity);
            }
        }

        public IEnumerable<TDbModelEntity> ConvertAllToDbEntity(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                yield return ConvertToDbEntity(entity);
            }
        }

        public virtual TEntity ConvertToEntity(TDbModelEntity dbEntity)
        {
            return Mapper.Map<TDbModelEntity, TEntity>(dbEntity);
        }

        public virtual TDbModelEntity ConvertToDbEntity(TEntity entity)
        {
            return Mapper.Map<TEntity, TDbModelEntity>(entity);
        }

        public virtual QueryResult<TEntity> GetList(Query query)
        {
            QueryResult<TEntity> result = null;
            int? totalCount = null;

            ExecuteDbOperation(context =>
            {
                IQueryable<TDbModelEntity> dbEntities = GetDbEntities(context);

                if (query != null)
                {
                    if (query.Filters != null && query.Filters.Count > 0)
                    {
                        dbEntities = ApplyFilter(context, dbEntities, query.Filters);
                    }
                    if (query.Sorts != null && query.Sorts.Count > 0)
                    {
                        dbEntities = ApplyOrder(context, dbEntities, query.Sorts);
                    }
                    if (query.Start.HasValue && query.Limit.HasValue)
                    {
                        totalCount = dbEntities.Count();
                        if (query.Start > 0)
                        {
                            dbEntities = dbEntities.Skip(query.Start.Value);
                        }
                        dbEntities = dbEntities.Take(query.Limit.Value);
                    }
                }

                result = new QueryResult<TEntity>(
                            ConvertAllToEntity(dbEntities).ToList(), totalCount);
            });

            return result;
        }

        public void AddItem(TEntity entity)
        { 
            ExecuteDbOperation(context =>
            {
                var dbEntity = ConvertToDbEntity(entity);
                GetDbEntities(context).Add(dbEntity);
                context.SaveChanges();
                UpdateEntityId(entity, dbEntity);
            });
        }

        public void AddList(List<TEntity> items)
        {
            ExecuteDbOperation(context =>
            {
                var dbEntities = ConvertAllToDbEntity(items);
                GetDbEntities(context).AddRange(dbEntities);
                context.SaveChanges();
            });
        }

        public void UpdateItem(TEntity entity)
        {
            ExecuteDbOperation(context =>
            {
                TDbModelEntity updatedDbEntity = ConvertToDbEntity(entity);
                context.Entry(updatedDbEntity).State = EntityState.Modified;
                context.SaveChanges();
            });
        }

        public void DeleteItemById(TKey id)
        {
            ExecuteDbOperation(context =>
            {
                DbSet<TDbModelEntity> entities = GetDbEntities(context);
                TDbModelEntity toDelete = CreateDbEntity(id);
                entities.Attach(toDelete);
                entities.Remove(toDelete);
                context.SaveChanges();
            });
        }

        public void DeleteItems(IEnumerable<TKey> ids)
        {
            ExecuteDbOperation(context =>
            {
                DbSet<TDbModelEntity> entities = GetDbEntities(context);
                foreach (TKey id in ids)
                {
                    TDbModelEntity toDelete = CreateDbEntity(id);
                    entities.Attach(toDelete);
                    entities.Remove(toDelete);
                }                
                context.SaveChanges();
            });
        }


        public MsSqlRepository()
        {
            ConfigureEntityMapping();
        }
    }
}
