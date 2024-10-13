using Newtonsoft.Json;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace BattleGearAssembly
{
    public class GearItem
    {
        [JsonProperty("item")]
        public ItemInfo ItemInfo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quality")]
        public Quality Quality { get; set; }

        [JsonProperty("name_description")]
        public Source Source { get; set; }

        [JsonProperty("level")]
        public Level Level { get; set; }

        [JsonProperty("transmog")]
        public Transmog Transmog { get; set; }

        [JsonProperty("binding")]
        public Binding Binding { get; set; }

        [JsonProperty("unique_equipped")]
        public string UniqueEquipped { get; set; }

        [JsonProperty("slot")]
        public Slot Slot { get; set; }

        [JsonProperty("inventory_type")]
        public InventoryType InventoryType { get; set; }

        [JsonProperty("item_subclass")]
        public ArmorClass ArmorClass { get; set; }

        [JsonProperty("weapon")]
        public Weapon Weapon { get; set; }

        [JsonProperty("stats")]
        public Stat[] Stats { get; set; }

        [JsonProperty("enchantments")]
        public Enchantment[] Enchantments { get; set; }

        [JsonProperty("sockets")]
        public Socket[] Sockets { get; set; }

        [JsonProperty("spells")]
        public Spell[] Spells { get; set; }

        [JsonProperty("set")]
        public ItemSet Set { get; set; }

        [JsonProperty("durability")]
        public Durability Durability { get; set; }

        [JsonProperty("playable_classes")]
        public PlayerClass PlayerClass { get; set; }

        [JsonProperty("requirements")]
        public Requirements Requirements { get; set; }

        [JsonProperty("sell_price")]
        public SellPrice SellPrice { get; set; }

        public ImageSource Image { get; set; }

        public static TextBlock ItemText(string[] textProperties)
        {
            int numProps = textProperties.Length;
            string color = numProps > 1 ? textProperties[1] : "#FFFFFF";
            string size = numProps > 2 ? textProperties[2] : "12";

            return new TextBlock
            {
                Text = textProperties[0],
                FontFamily = new FontFamily("sans-serif"),
                Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(color)),
                FontWeight = FontWeights.DemiBold,
                FontSize = Int32.Parse(size),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300
            };
        }
    }

    public class Root
    {
        [JsonProperty("equipped_items")]
        public GearItem[] GearItems { get; set; }
    }

    public class ItemInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }
    }

    public class Quality
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public class Level
    {
        [JsonProperty("value")]
        public int Ilvl { get; set; }
    }

    public class Transmog
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class Binding
    {
        [JsonProperty("name")]
        public string Type { get; set; }
    }

    public class Slot
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class InventoryType
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ArmorClass
    {
        [JsonProperty("name")]
        public string Class { get; set; }
    }

    public class Source
    {
        [JsonProperty("display_string")]
        public string Name { get; set; }
    }

    public class Weapon
    {
        [JsonProperty("damage")]
        public Damage Damage { get; set; }

        [JsonProperty("attack_speed")]
        public AttackSpeed AttackSpeed { get; set; }

        [JsonProperty("dps")]
        public DPS DPS { get; set; }
    }

    public class Damage
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class AttackSpeed
    {
        [JsonProperty("display_string")]
        public string Speed { get; set; }
    }

    public class DPS
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class Stat
    {
        [JsonProperty("type")]
        public StatType Type { get; set; }

        [JsonProperty("display")]
        public StatDisplay Display { get; set; }
    }

    public class StatType
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public class StatDisplay
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }

        [JsonProperty("color")]
        public DisplayColor Color { get; set; }
    }

    public class Enchantment
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class Socket
    {
        [JsonProperty("socket_type")]
        public SocketType SocketType { get; set; }

        [JsonProperty("item")]
        public Gem Gem { get; set; }

        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class SocketType
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public class Gem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Spell
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("display_color")]
        public DisplayColor Color { get; set; }
    }

    public class ItemSet
    {
        [JsonProperty("items")]
        public SetPiece[] SetPieces { get; set; }

        [JsonProperty("effects")]
        public SetEffect[] Effects { get; set; }

        [JsonProperty("display_string")]
        public string Count { get; set; }
    }

    public class SetPiece
    {
        [JsonProperty("item")]
        public ItemInfo ItemInfo { get; set; }

        [JsonProperty("is_equipped")]
        public bool IsEquipped { get; set; } = false;

        public string Color(bool IsEquipped)
        {
            if (IsEquipped) return "#FFFF99";
            else return "#808080";
        }
    }

    public class SetEffect
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }

        [JsonProperty("required_count")]
        public int RequiredCount { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; } = false;

        public string Color(bool IsActive)
        {
            if (IsActive) return "#00FF00";
            else return "#808080";
        }
    }

    public class Durability
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class PlayerClass
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class Requirements
    {
        [JsonProperty("level")]
        public LevelRequirement LevelRequirement { get; set; }
    }

    public class LevelRequirement
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public class SellPrice
    {
        [JsonProperty("display_strings")]
        public Cost Value { get; set; }
    }

    public class Cost
    {
        [JsonProperty("gold")]
        public string Gold { get; set; }

        [JsonProperty("silver")]
        public string Silver { get; set; }

        [JsonProperty("copper")]
        public string Copper { get; set; }
    }

    public class DisplayColor
    {
        [JsonProperty("r")]
        public int Red { get; set; }

        [JsonProperty("g")]
        public int Green { get; set; }

        [JsonProperty("b")]
        public int Blue { get; set; }

        public string GetColor()
        {
            return "#" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2");
        }
    }

}
