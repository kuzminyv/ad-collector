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
    public class StreetsRepository : BinaryRepository<Street, int>, IStreetsRepository
    {
        public new List<Street> GetList(int locationId)
        {
            lock (_lockObject)
            {
                return base.Entities.Where(kvp => kvp.Value.LocationId == locationId).Select(kvp => kvp.Value).ToList();
            }
        }
    }
}
