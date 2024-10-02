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
                    string qualityColorHex = API_Globals.QualityColors[Gear[g.Name].Quality.QualityType];
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

            CreateGearWindow(g.Name);
            if (GearToolTip.Visibility == Visibility.Collapsed) { GearToolTip.Visibility = Visibility.Visible; }
            else { GearToolTip.Visibility = Visibility.Collapsed; }
        }

        private void CreateGearWindow(string slot)
        {
            GearInfoPanel.Children.Clear();
            GearItem item = API_Globals.Gear[slot];

            // Will be "for not null # of properties

            PropertyInfo[] properties = item.GetType().GetProperties();
            int count = 0;

            foreach (PropertyInfo property in properties) // returns number of parent methods in GearItem currently
            {
                count++;
                Console.WriteLine(property.Name);
                TextBox textBox = new TextBox();
                //textBox.Text = property;
            }

            //GearTextBox g = new GearTextBox(string text, SolidColorBrush foreground, FontWeight fontWeight, int fontSize);
            Console.WriteLine(item.Name + ": " + count.ToString());
            TT_Name.Text = "Test";
            TT_Name.Foreground = Brushes.White;

            GearInfoPanel.Children.Add(TT_Name);
            
        }
    }
}
