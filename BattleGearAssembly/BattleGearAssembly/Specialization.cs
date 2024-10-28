using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BattleGearAssembly
{
    public class Specialization
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("playable_class")]
        public CharacterClass Class { get; set; }

        [JsonProperty("role")]
        public Role Role { get; set; }
    }

    public class SpecRoot
    {
        [JsonProperty("character_specializations")]
        public Specialization[] Specializations { get; set; }
    }

    public class Role
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
