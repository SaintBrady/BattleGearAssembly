using Newtonsoft.Json;
using System;

namespace BattleGearAssembly
{
    public class Dungeon
    {
        [JsonProperty("dungeon")]
        public DungeonName Name { get; set; }

        [JsonProperty("keystone_level")]
        public int Level { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("members")]
        public DungeonCharacter[] Characters { get; set; }

        [JsonProperty("map_rating")]
        public Rating Rating { get; set; }

        public string GetTime()
        {
            double dur = Duration / 1000.0;
            Console.WriteLine("Duration: " + dur);
            
            double seconds = dur % 60;
            Console.WriteLine("Seconds: " + seconds);
            int minutes = (int)(dur / 60);

            return $"{minutes}:{seconds:00.000}";
        }
    }

    public class KeyProfile
    {
        [JsonProperty("best_runs")]
        public Dungeon[] Dungeons { get; set; }

        [JsonProperty("mythic_rating")]
        public Rating Rating { get; set; }
    }

    public class DungeonName
    {
        [JsonProperty("name")]
        public string Value { get; set; }
    }

    public class Rating
    {
        [JsonProperty("rating")]
        public float Value { get; set; }

        [JsonProperty("color")]
        public DisplayColor Color { get; set; }
    }

    public class DungeonCharacter
    {
        [JsonProperty("character")]
        public CharacterInfo Info { get; set; }

        [JsonProperty("specialization")]
        public Specialization Spec {  get; set; }
    }

    public class CharacterInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("realm")]
        public Realm Realm { get; set; }
    }
}
