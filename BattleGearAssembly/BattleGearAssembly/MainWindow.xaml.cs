using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

/* TODO
 * ADD ENCHANTS TO GEAR
 * RENDER BOX FOR GEAR ITEM ON HOVER
 * HYPERLINKS FOR CLICKING NAME AND GEAR ITEMS
 * ADDITIONAL TABS FOR IO SCORE, ETC
*/

namespace BattleGearAssembly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public static class Globals
    {
        public static string API_TOKEN = "";
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetAPIToken();
            LoadRealms();
        }

        private async Task GetAPIToken()
        {
            Globals.API_TOKEN = await API_Request.RequestAsync();
        }

        private async void LoadRealms()
        {
            List<string> realmList = await API_Request.API_LoadRealms(Globals.API_TOKEN);
            foreach (string realm in realmList)
            {
                RealmCB.Items.Add(realm);
            }
        }

        private async void Button_LoadGear(object sender, RoutedEventArgs e)
        {
            API_Globals.Gear.Clear();
            Dictionary<string, GearItem> Gear = API_Globals.Gear;

            //Console.Write(await API_Request.API_LoadItem(API_Globals.API_Token));

            try
            {
                await API_Request.API_LoadGear(Globals.API_TOKEN, RegionCB.Text, RealmCB.Text, CharacterNameBox.Text);

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = await API_Request.API_LoadPlayerMedia(Globals.API_TOKEN, RegionCB.Text, RealmCB.Text, CharacterNameBox.Text);
                brush.Stretch = Stretch.UniformToFill;
                MainPanel.Background = brush;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught Exception: " + ex);
                MessageBox.Show("Error: Failed to load player profile data");
                return;
            }

            string tempName = CharacterNameBox.Text;
            string retName = tempName[0].ToString().ToUpper() + tempName.Substring(1, tempName.Length - 1).ToLower();

            CHARACTER_NAME.Text = retName;
            CHARACTER_ILVL.Text = "Item Level " + API_Globals.Player_Ilvl.ToString();

            foreach (Grid g in MainPanel.Children)
            {
                Image image = g.Children.OfType<Image>().First();
                StackPanel sp = g.Children.OfType<StackPanel>().First();
                TextBlock itemName = sp.Children.OfType<TextBlock>().ElementAt(0);
                TextBlock ilvl = sp.Children.OfType<TextBlock>().ElementAt(1);
                Border border = g.Children.OfType<Border>().First();

                try
                {
                    string qualityColorHex = API_Globals.QualityColors[Gear[g.Name].Quality.Value];
                    SolidColorBrush itemColor = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(qualityColorHex));

                    image.Source = Gear[g.Name].Image;
                    ilvl.Text = Gear[g.Name].Level.Ilvl.ToString();
                    itemName.Text = Gear[g.Name].Name;
                    itemName.Foreground = itemColor;
                    border.BorderBrush = itemColor;
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    LoadEmptySlot(image, sp, itemName, ilvl, border, g);
                }
            }
        }

        private void LoadEmptySlot(Image image, StackPanel sp, TextBlock itemName, TextBlock ilvl, Border border, Grid g)
        {
            Dictionary<string, GearItem> Gear = API_Globals.Gear;

            string gearSlotName = g.Name;
            if(gearSlotName.Contains("_1") || gearSlotName.Contains("_2")) { gearSlotName = gearSlotName.Substring(0, gearSlotName.Length - 2); }

            BitmapImage imageBlank = new BitmapImage();
            string imgString = "ImageResources/ItemBlanks/" + gearSlotName + "blank.png";
            imageBlank.BeginInit();
            imageBlank.UriSource = new Uri(@imgString, UriKind.Relative);
            imageBlank.EndInit();

            image.Source = imageBlank;
            ilvl.Text = "";
            itemName.Text = "";
            itemName.Foreground = Brushes.Black;
            border.BorderBrush = Brushes.Black;
        }

        private void ToggleShowGearTT(object sender, EventArgs e)
        {
            Grid g = sender as Grid;
            Console.WriteLine("Sender: " + g.Name);

            if(CreateGearWindow(g.Name) != 0) return;
            GearToolTip.Visibility = GearToolTip.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private int CreateGearWindow(string slot)
        {
            // Handles empty gear slot hover
            if (!API_Globals.Gear.TryGetValue(slot, out GearItem item)) return -1;

            GearInfoPanel.Children.Clear();

            Dictionary<string, string[]> textBlockData = new Dictionary<string, string[]> // Eventually move keys to function that allows default args
            {
                { "Durability", new string[] { item.Durability == null ? "" : item.Durability.Value, "#FFFFFF", "regular", "12" } },
                { "Enchantments", new string[] { item.Enchantments == null ? "" : item.Enchantments[0].Value, "#00FF00", "regular", "12" } },
                { "Name", new string[] { item.Name, API_Globals.QualityColors[item.Quality.Value], "demibold", "16" } },
                { "Level", new string[] { "Item Level " + item.Level.Ilvl.ToString(), "#EEEE00", "regular", "12" } },
                { "Binding", new string[] { item.Binding.Type, "#FFFFFF", "regular", "12" } },
                { "InventoryType", new string[] { item.InventoryType.Name, "#FFFFFF", "regular", "12" } },
                { "ArmorClass", new string[] { item.ArmorClass.Class, "#FFFFFF", "regular", "12" } },
                { "Requirements", new string[] { item.Requirements == null ? "" : item.Requirements.LevelRequirement.Value, "#FFFFFF", "regular", "12" } },
                { "Stats", new string[] { item.Stats[0].Display.Value, "#FFFFFF", "regular", "12" } },
                { "Sockets", new string[] { item.Sockets == null ? "" : item.Sockets[0].Value, "#FFFFFF", "regular", "12" } },
                { "Source", new string[] { item.Source == null ? "" : item.Source.Name, "#00FF00", "regular", "12" } },
                { "Spells", new string[] { item.Spells == null ? "" : item.Spells[0].Description, "#FFFFFF", "regular", "12" } },
                { "Transmog", new string[] { item.Transmog == null ? "" : item.Transmog.Value, "#FF99FF", "regular", "12" } },
                { "UniqueEquipped", new string[] { item.UniqueEquipped, "#FFFFFF", "regular", "12" } },
                { "Weapon", new string[] { item.Weapon == null ? "" : item.Weapon.Damage.Value, "#FFFFFF", "regular", "12" } },
                { "Speed", new string[] { item.Weapon == null ? "" : item.Weapon.AttackSpeed.Speed, "#FFFFFF", "regular", "12" } },
                { "DPS", new string[] { item.Weapon == null ? "" : item.Weapon.DPS.Value, "#FFFFFF", "regular", "12" } },
            };

            PropertyInfo[] properties = item.GetType().GetProperties();

            string[] armorTypeExclusions = { "NECK", "CLOAK", "FINGER", "TRINKET" };
            Grid g;

            foreach (PropertyInfo property in properties)
            {
                // Ignores properties not set on item i.e. Weapon on non-weapon items
                if (!textBlockData.ContainsKey(property.Name)) { continue; }
                if (property.GetValue(item, null) == null) { continue; }

                TextBlock t = GearItem.ItemText(textBlockData[property.Name]);
                g = new Grid();

                switch (property.Name)
                {
                    case "InventoryType":
                        goto case "ArmorClass";

                    case "ArmorClass":
                        if (property.Name == "ArmorClass") break;
                        g.Children.Add(t);
                        if (!armorTypeExclusions.Contains(item.InventoryType.Type))
                        {
                            t = GearItem.ItemText(textBlockData["ArmorClass"]);
                            t.HorizontalAlignment = HorizontalAlignment.Right;
                            g.Children.Add(t);
                        }

                        GearInfoPanel.Children.Add(g);
                        break;

                    case "Weapon":
                        g.Children.Add(t);
                        t = GearItem.ItemText(textBlockData["Speed"]);
                        t.HorizontalAlignment = HorizontalAlignment.Right;
                        g.Children.Add(t);
                        GearInfoPanel.Children.Add(g);
                        GearInfoPanel.Children.Add(GearItem.ItemText(textBlockData["DPS"]));
                        
                        break;

                    case "Stats":
                        for (int i = 0; i < item.Stats.Length; i++)
                        {
                            t = GearItem.ItemText(new string[] { item.Stats[i].Display.Value, item.Stats[i].Display.Color.GetColor(), "regular", "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Enchantments":
                        for (int i = 0; i < item.Enchantments.Length; i++)
                        {
                            string s = item.Enchantments[i].Value;
                            t = GearItem.ItemText(new string[] { s.Substring(0, s.IndexOf(" |A") < 0 ? s.Length : s.IndexOf(" |A")), "#00FF00", "regular", "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Sockets":
                        for (int i = 0; i < item.Sockets.Length; i++)
                        {
                            string s = item.Sockets[i].Value;
                            t = GearItem.ItemText(new string[] { item.Sockets[i].Value, "#FFFFFF", "regular", "12" });
                            //g.Children.Add(API_Request.RenderImage("ImageResources/Gems/" + item.Sockets[i].Media.Value.ToString() + ".png"));
                            //g.Children.Add(t);
                            GearInfoPanel.Children.Add(t);
                        }
                        break;

                    case "Spells":
                        for (int i = 0; i < item.Spells.Length; i++)
                        {
                            string color = item.Spells[i].Color == null ? "#00FF00" : item.Spells[i].Color.GetColor();
                            t = GearItem.ItemText(new string[] { item.Spells[i].Description, color, "regular", "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        break;

                    default:
                        GearInfoPanel.Children.Add(t);
                        break;
                }
            }  

            return 0;
        }
    }
}
