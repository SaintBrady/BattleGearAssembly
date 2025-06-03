using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

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

        public Dictionary<string, GearItem> Gear {  get; set; } = new Dictionary<string, GearItem>();

        public Dictionary<string, Dungeon> TopDungeons { get; set; } = new Dictionary<string, Dungeon>
        {
            { "525", null },
            { "506", null },
            { "504", null },
            { "500", null },
            { "499", null },
            { "382", null },
            { "370", null },
            { "247", null },
        };

        public KeyProfile KeyProfile { get; set; }

        [JsonProperty("equipment")]
        public Href Equipment { get; set; }

        [JsonProperty("media")]
        public Href Media { get; set; }

        [JsonProperty("mythic_keystone_profile")]
        public Href MythicPlus { get; set; }

        public SolidColorBrush getScoreColor()
        {
            return new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(KeyProfile.Rating.Color.GetColor()));
        }

        public float getMythicPluScore()
        {
            return KeyProfile.Rating.Value;
        }

        public void getTopDungeons()
        {
            KeyProfile.Dungeons = KeyProfile.Dungeons.Where(d => d.IsTimed).ToList();

            foreach (Dungeon d in KeyProfile.Dungeons)
            {
                TopDungeons[d.Info.Id] = d;
            }
        }
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

        public SolidColorBrush getColor()
        {
            return new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(API_Globals.ClassColors[Name]));
        }
    }

    public class KeyProfile
    {
        [JsonProperty("best_runs")]
        public List<Dungeon> Dungeons { get; set; }

        [JsonProperty("mythic_rating")]
        public Rating Rating { get; set; }
    }

    public class Href
    {
        [JsonProperty("href")]
        public string Url { get; set; }
    }
}
