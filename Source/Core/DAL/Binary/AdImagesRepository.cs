using Core.DAL.API;
using Core.DAL.Binary.Common;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Binary
{
    public class AdImagesRepository : BinaryRepository<AdImage, int>, IAdImagesRepository
    {
        public new List<AdImage> GetList(int adId)
        {
            lock (_lockObject)
            {
                return base.Entities.Where(kvp => kvp.Value.AdId == adId).Select(kvp => kvp.Value).ToList();
            }
        }
    }
}
