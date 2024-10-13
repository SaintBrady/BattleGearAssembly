using Newtonsoft.Json;

namespace BattleGearAssembly
{
    public class Character
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("equipped_item_level")]
        public string Ilvl { get; set; }

        [JsonProperty("equipment")]
        public Href Equipment { get; set; }
    }

    public class Href
    {
        [JsonProperty("href")]
        public string Url { get; set; }
    }
}
