using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;
using Image = System.Windows.Controls.Image;

namespace BattleGearAssembly
{
    /*public class GearItemElement : INotifyPropertyChanged
    {
        //public Image Image { get; set; }
        //public TextBlock Ilvl {  get; set; }
        //public TextBlock Name { get; set; }
        //public Border Border { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        int s = 12;

        public int S
        {
            set
            {
                if (s != value)
                {
                    s = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("S"));
                    }
                }
            }
            get
            {
                return s;
            }
        }
    }*/

    public partial class Gear : Page
    {
        public Gear()
        {
            InitializeComponent();
        }

        private async void LoadRealms(object sender = null, EventArgs e = null)
        {
            RealmCB.IsEnabled = true;
            RealmCB.Items.Clear();
            List<string> realmList = await API_Request.LoadRealms(((ComboBoxItem)RegionCB.SelectedItem).Content.ToString());

            foreach (string realm in realmList)
            {
                RealmCB.Items.Add(realm);
            }
        }

        private void LookupItem(object sender, RoutedEventArgs e)
        {
            Grid g = sender as Grid;
            if (!API_Globals.character.Gear.ContainsKey(g.Name)) { return; }

            string item_id = API_Globals.character.Gear[g.Name].ItemInfo.ID.ToString();
            string url = $"https://www.wowhead.com/item={item_id}";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private async void Button_LoadGear(object sender, RoutedEventArgs e)
        {
            try
            {
                 await API_Request.LoadCharacterProfile(RegionCB.Text, RealmCB.Text, CharacterNameBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Failed to load player profile data");
                return;
            }

            Character character = API_Globals.character;

            ImageBrush backdrop = new ImageBrush()
            {
                ImageSource = character.Faction.Type == "HORDE" ? API_Request.RenderImage("ImageResources/Backgrounds/HordeBG.png", 800, 800) : API_Request.RenderImage("ImageResources/Backgrounds/AllianceBG.png", 800, 800),
                Opacity = 1.0
            };
            ImageBrush charImage = new ImageBrush()
            {
                ImageSource = await API_Request.LoadPlayerMedia(character.Media.Url)
            };

            MainGrid.Background = backdrop;
            MainPanel.Background = charImage;

            CHARACTER_NAME.Text = character.Name;
            CHARACTER_NAME.Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(character.Class.getColor()));
            CHARACTER_TITLE.Text = character.Title == null ? "" : character.Title.Name;
            CHARACTER_FACTION.Source = character.Faction.Type == "HORDE" ? API_Request.RenderImage("ImageResources/Faction/Horde.png", 240, 240) : API_Request.RenderImage("ImageResources/Faction/Alliance.png", 240, 240);
            CHARACTER_ILVL.Text = "Item Level " + character.Ilvl;

            Dictionary<string, GearItem> Gear = API_Globals.character.Gear;

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
                    LoadEmptySlot(image, itemName, ilvl, border, g);
                }
            }
        }

        private void LoadEmptySlot(Image image, TextBlock itemName, TextBlock ilvl, Border border, Grid g)
        {
            string gearSlotName = g.Name;
            if (gearSlotName.Contains("_1") || gearSlotName.Contains("_2")) { gearSlotName = gearSlotName.Substring(0, gearSlotName.Length - 2); }

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
            if (CreateGearWindow(g.Name) != 0) return;

            double topMargin = Math.Min(Math.Max(g.Margin.Top - (GearViewBox.ActualHeight / 2), 50), 650 - GearViewBox.ActualHeight);
            if (g.Name == "MAIN_HAND" || g.Name == "OFF_HAND") topMargin -= 100;

            GearToolTip.Margin = new Thickness(g.Margin.Left + 150, topMargin, g.Margin.Right + 150, 0);
            GearToolTip.HorizontalAlignment = g.HorizontalAlignment;
            GearToolTip.Visibility = Visibility.Visible;
        }

        private void ToggleHideGearTT(object sender, EventArgs e)
        {
            Grid g = sender as Grid;

            if (CreateGearWindow(g.Name) != 0) return;
            GearToolTip.Visibility = Visibility.Collapsed;
        }

