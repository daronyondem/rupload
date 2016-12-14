using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class JsonConfigService<T> : rupload.Services.IJsonConfigService<T>
    {
        const string configFileName = "rupload_azureconfig.json";

        public async Task SaveSetting(T configObject)
        {
            string outputJson = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(configObject, Formatting.Indented));
            string configEnvironment = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileFullPath = configEnvironment + @"\" + configFileName;
            StreamWriter sWriter = File.CreateText(fileFullPath);
            await sWriter.WriteLineAsync(outputJson);
            await sWriter.FlushAsync();
            sWriter.Close();
        }
        public async Task<T> GetSettings()
        {
            T retVal = default(T);
            string configEnvironment = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileFullPath = configEnvironment + @"\" + configFileName;
            if (File.Exists(fileFullPath))
            {
                using (var reader = File.OpenText(fileFullPath))
                {
                    var jsonContent = await reader.ReadToEndAsync();
                    retVal = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(jsonContent));
                }
            }
            return retVal;
        }
    }
}
