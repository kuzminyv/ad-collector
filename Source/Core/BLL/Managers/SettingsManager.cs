using Core.DAL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.BLL
{
    public class SettingsManager
    {
        private Settings _settings;
        public Settings GetSettings()
        {
            if (_settings != null)
            {
                return _settings;
            }

            Settings settings = GetDefaultSettings();
            Setting setting;

            setting = Repositories.SettingsRepository.GetItem("CheckForNewAdsMaxAdsCount");
            if (setting != null)
            {
                settings.CheckForNewAdsMaxAdsCount = int.Parse(setting.Value);
            }

            setting = Repositories.SettingsRepository.GetItem("CheckForNewAdsIntervalMinutes");
            if (setting != null)
            {
                settings.CheckForNewAdsIntervalMinutes = int.Parse(setting.Value);
            }

            return settings;
        }

        public void SaveSettings(Settings settings)
        {
            SaveSetting("CheckForNewAdsMaxAdsCount", settings.CheckForNewAdsMaxAdsCount.ToString());
            SaveSetting("CheckForNewAdsIntervalMinutes", settings.CheckForNewAdsIntervalMinutes.ToString());
            _settings = null;
        }

        private void SaveSetting(string name, string value)
        {
            Setting setting = Repositories.SettingsRepository.GetItem(name);
            if (setting == null)
            {
                setting = new Setting() { Name = name, Value = value };
                Repositories.SettingsRepository.AddItem(setting);
            }
            else
            {
                setting.Value = value;
                Repositories.SettingsRepository.UpdateItem(setting);
            }
        }

        public Settings GetDefaultSettings()
        {
            Settings settings = new Settings()
            {
                CheckForNewAdsMaxAdsCount = 5000,
                CheckForNewAdsIntervalMinutes = 300
            };

            return settings;
        }
    }
}