        private int CreateGearWindow(string slot)
        {
            // Handles empty gear slot hover
            if (!API_Globals.character.Gear.TryGetValue(slot, out GearItem item)) return -1;

            GearInfoPanel.Children.Clear();

            Dictionary<string, string[]> textBlockData = new Dictionary<string, string[]>
            {
                { "ArmorClass", new string[] { item.ArmorClass.Class } },
                { "Binding", new string[] { item.Binding.Type } },
                { "DPS", new string[] { item.Weapon == null ? "" : item.Weapon.DPS.Value } },
                { "Durability", new string[] { item.Durability == null ? "" : item.Durability.Value } },
                { "Enchantments", new string[] { item.Enchantments == null ? "" : item.Enchantments[0].Value, "#00FF00"} },
                { "InventoryType", new string[] { item.InventoryType.Name } },
                { "Level", new string[] { "Item Level " + item.Level.Ilvl.ToString(), "#EEEE00"} },
                { "PlayerClass", new string[] { item.PlayerClass == null ? "" : item.PlayerClass.Value } },
                { "Name", new string[] { item.Name, API_Globals.QualityColors[item.Quality.Value], "16" } },
                { "Requirements", new string[] { item.Requirements == null || item.Requirements.LevelRequirement == null ? "" : item.Requirements.LevelRequirement.Value } },
                { "Set", new string[] { item.Set == null ? "" : item.Set.Count, "#FFC822"} },
                { "SellPrice", new string[] { item.SellPrice == null ? "" : item.SellPrice.Value.Gold } },
                { "Sockets", new string[] { item.Sockets == null ? "" : item.Sockets[0].Value, "#FFBB00"} },
                { "Source", new string[] { item.Source == null ? "" : item.Source.Name, "#00FF00"} },
                { "Speed", new string[] { item.Weapon == null ? "" : item.Weapon.AttackSpeed.Speed } },
                { "Spells", new string[] { item.Spells == null ? "" : item.Spells[0].Description } },
                { "Stats", new string[] { item.Stats == null ? "" : item.Stats[0].Display.Value } },
                { "Transmog", new string[] { item.Transmog == null ? "" : item.Transmog.Value, "#FF99FF"} },
                { "UniqueEquipped", new string[] { item.UniqueEquipped } },
                { "Weapon", new string[] { item.Weapon == null ? "" : item.Weapon.Damage.Value } }
            };

            PropertyInfo[] properties = item.GetType().GetProperties();

            string[] armorTypeExclusions = { "BODY", "SHIRT", "TABARD", "NECK", "CLOAK", "FINGER", "TRINKET" };

            foreach (PropertyInfo property in properties)
            {
                // Ignores properties not set on item i.e. Weapon on non-weapon items
                if (!textBlockData.ContainsKey(property.Name) || property.GetValue(item, null) == null) { continue; }

                TextBlock t = GearItem.ItemText(textBlockData[property.Name]);
                Grid g = new Grid();

                switch (property.Name)
                {
                    case "ArmorClass":
                        break;

                    case "InventoryType":
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
                            t = GearItem.ItemText(new string[] { item.Stats[i].Display.Value, item.Stats[i].Display.Color.GetColor() });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Enchantments":
                        for (int i = 0; i < item.Enchantments.Length; i++)
                        {
                            string s = item.Enchantments[i].Value;
                            t = GearItem.ItemText(new string[] { s.Substring(0, s.IndexOf(" |A") < 0 ? s.Length : s.IndexOf(" |A")), "#00FF00" });
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
                                    t = GearItem.ItemText(new string[] { item.Sockets[i].Value });
                                }
                                else
                                {
                                    gemImage.Source = API_Request.RenderImage("ImageResources/Gems/Unknown.png", 12, 12);
                                    t = GearItem.ItemText(new string[] { "Prismatic Socket", "#808080" });
                                }
                            }
                            catch
                            {
                                gemImage.Source = API_Request.RenderImage("ImageResources/Gems/Unknown.png", 12, 12);
                                t = GearItem.ItemText(new string[] { item.Sockets[i].Value });
                            }

                            Grid gr = new Grid();
                            StackPanel socketPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                            Border mask = new Border() { Style = (Style)Resources["GemMask"] };
                            Border border = new Border() {
                                Style = (Style)Resources["GemBorder"],
                                Child = gr
                            };

                            gr.Children.Add(mask);
                            gr.Children.Add(gemImage);

                            socketPanel.Children.Add(border);
                            socketPanel.Children.Add(t);

                            GearInfoPanel.Children.Add(socketPanel);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Spells":
                        for (int i = 0; i < item.Spells.Length; i++)
                        {
                            string color = item.Spells[i].Color == null ? "#00FF00" : item.Spells[i].Color.GetColor();
                            t = GearItem.ItemText(new string[] { item.Spells[i].Description, color });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Set":
                        GearInfoPanel.Children.Add(t);
                        for (int i = 0; i < item.Set.SetPieces.Length; i++)
                        {
                            var setItem = item.Set.SetPieces[i];
                            t = GearItem.ItemText(new string[] { setItem.ItemInfo.Name, setItem.Color(setItem.IsEquipped) });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        for (int i = 0; i < item.Set.Effects.Length; i++)
                        {
                            var setEffect = item.Set.Effects[i];
                            t = GearItem.ItemText(new string[] { setEffect.Value, setEffect.Color(setEffect.IsActive) });
                            GearInfoPanel.Children.Add(t);
                        }
                        GearInfoPanel.Children.Add(new TextBlock()); // Spacing
                        break;

                    case "Durability":
                        GearInfoPanel.Children.Add(t);
                        break;

                    case "SellPrice":
                        StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                        stackPanel.Children.Add(GearItem.ItemText(new string[] { "Sell Price: " }));

                        PropertyInfo[] priceProperties = item.SellPrice.Value.GetType().GetProperties();
                        foreach (PropertyInfo p in priceProperties)
                        {
                            Image img = new Image();
                            img.Source = API_Request.RenderImage("ImageResources/Money/" + p.Name + ".png", 12, 12);
                            img.Margin = new Thickness(5, 0, 5, 0);
                            stackPanel.Children.Add(GearItem.ItemText(new string[] { p.GetValue(item.SellPrice.Value).ToString() }));
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
