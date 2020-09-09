using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TaskSystemCli.DataCenter
{
    public class RolesText
    {
        private static readonly string Address = "DataCenter/RolesText.json";

        public string[] Text { get; set; }
        public string Roles { get; set; }
    
        public static async Task<List<RolesText>> GetRolesTextsAsync()
        {
            using StreamReader sr = new StreamReader(Address);
            return JsonConvert.DeserializeObject<List<RolesText>>(await sr.ReadToEndAsync());
        }
    }
}
