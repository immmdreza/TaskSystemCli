using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TaskSystemCli.DataCenter
{
    class Roles
    {
        private static readonly string Address = "DataCenter/Roles.json";

        public string Emoji { get; set; }
        public string FullName { get; set; }
        public string[] Kinders { get; set; }
        public string ShortName { get; set; }
        public string Team { get; set; }
        public string[] Achivments { get; set; }

        public static async Task<List<Roles>> GetRolesAsync()
        {
            using StreamReader sr = new StreamReader(Address);
            return JsonConvert.DeserializeObject<List<Roles>>(await sr.ReadToEndAsync());
        }
    }
}
