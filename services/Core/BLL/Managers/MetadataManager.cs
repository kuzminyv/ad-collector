using Core.DAL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BLL
{
    public class MetadataManager
    {
        public Metadata GetItem(int userId, int adId)
        {
            return Repositories.MetadataRepository.GetItem(userId, adId);
        }

        public void SaveItem(Metadata item)
        {
            Repositories.MetadataRepository.SaveItem(item);
        }
    }
}
