using System;
using System.IO;
using System.Threading.Tasks;

using static Newtonsoft.Json.JsonConvert;

namespace DotcoinApi.Settings
{
    public class SettingsManager : IDisposable
    {
        private readonly string _fileLocation = "dotcoin.settings";
        private SettingsModel _settingsCache;
        
        public async Task<SettingsModel> GetSettings()
        {
            if(!ValidateFile())
            {
                return new SettingsModel();
            }

            var fileContents = await File.ReadAllTextAsync(_fileLocation);

            _settingsCache = DeserializeObject<SettingsModel>(fileContents);
            
            return _settingsCache;
        }

        public void Updatesettings(SettingsModel settingsModel)
        {
            _settingsCache = settingsModel;
        }
        
        public void Dispose()
        {
            SaveSettings(_settingsCache);
        }
        
        private void SaveSettings(SettingsModel settingsModel)
        {
            ValidateFile();

            using (var streamWriter = File.CreateText(_fileLocation))
            {
                streamWriter.WriteLine(SerializeObject(settingsModel));
            }
            
        }

        /// <summary>
        /// Validates if a file exists, if not creates the settings file
        /// </summary>
        /// <returns>Returns true if the file exists, false if a new file is created</returns>
        private bool ValidateFile()
        {
            if (!File.Exists(_fileLocation))
            {
                File.Create(_fileLocation);
                return false;
            }
            return true;
        }
    }
}