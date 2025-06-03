using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
/*
 * TO DO
 * Add images to dungeons w/ greyscaling
 * Fix "Best in Time" defaulting to 312.6 value
 * Box being overscaled for Workshop due to name overflow
 * 
 * Add specs file where can directly read rather than pull API requests
 */

namespace BattleGearAssembly
{

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
            Grid g = sender as Grid;

            if (DungeonToolTip.Visibility == Visibility.Visible) {
                DungeonToolTip.Visibility = Visibility.Collapsed;
                Chest.Children.Clear();
                Party.Children.Clear();
                return; 
            }

            // Resolves dungeons with no keystone data for given player
            if (API_Globals.character.TopDungeons[g.Tag.ToString()] == null) return;

            Dungeon d = API_Globals.character.TopDungeons[g.Tag.ToString()];

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

            foreach (DungeonCharacter dunChar in d.Characters)
            {
                dunChar.Spec = API_Globals.SpecDict[dunChar.Spec.Id];
            }

            IEnumerable<DungeonCharacter> dunChars = d.Characters.OrderByDescending(c => c.Spec.Role.Name);

            foreach (DungeonCharacter dunChar in dunChars)
            {
                Grid memberGrid = new Grid();

                Image roleIcon = new Image()
                {
                    Source = API_Request.RenderImage($"ImageResources/RoleIcons/{dunChar.Spec.Role.Name}.png", 64, 64),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                memberGrid.Children.Add(roleIcon);

                TextBlock t = new TextBlock()
                {
                    Style = Resources["PartyMember"] as Style,
                    Text = $"{dunChar.Info.Name} - {dunChar.Spec.Name} {dunChar.Spec.Class.Name}", // Gets spec when dungeon was run
                    Foreground = dunChar.Spec.Class.getColor()
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

        public void ShowMythicPlus()
        {
            Score.Text = ((int)API_Globals.character.getMythicPluScore()).ToString();
            Score.Foreground = API_Globals.character.getScoreColor();

            foreach (Grid g in DungeonSP.Children)
            {
                TextBlock textBlock = g.Children.OfType<TextBlock>().First();
                Image image = g.Children.OfType<Image>().First();
                Border border = g.Children.OfType<Border>().First();

                if (API_Globals.character.TopDungeons[g.Tag.ToString()] != null)
                {
                    Dungeon d = API_Globals.character.TopDungeons[g.Tag.ToString()];
                    textBlock.Text = d.Level.ToString();
                    image.Source = API_Request.RenderImage("ImageResources/Dungeons/" + g.Name + ".png", 100, 100);

                    SolidColorBrush levelColor = d.getLevelColor();
                    textBlock.Foreground = levelColor;
                    border.BorderBrush = levelColor;
                }
                else
                {
                    image.Source = API_Request.RenderImage("ImageResources/Dungeons/Blanks/" + g.Name + "_Gray.png", 100, 100);
                    textBlock.Text = "";
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
            }
        }
    }
}