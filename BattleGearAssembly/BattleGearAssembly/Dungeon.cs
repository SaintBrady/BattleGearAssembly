﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace BattleGearAssembly
{
    public class Dungeon
    {
        [JsonProperty("dungeon")]
        public DungeonInfo Info { get; set; }

        [JsonProperty("keystone_level")]
        public int Level { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("is_completed_within_time")]
        public bool IsTimed { get; set; }

        [JsonProperty("keystone_upgrades")]
        public KeystoneUpgrade[] KeystoneUpgrades { get; set; }

        [JsonProperty("members")]
        public DungeonCharacter[] Characters { get; set; }

        [JsonProperty("map_rating")]
        public Rating Rating { get; set; }

        public string GetTime()
        {
            double dur = Duration / 1000.0;
            int minutes = (int)(dur / 60);
            double seconds = dur - (minutes * 60);

            return $"{minutes}:{seconds:00.000}";
        }

        public SolidColorBrush getLevelColor()
        {
            return new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(Rating.Color.GetColor()));
        }
    }

    public class DungeonRoot
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("keystone_upgrades")]
        public KeystoneUpgrade[] KeystoneUpgrades { get; set; }
    }

    public class DungeonInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Rating
    {
        [JsonProperty("rating")]
        public float Value { get; set; }

        [JsonProperty("color")]
        public DisplayColor Color { get; set; }
    }

    public class KeystoneUpgrade
    {
        [JsonProperty("upgrade_level")]
        public int Level { get; set; }

        [JsonProperty("qualifying_duration")]
        public int Duration { get; set; }
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
