using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        [JsonProperty]
        internal int ID { get; set; }

        [JsonProperty]
        internal int Ilvl { get; set; }

        [JsonProperty]
        internal string Binding { get; set; }

        [JsonProperty]
        internal string Slot { get; set; }

        [JsonProperty]
        internal string Quality { get; set; }

        [JsonProperty]
        internal string Name { get; set; }

        [JsonProperty]
        internal BitmapImage Image { get; set; }
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
            dynamic d = JObject.Parse(jsonGearList);
            API_Globals.Player_Ilvl = 0;
            bool isTwoHanded = false;

            // Eventually need to add additional stats. Will need to rework calls to make more efficient
            foreach(dynamic item in d.equipped_items)
            {
                if (isTwoHanded) continue;

                GearItem gearItem = new GearItem()
                {
                    ID = item["item"]["id"],
                    Slot = item["slot"]["type"],
                    Ilvl = item["level"]["value"],
                    Quality = item["quality"]["type"],
                    Name = item["name"],
                };
                gearItem.Image = await API_LoadImage(API_Globals.API_Token, gearItem.ID);

                if (gearItem.Slot.Equals("MAIN_HAND") && item["inventory_type"]["type"] == "TWOHWEAPON")
                {
                    API_Globals.Player_Ilvl += gearItem.Ilvl;
                    isTwoHanded = true;
                }

                API_Globals.Gear[gearItem.Slot] = gearItem;
                if (gearItem.Slot.Equals("SHIRT") || gearItem.Slot.Equals("TABARD")) { continue; }
                API_Globals.Player_Ilvl += gearItem.Ilvl;
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

            foreach(dynamic realm in d.realms) {
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
    }
}
