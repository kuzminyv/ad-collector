using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface ISettingsRepository
    {
        Setting GetItem(string settingName);

        void AddItem(Setting setting);

        void UpdateItem(Setting setting);
    }
}
