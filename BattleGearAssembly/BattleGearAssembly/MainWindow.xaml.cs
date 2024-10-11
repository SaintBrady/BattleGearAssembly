﻿using Newtonsoft.Json.Serialization;
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

        private async void LoadRealms(object sender = null, RoutedEventArgs e = null)
        {
            RealmCB.Items.Clear();
            List<string> realmList = await API_Request.API_LoadRealms(Globals.API_TOKEN, RegionCB.Text);
            foreach (string realm in realmList)
            {
                RealmCB.Items.Add(realm);
            }
        }

        private async void Button_LoadGear(object sender, RoutedEventArgs e)
        {
            API_Globals.Gear.Clear();
            Dictionary<string, GearItem> Gear = API_Globals.Gear;

            try
            {
                await API_Request.API_LoadGear(Globals.API_TOKEN, RegionCB.Text, RealmCB.Text, CharacterNameBox.Text);

                ImageBrush backdrop = new ImageBrush();
                backdrop.ImageSource = API_Request.RenderImage("ImageResources/Backgrounds/Background_Nzoth.png", 800, 800);
                backdrop.Opacity = 0.6;
                MainGrid.Background = backdrop;

                ImageBrush charImage = new ImageBrush();
                charImage.ImageSource = await API_Request.API_LoadPlayerMedia(Globals.API_TOKEN, RegionCB.Text, RealmCB.Text, CharacterNameBox.Text);
                MainPanel.Background = charImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught Exception: " + ex);
                MessageBox.Show("Error: Failed to load player profile data");
                return;
            }

            string charName = CharacterNameBox.Text.ToLower();
            CHARACTER_NAME.Text = charName[0].ToString().ToUpper() + charName.Substring(1, charName.Length - 1);
            CHARACTER_ILVL.Text = "Item Level " + API_Globals.Player_Ilvl.ToString();

            //!!!// EVENTUALLY MOVE TO INSTANTIATE GRIDS BASED ON STYLES? //!!!//
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
            //Console.WriteLine("Sender: " + g.Name);

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
                { "ArmorClass", new string[] { item.ArmorClass.Class, "#FFFFFF", "12"} },
                { "Binding", new string[] { item.Binding.Type, "#FFFFFF", "12" } },
                { "DPS", new string[] { item.Weapon == null ? "" : item.Weapon.DPS.Value, "#FFFFFF", "12" } },
                { "Durability", new string[] { item.Durability == null ? "" : item.Durability.Value, "#FFFFFF", "12" } },
                { "Enchantments", new string[] { item.Enchantments == null ? "" : item.Enchantments[0].Value, "#00FF00", "12" } },
                { "InventoryType", new string[] { item.InventoryType.Name, "#FFFFFF", "12" } },
                { "Level", new string[] { "Item Level " + item.Level.Ilvl.ToString(), "#EEEE00", "12" } },
                { "Name", new string[] { item.Name, API_Globals.QualityColors[item.Quality.Value], "16" } },
                { "Requirements", new string[] { item.Requirements == null || item.Requirements.LevelRequirement == null ? "" : item.Requirements.LevelRequirement.Value, "#FFFFFF", "12" } },
                { "SellPrice", new string[] { item.SellPrice == null ? "" : item.SellPrice.Value.Gold, "#FFFFFF", "12" } },
                { "Sockets", new string[] { item.Sockets == null ? "" : item.Sockets[0].Value, "#FFFFFF", "12" } },
                { "Source", new string[] { item.Source == null ? "" : item.Source.Name, "#00FF00", "12" } },
                { "Speed", new string[] { item.Weapon == null ? "" : item.Weapon.AttackSpeed.Speed, "#FFFFFF", "12" } },
                { "Spells", new string[] { item.Spells == null ? "" : item.Spells[0].Description, "#FFFFFF", "12" } },
                { "Stats", new string[] { item.Stats == null ? "" : item.Stats[0].Display.Value, "#FFFFFF", "12" } },
                { "Transmog", new string[] { item.Transmog == null ? "" : item.Transmog.Value, "#FF99FF", "12" } },
                { "UniqueEquipped", new string[] { item.UniqueEquipped, "#FFFFFF", "12" } },
                { "Weapon", new string[] { item.Weapon == null ? "" : item.Weapon.Damage.Value, "#FFFFFF", "12" } },
            };

            PropertyInfo[] properties = item.GetType().GetProperties();

            string[] armorTypeExclusions = { "BODY", "SHIRT", "TABARD", "NECK", "CLOAK", "FINGER", "TRINKET" };

            foreach (PropertyInfo property in properties)
            {
                // Ignores properties not set on item i.e. Weapon on non-weapon items
                if (!textBlockData.ContainsKey(property.Name)) { continue; }
                if (property.GetValue(item, null) == null) { continue; }

                TextBlock t = GearItem.ItemText(textBlockData[property.Name]);
                Grid g = new Grid();

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
                            t = GearItem.ItemText(new string[] { item.Stats[i].Display.Value, item.Stats[i].Display.Color.GetColor(), "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Enchantments":
                        for (int i = 0; i < item.Enchantments.Length; i++)
                        {
                            string s = item.Enchantments[i].Value;
                            t = GearItem.ItemText(new string[] { s.Substring(0, s.IndexOf(" |A") < 0 ? s.Length : s.IndexOf(" |A")), "#00FF00", "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Sockets":
                        for (int i = 0; i < item.Sockets.Length; i++)
                        {
                            Image gemImage = new Image();
                            try
                            {
                                //Catches empty sockets
                                if (item.Sockets[i].Gem != null)
                                {
                                    gemImage.Source = API_Request.RenderImage("ImageResources/Gems/" + item.Sockets[i].Gem.Name + ".png", 12, 12);
                                    t = GearItem.ItemText(new string[] { item.Sockets[i].Value, "#FFFFFF", "12" });
                                }
                                else
                                {
                                    t = GearItem.ItemText(new string[] { "Empty Socket", "#FF0000", "12" });
                                }
                            }
                            catch
                            {
                                gemImage.Source = API_Request.RenderImage("ImageResources/Gems/Unknown.png", 12, 12);
                            }

                            StackPanel socketPanel = new StackPanel();
                            socketPanel.Orientation = Orientation.Horizontal;

                            Border border = new Border();
                            border.Style = (Style)Resources["GemBorder"];
                            
                            Border mask = new Border();
                            mask.Style = (Style)Resources["GemMask"];

                            Grid gr = new Grid();
                            border.Child = gr;
                            
                            gr.Children.Add(mask);
                            gr.Children.Add(gemImage);

                            socketPanel.Children.Add(border);
                            socketPanel.Children.Add(t);

                            GearInfoPanel.Children.Add(socketPanel);                       
                        }
                        break;

                    case "Spells":
                        for (int i = 0; i < item.Spells.Length; i++)
                        {
                            string color = item.Spells[i].Color == null ? "#00FF00" : item.Spells[i].Color.GetColor();
                            t = GearItem.ItemText(new string[] { item.Spells[i].Description, color, "12" });
                            GearInfoPanel.Children.Add(t);
                        }
                        break;

                    case "SellPrice":
                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Horizontal;
                        stackPanel.Children.Add(GearItem.ItemText(new string[] { "Sell Price: ", "#FFFFFF", "12" }));

                        PropertyInfo[] priceProperties = item.SellPrice.Value.GetType().GetProperties();
                        foreach(PropertyInfo p in priceProperties)
                        {
                            Image img = new Image();
                            img.Source = API_Request.RenderImage("ImageResources/Money/" + p.Name + ".png", 12, 12);
                            img.Margin = new Thickness(5, 0, 5, 0);
                            stackPanel.Children.Add(GearItem.ItemText(new string[] {p.GetValue(item.SellPrice.Value).ToString(), "#FFFFFF", "12" }));
                            stackPanel.Children.Add(img);
                        }

                        GearInfoPanel.Children.Add(stackPanel);
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