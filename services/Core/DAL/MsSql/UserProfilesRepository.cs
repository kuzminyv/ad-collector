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
using System.Xml.Serialization;
using Core.Utils;

namespace Core.DAL.MsSql
{
    public class UserProfilesRepository : MsSqlRepository<DbUserProfile, UserProfile, int>, IUserProfilesRepository
    {
        #region Abstract methods implementation

        protected override DbSet<DbUserProfile> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.DbUserProfiles;
        }

        protected override DbUserProfile CreateDbEntity(int id)
        {
            return new DbUserProfile() { Id = id };
        }

        protected override IQueryable<DbUserProfile> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbUserProfile> entities, List<DAL.Common.Filter> filters)
        {
            if (filters.Any())
            {
                throw new NotImplementedException();
            }
            return entities;
        }

        protected override IQueryable<DbUserProfile> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbUserProfile> entities, List<DAL.Common.Sort> sorts)
        {
            if (sorts.Any())
            {
                throw new NotImplementedException();
            }
            return entities;
        }

        protected override void UpdateEntityId(UserProfile entity, DbUserProfile dbEntity)
        {
            entity.Id = dbEntity.Id;
        }

        #endregion

        protected override void ConfigureEntityMapping()
        { 
            //custom mapping
        }

        public override DbUserProfile ConvertToDbEntity(UserProfile entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new DbUserProfile()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                AdsQuery = Serializer.ToXmlString(entity.AdsQuery)
            };
        }

        public override UserProfile ConvertToEntity(DbUserProfile dbEntity)
        {
            if (dbEntity == null)
            {
                return null;
            }

            return new UserProfile()
            {
                Id = dbEntity.Id,
                UserId = dbEntity.UserId,
                AdsQuery = Serializer.FromXmlString<AdsQuery>(dbEntity.AdsQuery)
            };
        }

        public void SaveItem(UserProfile userProfile)
        {
            if (userProfile.Id > 0)
            {
                UpdateItem(userProfile);
            }
            else
            {
                AddItem(userProfile);
            }
        }

        public UserProfile GetItem(int userId)
        {
            UserProfile userProfile = null;
            ExecuteDbOperation(context =>
            {
                userProfile = ConvertToEntity(context.DbUserProfiles.Where(up => up.UserId == userId).FirstOrDefault());
            });
            return userProfile;
        }
    }
}
