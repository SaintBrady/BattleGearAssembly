using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Switch to using System.Text.Json?

namespace BattleGearAssembly
{
    public static class API_Globals
    {
        public static HttpClient client = new HttpClient();
        public static Character character = new Character();
        public static Dictionary<string, DungeonRoot> dungeonsList = new Dictionary<string, DungeonRoot>();

        public static string API_Token;
        public const int MythicPlusSeason = 14;

        public static Dictionary<string, string> RealmSlugDict = new Dictionary<string, string>();
        public static Dictionary<string, Specialization> SpecDict = new Dictionary<string, Specialization>();
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
        public static Dictionary<string, string> ClassColors = new Dictionary<string, string>
        {
            {"Death Knight", "#C41E3A"},
            {"Demon Hunter", "#A330C9"},
            {"Druid", "#FF7C0A"},
            {"Evoker", "#33937F"},
            {"Hunter", "#33937F"},
            {"Mage", "#3FC7EB"},
            {"Monk", "#00FF98"},
            {"Paladin", "#F48CBA"},
            {"Priest", "#FFFFFF"},
            {"Rogue", "#FFF468"},
            {"Shaman", "#0070DD"},
            {"Warlock", "#8788EE"},
            {"Warrior", "#C69B6D"}
        };
    }

    public class API_Request
    {
        public static async Task<string> BuildHttpRequest(string httpMessage)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpMessage);

            request.Headers.Add("Authorization", $"Bearer {API_Globals.API_Token}");

            HttpResponseMessage response = await API_Globals.client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // Generates API Token
        public static async Task RequestAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.battle.net/token");

            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{tokens.client_id}:{tokens.client_secret}")));
            request.Content = new StringContent("grant_type=client_credentials");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await API_Globals.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            API_Globals.API_Token = JObject.Parse(responseBody)["access_token"].ToString();
        }

        // Gets Player Data for Given Character Profile
        public static async Task LoadCharacterProfile(string region, string realmSlug, string characterName)
        {
            if(char.IsUpper(realmSlug[0])) realmSlug = API_Globals.RealmSlugDict[realmSlug]; // Returns slug if name, otherwise skips
            string httpMessage = $"https://{region}.api.blizzard.com/profile/wow/character/{realmSlug}/{characterName}?namespace=profile-{region}&locale=en_us".ToLower();
            string responseBody = await BuildHttpRequest(httpMessage);

            API_Globals.character = JsonConvert.DeserializeObject<Character>(responseBody);
            API_Globals.character.Region = region;

            await LoadGear();
        }

        // Gets Character Gear JSON from API Request and Converts to Gear List
        public static async Task LoadGear()
        {
            string responseBody = await BuildHttpRequest(API_Globals.character.Equipment.Url + "&locale=en_us");

            GearRoot root = JsonConvert.DeserializeObject<GearRoot>(responseBody);

            foreach (GearItem gearItem in root.GearItems)
            {
                gearItem.Image = await LoadImage(gearItem.ItemInfo.ID);
                API_Globals.character.Gear.Add(gearItem.Slot.Type, gearItem);
            }

            await LoadMythicPlus();
        }

        // Gets Realms from given Region
        public static async Task<List<string>> LoadRealms(string region)
        {
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/realm/index?namespace=dynamic-{region}&locale=en_us".ToLower();
            string responseBody = await BuildHttpRequest(httpMessage);

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

        public static async Task LoadMythicPlus()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            // Resolves players with no keys done for the current season
            try
            {
                Character character = API_Globals.character;
                string httpMessage = $"https://{character.Region}.api.blizzard.com/profile/wow/character/{character.Realm.Slug}/{character.Name}/mythic-keystone-profile/season/{API_Globals.MythicPlusSeason}?namespace=profile-{character.Region}&locale=en_us".ToLower();
                string responseBody = await BuildHttpRequest(httpMessage);

                // Sorts so that highest keys are pulled into dict for MythicPlus.xaml
                character.KeyProfile = JsonConvert.DeserializeObject<KeyProfile>(responseBody);
                character.getTopDungeons();

                foreach (KeyValuePair<string, Dungeon> kv in character.TopDungeons)
                {
                    if (kv.Value == null) continue;
                    kv.Value.KeystoneUpgrades = API_Globals.dungeonsList[kv.Key].KeystoneUpgrades;
                }

                mainWindow.MythicPlus.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                mainWindow.MythicPlus.IsEnabled = false;
            }
        }

        // Gets Character Media JSON from API Request
        public static async Task<ImageSource> LoadPlayerMedia(string url)
        {
            string responseBody = await BuildHttpRequest(url);

            dynamic d = JObject.Parse(responseBody);
            string imageURL = d.assets[2]["value"].ToString();
            return RenderImage(imageURL, 800, 800, "");
        }

        // Gets Image From API Source using item_id
        public static async Task<ImageSource> LoadImage(int item_id, string region = "us")
        {
            string httpMessage = $"https://{region}.api.blizzard.com/data/wow/media/item/{item_id}?namespace=static-{region}&locale=en_us".ToLower();
            string responseBody = await BuildHttpRequest(httpMessage);

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

        public static async Task LoadSpecs()
        {
            string httpMessage = $"https://us.api.blizzard.com/data/wow/playable-specialization/index?namespace=static-us&locale=en_us".ToLower();
            string responseBody = await BuildHttpRequest(httpMessage);

            SpecRoot sr = JsonConvert.DeserializeObject<SpecRoot>(responseBody);

            foreach (Specialization spec in sr.Specializations)
            {
                string httpMessage2 = $"https://us.api.blizzard.com/data/wow/playable-specialization/{spec.Id}?namespace=static-us&locale=en_US".ToLower();
                string responseBody2 = await BuildHttpRequest(httpMessage2);

                Specialization specParse = JsonConvert.DeserializeObject<Specialization>(responseBody2);
                API_Globals.SpecDict.Add(spec.Id, specParse);
            }
        }

        public static async Task LoadDungeons()
        {
            string[] dungeonIDs = { "525", "506", "504", "500", "499", "382", "370", "247" };

            for (int i = 0; i < 8; i++)
            {
                string dungeonJson = await BuildHttpRequest($"https://us.api.blizzard.com/data/wow/mythic-keystone/dungeon/{dungeonIDs[i]}?namespace=dynamic-us&locale=en_us");
                API_Globals.dungeonsList.Add(dungeonIDs[i], JsonConvert.DeserializeObject<DungeonRoot>(dungeonJson));
            }
        }

        private static async void Test()
        {
            var parameters = new Dictionary<string, string> { { "namespace", "dynamic-us" }, { "locale", "en_US" } };
            string httpMessage = $"https://us.api.blizzard.com/profile/wow/character/thrall/Tool/mythic-keystone-profile/season/{API_Globals.MythicPlusSeason}?namespace=profile-us&locale=en_us".ToLower();


            string responseBody = await BuildHttpRequest(httpMessage);
            Console.WriteLine(responseBody);
        }
    }
}
