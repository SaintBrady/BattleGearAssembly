using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGearAssembly
{
    public class Dungeon
    {
        [JsonProperty("dungeon")]
        public Info Info { get; set; }

        [JsonProperty("keystone_level")]
        public int Level { get; set; }
    }

    public class Info
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
