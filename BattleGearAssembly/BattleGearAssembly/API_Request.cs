using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using BattleGearAssembly;
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
        public ID ID { get; set; }

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
        public string UniqueEquipped {  get; set; }

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

        [JsonProperty("durability")]
        public Durability Durability { get; set; }

        [JsonProperty("requirements")]
        public Requirements Requirements { get; set; }

        [JsonProperty("sell_price")]
        public SellPrice SellPrice { get; set; }

        public BitmapImage Image { get; set; }

        public static TextBlock ItemText(string[] textProperties)
        {
            return new TextBlock
            {
                Text = textProperties[0],
                FontFamily = new FontFamily("Open Sans"),
                Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(textProperties[1])),
                FontWeight = textProperties[2] == "demibold" ? FontWeights.DemiBold : FontWeights.Regular,
                FontSize = Int32.Parse(textProperties[3]),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300
            };
        }
    }

    public partial class Root
    {
        [JsonProperty("equipped_items")]
        public GearItem[] GearItems { get; set; }
    }

    public partial class ID
    {
        [JsonProperty("id")]
        public int Value { get; set; }
    }

    public partial class Quality
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public partial class Level
    {
        [JsonProperty("value")]
        public int Ilvl { get; set; }
    }

    public partial class Transmog
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class Binding
    {
        [JsonProperty("name")]
        public string Type { get; set; }
    }

    public partial class Slot
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class InventoryType
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class ArmorClass
    {
        [JsonProperty("name")]
        public string Class { get; set; }
    }

    public partial class Source
    {
        [JsonProperty("display_string")]
        public string Name { get; set; }
    }

    public partial class Weapon
    {
        [JsonProperty("damage")]
        public Damage Damage { get; set; }

        [JsonProperty("attack_speed")]
        public AttackSpeed AttackSpeed { get; set; }

        [JsonProperty("dps")]
        public DPS DPS { get; set; }
    }

    public partial class Damage
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class AttackSpeed
    {
        [JsonProperty("display_string")]
        public string Speed { get; set; }
    }

    public partial class DPS
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("type")]
        public StatType Type { get; set; }

        [JsonProperty("display")]
        public StatDisplay Display { get; set; }
    }

    public partial class StatType
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public partial class StatDisplay
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }

        [JsonProperty("color")]
        public DisplayColor Color { get; set; }
    }

    public partial class Enchantment
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class Socket
    {
        [JsonProperty("socket_type")]
        public SocketType SocketType { get; set; }

        [JsonProperty("media")]
        public ID Media { get; set; }

        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class SocketType
    {
        [JsonProperty("type")]
        public string Value { get; set; }
    }

    public partial class Spell
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("display_color")]
        public DisplayColor Color { get; set; }
    }

    public partial class Durability
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class Requirements
    {
        [JsonProperty("level")]
        public LevelRequirement LevelRequirement { get; set; }
    }

    public partial class LevelRequirement
    {
        [JsonProperty("display_string")]
        public string Value { get; set; }
    }

    public partial class SellPrice
    {
        [JsonProperty("display_strings")]
        public Cost Value { get; set; }
    }

    public partial class Cost
    {
        [JsonProperty("gold")]
        public string Gold { get; set; }

        [JsonProperty("silver")]
        public string Silver { get; set; }

        [JsonProperty("copper")]
        public string Copper { get; set; }
    }

    public partial class DisplayColor
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
        // Generate API Token
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
        public static async Task<BitmapImage> API_LoadImage(string token, int item_id = 19019, string region = "us")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "static-us" }, { "locale", "en_US" } };
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
        public static async Task API_LoadGear(string token, string region = "us", string realmSlug = "thrall", string characterName = "euphrelia")
        {
            realmSlug = API_Globals.RealmSlugDict[realmSlug];
            var parameters = new Dictionary<string, string> { { "namespace", "profile-us" }, { "locale", "en_US" } };
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
            Console.WriteLine(jsonGearList);

            foreach (GearItem gearItem in root.GearItems)
            {
                if (isTwoHanded) continue;

                //ID.Value = OK
                //Name = OK
                //Quality.Value = OK
                //Source = OK
                //Level.Ilvl = OK
                //Transmog = OK
                //Binding.Type = OK
                //Unique Equipped = OK
                //gearItem.InventoryType.Name = OK
                //ArmorClass.Class = OK
                //Damage Range = OK
                //Weapon.AttackSpeed.Speed = OK
                //DPS = OK
                //Stats = OK
                //Enchants = OK
                //Sharpening/Oils = OK
                //Gems = OK
                //Equip Effect = OK
                //On use Name = OK
                //On use effect = OK
                //CD = OK
                //!!!// Set Bonus
                //Durability = OK
                //Requirements = OK
                //!!!//Sell_Price = item["sell_price"]["display_strings"]

                gearItem.Image = await API_LoadImage(API_Globals.API_Token, gearItem.ID.Value);

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
        public static async Task<List<string>> API_LoadRealms(string token, string region = "us")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "dynamic-us" }, { "locale", "en_US" } };
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
        public static async Task<BitmapImage> API_LoadPlayerMedia(string token, string region = "us", string realmSlug = "thrall", string characterName = "euphrelia")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "profile-us" }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/profile/wow/character/{realmSlug}/{characterName}/character-media?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            return API_ParsePlayerMedia(responseBody);
        }

        public static BitmapImage API_ParsePlayerMedia(string mediaString)
        {
            dynamic d = JObject.Parse(mediaString);
            // Assets[2]["value"] or Assets[where key = main-raw]["value"]
            string imageURL = d.assets[2]["value"].ToString();

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@imageURL, UriKind.Absolute);
            image.EndInit();

            return image;
        }

        public static async Task<string> API_LoadItem(string token, string region = "us", string item_id = "19019")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "static-us" }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/item/{item_id}?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            return responseBody;
        }

        public static BitmapImage API_PaseItem(string mediaString)
        {
            dynamic d = JObject.Parse(mediaString);
            // Assets[2]["value"] or Assets[where key = main-raw]["value"]
            string imageURL = d.assets[2]["value"].ToString();

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@imageURL, UriKind.Absolute);
            image.EndInit();

            return image;
        }

        public static Image RenderImage(string mediaString)
        {
            Image myImage = new Image();
            myImage.Width = 10;

            // Create source
            BitmapImage myBitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri("pack://application:,,,/" + mediaString);
            myBitmapImage.DecodePixelWidth = 10;
            myBitmapImage.EndInit();

            myImage.Source = myBitmapImage;

            return myImage;
        }
    }
}
