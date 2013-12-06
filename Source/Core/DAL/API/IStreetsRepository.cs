using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface IStreetsRepository
    {
        List<Street> GetList(int locationId);
        void AddList(List<Street> entities);
    }
}
