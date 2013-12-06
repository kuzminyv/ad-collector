using Core.DAL.Binary.Common;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Binary
{
    public class SettingsRepository : BinaryRepository<Setting, int>, ISettingsRepository
    {
        protected override IEnumerable<Setting> ApplyFilter(IEnumerable<Setting> entities, List<Filter> filters)
        {
            var result = entities;
            foreach (var filter in filters)
            {
                switch (filter.Name)
                {
                    case "Name":
                        var name = (string)filter.Value;
                        result = result.Where(t => t.Name == name);
                        break;
                    default:
                        throw new Exception(string.Format("Unknown filter '{0}'!", filter.Name));
                }
            }
            return result;
        }

        public Setting GetItem(string settingName)
        {
            Query query = new Query(0, 1).AddFilter("Name", settingName);
            return GetList(query).Items.FirstOrDefault();
        }
    }
}
