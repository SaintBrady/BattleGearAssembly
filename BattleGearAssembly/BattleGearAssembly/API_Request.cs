using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BattleGearAssembly
{
    public static class API_Globals
    {
        public static HttpClient client = new HttpClient();
        public static string API_Token = "";
        public static int Player_Ilvl = 0;

        public static Dictionary<string, GearItem> Gear = new Dictionary<string, GearItem>();
        public static Dictionary<string, string> RealmSlugDict = new Dictionary<string, string>();
        public static Dictionary<string, string> QualityColors = new Dictionary<string, string>
        {
            {"POOR", "#9D9D9D"},
            {"COMMON", "#FFFFFF"},
            {"UNCOMMON", "#1EFF00"},
            {"RARE", "#0070DD"},
            {"EPIC", "#A335EE"},
            {"LEGENDARY", "#FF8000"},
            {"ARTIFACT", "#E6CC80"},
            {"HEIRLOOM", "#00CCFF"}
        };
    }

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

        public BitmapImage Image { get; set; }

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
            if(IsEquipped) return "#FFFF99";
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

    public class API_Request
    {
        // Generates API Token
        public static async Task<string> BuildHttpRequest(string token, string httpMessage)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpMessage);

            request.Headers.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await API_Globals.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        public static async Task<string> RequestAsync()
        {
            string client_id = tokens.client_id;
            string client_secret = tokens.client_secret;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.battle.net/token");

            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{client_id}:{client_secret}")));
            request.Content = new StringContent("grant_type=client_credentials");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await API_Globals.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string token = JObject.Parse(responseBody)["access_token"].ToString();

            API_Globals.API_Token = token;
            return token;
        }

        // Gets Image From API Source using item_id
        public static async Task<BitmapImage> API_LoadImage(string token, int item_id, string region = "us")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "static-" + region }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/media/item/{item_id}?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            string imageURL = JObject.Parse(responseBody)["assets"][0]["value"].ToString();

            // Build image off of URL
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@imageURL, UriKind.Absolute);
            image.EndInit();

            return image;
        }

        // Gets Character Gear JSON from API Request
        public static async Task API_LoadGear(string token, string region, string realmSlug, string characterName)
        {
            realmSlug = API_Globals.RealmSlugDict[realmSlug];
            var parameters = new Dictionary<string, string> { { "namespace", "profile-" + region }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/profile/wow/character/{realmSlug}/{characterName}/equipment?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            await API_ParseGear(responseBody);
        }

        // Parses JSON Data into workable gear list in API_Globals
        private static async Task API_ParseGear(string jsonGearList)
        {
            Root root = JsonConvert.DeserializeObject<Root>(jsonGearList);
            API_Globals.Player_Ilvl = 0;
            bool isTwoHanded = false;
            //Console.WriteLine(jsonGearList);

            foreach (GearItem gearItem in root.GearItems)
            {
                if (isTwoHanded) continue;

                gearItem.Image = await API_LoadImage(API_Globals.API_Token, gearItem.ItemInfo.ID);

                if (gearItem.InventoryType.Type == "TWOHWEAPON")
                {
                    API_Globals.Player_Ilvl += gearItem.Level.Ilvl;
                    isTwoHanded = true;
                }

                API_Globals.Gear[gearItem.Slot.Type] = gearItem;
                if (gearItem.Slot.Type == "SHIRT" || gearItem.Slot.Type == "TABARD") { continue; }
                API_Globals.Player_Ilvl += gearItem.Level.Ilvl;
            }

            API_Globals.Player_Ilvl /= 16;
        }

        // Gets Realm JSON from API Request
        public static async Task<List<string>> API_LoadRealms(string token, string region)
        {
            var parameters = new Dictionary<string, string> { { "namespace", "dynamic-" + region }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/realm/index?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            return API_ParseRealms(responseBody);
        }

        public static List<string> API_ParseRealms(string jsonRealmList)
        {
            dynamic d = JObject.Parse(jsonRealmList);
            List<string> realmNames = new List<string>();

            foreach (dynamic realm in d.realms)
            {
                API_Globals.RealmSlugDict[realm["name"].ToString()] = realm["slug"].ToString();
                realmNames.Add(realm["name"].ToString());
            }

            realmNames.Sort();
            return realmNames;
        }

        // Gets Character Media JSON from API Request
        public static async Task<ImageSource> API_LoadPlayerMedia(string token, string region, string realmSlug, string characterName)
        {
            var parameters = new Dictionary<string, string> { { "namespace", "profile-" + region }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/profile/wow/character/{realmSlug}/{characterName}/character-media?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            dynamic d = JObject.Parse(responseBody);
            string imageURL = d.assets[2]["value"].ToString();
            return RenderImage(imageURL, 800, 800, "");
        }

        // Renders Image at given dimensions
        public static ImageSource RenderImage(string mediaString, int width, int height, string prefix = "pack://application:,,,/")
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(prefix + mediaString);
            image.DecodePixelWidth = width;
            image.DecodePixelHeight = height;
            image.EndInit();

            return image;
        }
    }
}
