using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BattleGearAssembly
{
    public static class API_Globals
    {
        public static HttpClient client = new HttpClient();
        public static string API_Token = "";
        public static Character character = new Character();

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

    public class API_Request
    {
        public static async Task<string> BuildHttpRequest(string token, string httpMessage)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpMessage);

            request.Headers.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await API_Globals.client.SendAsync(request);
            response.EnsureSuccessStatusCode(); //!!!// THROWS 404 ON CHARACTERS WITHOUT A KEY COMPLETED ON THE CURRENT SEASON //!!!//
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        // Generates API Token
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

        // Gets Player Data for Given Character Profile
        public static async Task<Character> LoadCharacterProfile(string token, string region, string realmSlug, string characterName)
        {
            realmSlug = API_Globals.RealmSlugDict[realmSlug];
            var parameters = new Dictionary<string, string> { { "namespace", "profile-" + region }, { "locale", "en_us" } };
            string httpMessage = $"https://{region}.api.blizzard.com/profile/wow/character/{realmSlug}/{characterName}?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            return JsonConvert.DeserializeObject<Character>(responseBody);
        }

        // Gets Character Gear JSON from API Request and Converts to Gear List
        public static async Task LoadGear(string token, string url)
        {
            string responseBody = await BuildHttpRequest(token, url + "&locale=en_us");

            Root root = JsonConvert.DeserializeObject<Root>(responseBody);

            foreach (GearItem gearItem in root.GearItems)
            {
                gearItem.Image = await LoadImage(API_Globals.API_Token, gearItem.ItemInfo.ID);
                API_Globals.Gear[gearItem.Slot.Type] = gearItem;
            }
        }

        // Gets Realms from given Region
        public static async Task<List<string>> LoadRealms(string token, string region)
        {
            var parameters = new Dictionary<string, string> { { "namespace", "dynamic-" + region }, { "locale", "en_us" } };
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/realm/index?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            dynamic d = JObject.Parse(responseBody);
            List<string> realmNames = new List<string>();

            foreach (dynamic realm in d.realms)
            {
                API_Globals.RealmSlugDict[realm["name"].ToString()] = realm["slug"].ToString();
                realmNames.Add(realm["name"].ToString());
            }

            realmNames.Sort();
            return realmNames;
        }

        public static async Task LoadMythicPlus(string token)
        {
            int index = API_Globals.character.MythicPlus.Url.IndexOf('?');
            string url = API_Globals.character.MythicPlus.Url.Insert(index, "/season/13") + "&locale=en_us";
            string responseBody = await BuildHttpRequest(token, url);
            API_Globals.character.KeyProfile = JsonConvert.DeserializeObject<KeyProfile>(responseBody);

            // Sorts so that highest keys are pulled into dict for MythicPlus.xaml
            API_Globals.character.KeyProfile.Dungeons = API_Globals.character.KeyProfile.Dungeons.OrderBy(c => c.Level).ToArray();
            Array.Reverse(API_Globals.character.KeyProfile.Dungeons);
        }

        // Gets Character Media JSON from API Request
        public static async Task<ImageSource> LoadPlayerMedia(string token, string url)
        {
            string responseBody = await BuildHttpRequest(token, url);

            dynamic d = JObject.Parse(responseBody);
            string imageURL = d.assets[2]["value"].ToString();
            return RenderImage(imageURL, 800, 800, "");
        }

        // Gets Image From API Source using item_id
        public static async Task<ImageSource> LoadImage(string token, int item_id, string region = "us")
        {
            var parameters = new Dictionary<string, string> { { "namespace", "static-" + region }, { "locale", "en_US" } };
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/media/item/{item_id}?namespace={parameters["namespace"]}&locale={parameters["locale"]}".ToLower();
            string responseBody = await BuildHttpRequest(token, httpMessage);

            string imageURL = JObject.Parse(responseBody)["assets"][0]["value"].ToString();

            return RenderImage(imageURL, 50, 50, "");
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
