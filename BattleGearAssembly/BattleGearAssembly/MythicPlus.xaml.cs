using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattleGearAssembly
{
    public static class MythicPlusGlobals
    {
        public static Dictionary<string, Dungeon> dungeons = new Dictionary<string, Dungeon>();
    }

    public partial class MythicPlus : Page
    {
        public MythicPlus()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowMythicPlus();
        }

        private void ToggleShowDungeonTT(object sender, RoutedEventArgs e)
        {
            if (DungeonToolTip.Visibility == Visibility.Visible) {
                DungeonToolTip.Visibility = Visibility.Collapsed;
                Chest.Children.Clear();
                Party.Children.Clear();
                return; 
            }

            Grid g = sender as Grid;

            // Resolves dungeons with no keystone data for given player
            if (!MythicPlusGlobals.dungeons.ContainsKey(g.Name)) return;

            Dungeon d = MythicPlusGlobals.dungeons[g.Name];

            foreach (KeystoneUpgrade k in d.KeystoneUpgrades)
            {
                if(d.Duration <= k.Duration)
                {
                    Image starImage = new Image();
                    starImage.Source = API_Request.RenderImage("ImageResources/Dungeons/Star.png", 64, 64);
                    Chest.Children.Add(starImage);
                }
            }

            DungeonName.Text = d.Info.Name;

            string region = API_Globals.character.Region;

            foreach (DungeonCharacter dc in d.Characters)
            {
                dc.Spec = API_Globals.SpecDict[dc.Spec.Id];
            }

            IEnumerable<DungeonCharacter> dunChars = d.Characters.OrderByDescending(c => c.Spec.Role.Name);

            foreach (DungeonCharacter dc in dunChars)
            {
                Grid memberGrid = new Grid();

                Image roleIcon = new Image()
                {
                    Source = API_Request.RenderImage($"ImageResources/RoleIcons/{dc.Spec.Role.Name}.png", 64, 64),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                memberGrid.Children.Add(roleIcon);

                TextBlock t = new TextBlock()
                {
                    Style = Resources["PartyMember"] as Style,
                    Text = $"{dc.Info.Name} - {dc.Spec.Name} {dc.Spec.Class.Name}", // Gets spec when dungeon was run
                    Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(dc.Spec.Class.getColor()))
                };
                memberGrid.Children.Add(t);

                Party.Children.Add(memberGrid);
            }

            Score1_FG.Text = d.Level.ToString();
            Score1_BG.Text = Score1_FG.Text;
            Score1.Text = d.Rating.Value.ToString("0.0");
            Score1_Time.Text = "BEST OVERALL - " + d.GetTime();

            DungeonToolTip.Visibility = Visibility.Visible;
        }

        public void getDungeons()
        {
            MythicPlusGlobals.dungeons.Clear();
            Dictionary<string, string> dungeonAliases = new Dictionary<string, string>()
            {
                {"The Necrotic Wake", "NW"},
                {"Mists of Tirna Scithe", "MOTS"},
                {"Siege of Boralus", "SOB"},
                {"City of Threads", "COT"},
                {"Ara-Kara, City of Echoes", "AK"},
                {"Grim Batol", "GB"},
                {"The Stonevault", "SV"},
                {"The Dawnbreaker", "DB"}
            };

            foreach (Dungeon d in API_Globals.character.KeyProfile.Dungeons)
            {
                string dunName = dungeonAliases[d.Info.Name];

                if(!MythicPlusGlobals.dungeons.ContainsKey(dunName))
                {
                    MythicPlusGlobals.dungeons.Add(dunName, d);
                }
            }
        }

        public void ShowMythicPlus()
        {
            getDungeons();

            SolidColorBrush scoreColor = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(API_Globals.character.KeyProfile.Rating.Color.GetColor()));

            Score.Text = ((int)API_Globals.character.KeyProfile.Rating.Value).ToString();
            Score.Foreground = scoreColor;

            foreach (Grid g in DungeonSP.Children)
            {
                TextBlock textBlock = g.Children.OfType<TextBlock>().First();
                Image image = g.Children.OfType<Image>().First();
                Border border = g.Children.OfType<Border>().First();

                if (MythicPlusGlobals.dungeons.ContainsKey(g.Name))
                {
                    Dungeon d = MythicPlusGlobals.dungeons[g.Name];
                    textBlock.Text = d.Level.ToString();
                    image.Source = API_Request.RenderImage("ImageResources/Dungeons/" + g.Name + ".png", 100, 100);

                    string colorHex = d.Rating.Color.GetColor();
                    SolidColorBrush levelColor = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex));
                    textBlock.Foreground = levelColor;
                    border.BorderBrush = levelColor;
                }
            }
        }
    }
}