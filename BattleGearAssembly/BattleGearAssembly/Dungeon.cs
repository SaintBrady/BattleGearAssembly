using Newtonsoft.Json;

namespace BattleGearAssembly
{
    public class Dungeon
    {
        [JsonProperty("dungeon")]
        public Info Info { get; set; }

        [JsonProperty("keystone_level")]
        public int Level { get; set; }

        [JsonProperty("map_rating")]
        public Rating Rating { get; set; }
    }

    public class KeyProfile
    {
        [JsonProperty("best_runs")]
        public Dungeon[] Dungeons { get; set; }

        [JsonProperty("mythic_rating")]
        public Rating Rating { get; set; }
    }

    public class Info
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Rating
    {
        [JsonProperty("rating")]
        public float Value { get; set; }

        [JsonProperty("color")]
        public DisplayColor Color { get; set; }
    }
}
