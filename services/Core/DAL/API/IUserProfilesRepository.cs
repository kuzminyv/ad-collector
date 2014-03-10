using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL.API
{
    public interface IUserProfilesRepository
    {
        void SaveItem(UserProfile userProfile);
        UserProfile GetItem(int userId);
    }
}
