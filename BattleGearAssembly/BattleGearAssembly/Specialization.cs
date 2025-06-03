using Newtonsoft.Json;

namespace BattleGearAssembly
{
    public class Specialization
    {
        [JsonProperty("name")]
        public string Name { get; set; } //Resolves Blizzard's ingenious new embedded localization JSON properties

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
