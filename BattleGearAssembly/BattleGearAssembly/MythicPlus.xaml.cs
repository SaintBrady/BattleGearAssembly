using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattleGearAssembly
{
    public partial class MythicPlus : Page
    {
        public MythicPlus()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await API_Request.LoadMythicPlus();
            ShowMythicPlus();
        }

        private void ToggleShowDungeonTT(object sender, RoutedEventArgs e)
        {
            Grid g = sender as Grid;

            DungeonToolTip.Visibility = DungeonToolTip.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void CreateDungeonTT()
        {
            // Handle no keys for dungeon


        }

        public Dungeon getDungeon(string dungeonName)
        {
            foreach (Dungeon d in API_Globals.character.KeyProfile.Dungeons)
            {
                if (d.Info.Name == dungeonName)
                {
                    return d;
                }
            }
            return null;
        }

        public void ShowMythicPlus()
        {
            Dictionary<string, Dungeon> dungeons = new Dictionary<string, Dungeon>()
            {
                {"NW", getDungeon("The Necrotic Wake")},
                {"MOTS", getDungeon("Mists of Tirna Scithe")},
                {"SOB", getDungeon("Siege of Boralus")},
                {"COT", getDungeon("City of Threads")},
                {"AK", getDungeon("Ara-Kara, City of Echoes")},
                {"GB", getDungeon("Grim Batol")},
                {"SV", getDungeon("The Stonevault")},
                {"DB", getDungeon("The Dawnbreaker")}
            };

            SolidColorBrush scoreColor = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(API_Globals.character.KeyProfile.Rating.Color.GetColor()));

            Score.Text = ((int)API_Globals.character.KeyProfile.Rating.Value).ToString();
            Score.Foreground = scoreColor;

            foreach (Grid g in DungeonSP.Children)
            {
                TextBlock textBlock = g.Children.OfType<TextBlock>().First();
                Image image = g.Children.OfType<Image>().First();
                Border border = g.Children.OfType<Border>().First();

                if (dungeons[g.Name] != null)
                {
                    textBlock.Text = dungeons[g.Name].Level.ToString();
                    image.Source = API_Request.RenderImage("ImageResources/Dungeons/" + g.Name + ".png", 100, 100);

                    string colorHex = dungeons[g.Name].Rating.Color.GetColor();
                    SolidColorBrush levelColor = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex));
                    textBlock.Foreground = levelColor;
                    border.BorderBrush = levelColor;
                }
            }
        }
    }
}
