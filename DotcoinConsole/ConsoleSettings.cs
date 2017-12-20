using System;
using System.IO;
using System.Threading.Tasks;

using static Newtonsoft.Json.JsonConvert;

namespace SimpleBlockChain
{
    public class ConsoleSettings
    {
        public string ClientBaseUrl;
        public int ClientPort;

        public async Task SaveSettings(string fileLocation)
        {
            var serialized = SerializeObject(this);

            if (!File.Exists(fileLocation))
            {
                File.Create(fileLocation);
            }

            await File.WriteAllTextAsync(fileLocation, serialized);
        }

        public async Task<ConsoleSettings> LoadSettings(string fileLocation)
        {
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException(fileLocation);
            }
            
            var fileContents = await File.ReadAllTextAsync(fileLocation);

            var temp = DeserializeObject<ConsoleSettings>(fileContents);
            
            ClientBaseUrl = temp.ClientBaseUrl;
            ClientPort = temp.ClientPort;
            
            return this;
        }
    }
}