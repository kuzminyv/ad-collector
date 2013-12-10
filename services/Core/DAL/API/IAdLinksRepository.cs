using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface IAdLinksRepository
    {
        List<AdLink> GetList(int adId);
        QueryResult<AdLink> GetList(Query query);
        void CreateAutoLinks(Ad ad);
        void DropAllAutoLinks();

        void AddList(List<AdLink> list);
    }
}
