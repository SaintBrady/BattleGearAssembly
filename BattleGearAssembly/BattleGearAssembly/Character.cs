using Newtonsoft.Json;

namespace BattleGearAssembly
{
    public class Character
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public string Region { get; set; }

        [JsonProperty("realm")]
        public Realm Realm { get; set; }

        [JsonProperty("equipped_item_level")]
        public string Ilvl { get; set; }

        [JsonProperty("active_title")]
        public Title Title { get; set; }

        [JsonProperty("faction")]
        public Faction Faction { get; set; }

        [JsonProperty("character_class")]
        public CharacterClass Class { get; set; }

        [JsonProperty("active_spec")]
        public Specialization Spec { get; set; }

        public KeyProfile KeyProfile { get; set; }

        [JsonProperty("equipment")]
        public Href Equipment { get; set; }

        [JsonProperty("media")]
        public Href Media { get; set; }

        [JsonProperty("mythic_keystone_profile")]
        public Href MythicPlus { get; set; }
    }

    public class Realm
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public class Faction
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Title
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class CharacterClass
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public string getColor()
        {
            return API_Globals.ClassColors[Name];
        }
    }

    public class Href
    {
        [JsonProperty("href")]
        public string Url { get; set; }
    }
}
