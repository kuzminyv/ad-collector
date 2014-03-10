using Core.BLL.Common;
using Core.DAL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.BLL
{
    public class UserProfileManager
    {
        public void SaveItem(UserProfile userProfile)
        {
            Repositories.UserProfilesRepository.SaveItem(userProfile);
        }

        public UserProfile GetItem(int userId)
        {
            return Repositories.UserProfilesRepository.GetItem(userId);
        }
    }
}
