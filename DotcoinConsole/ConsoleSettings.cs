using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace SimpleBlockChain
{
    public class ConsoleSettings
    {
        const string DEFAULT_FILE_LOCATION = "ConsoleSettings.txt";
       
        public string ClientBaseUrl = string.Empty;
        public int ClientPort = -1;
        public string DefaultRoot = string.Empty;

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };
        
        public string GetConnectionUrl()
        {
            if (ClientBaseUrl == string.Empty || ClientPort == -1 || DefaultRoot == string.Empty)
            {
                throw new Exception("Object not intialized, make sure to load the object");
            }
            return string.Format("{0}:{1}{2}", ClientBaseUrl, ClientPort, DefaultRoot);
        }
        
        public void SaveSettings(string fileLocation)
        {
            var serialized = SerializeObject(this, _serializerSettings);
            
            using (var file = File.CreateText(fileLocation))
            {
                file.Write(serialized);
            }
        }

        public async Task<ConsoleSettings> LoadSettings()
        {
            return await LoadSettings(DEFAULT_FILE_LOCATION);
        }
 
        public async Task<ConsoleSettings> LoadSettings(string fileLocation)
        {
            if (!File.Exists(fileLocation))
            {
                CreateDefaultFile();
                await LoadSettings(DEFAULT_FILE_LOCATION);
            }
            
            var fileContents = await File.ReadAllTextAsync(fileLocation);

            var temp = DeserializeObject<ConsoleSettings>(fileContents);
            
            ClientBaseUrl = temp.ClientBaseUrl;
            ClientPort = temp.ClientPort;
            
            return this;
        }

        private void CreateDefaultFile()
        {
            var serializeObject = SerializeObject(new ConsoleSettings
            {
                ClientBaseUrl = "http://localhost",
                ClientPort = 5000,
                DefaultRoot = "/api/Coin/"
            }, _serializerSettings);
            
            using (var file = File.CreateText(DEFAULT_FILE_LOCATION))
            {
                file.Write(serializeObject);   
            }
        }
    }
}