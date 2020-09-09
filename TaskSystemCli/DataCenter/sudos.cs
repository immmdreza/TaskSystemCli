using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TaskSystemCli.DataCenter
{
    class Sudos
    {
        private static readonly string Address = "DataCenter/sudos.json";

        public static async Task AddSudoAsync(int id)
        {
            List<int> sudos = new List<int>();
            using (StreamReader sr = new StreamReader(Address))
            {
                sudos = JsonConvert.DeserializeObject<List<int>>(await sr.ReadToEndAsync());
            }

            using StreamWriter sw = new StreamWriter(Address);
            sudos.Add(id);

            await sw.WriteAsync(JsonConvert.SerializeObject(sudos));
        }

        public static async Task<List<int>> GetSudosAsync()
        {
            using StreamReader sr = new StreamReader(Address);

            return JsonConvert.DeserializeObject<List<int>>(await sr.ReadToEndAsync());
        }
    }
}
