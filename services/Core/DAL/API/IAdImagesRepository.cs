using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface IAdImagesRepository
    {
        List<AdImage> GetList(int adId);
        void AddList(List<AdImage> entities);
        void SetList(int adId, List<AdImage> images);
    }
}
