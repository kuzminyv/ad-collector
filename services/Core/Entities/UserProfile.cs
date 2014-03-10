using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Serializable]
    public class UserProfile: Entity<int>
    {
        public int UserId
        {
            get;
            set;
        }

        public AdsQuery AdsQuery
        {
            get;
            set;
        }
    }
}
