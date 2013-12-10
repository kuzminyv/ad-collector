using Core.DAL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BLL
{
    public class StreetsManager
    {
        public List<Street> GetList(int locationId)
        {
            return Repositories.StreetsRepository.GetList(locationId);
        }
    }
}
