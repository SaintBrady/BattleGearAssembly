using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattleGearAssembly
{
    public static class MythicPlusGlobals
    {
        //public static Dictionary<Dungeon, Character[]> dungeonMembers = new Dictionary<string, Dungeon>()

        public static Dictionary<string, string> dungeonAlias = new Dictionary<string, string>()
            {
                {"NW", "The Necrotic Wake"},
                {"MOTS", "Mists of Tirna Scithe"},
                {"SOB", "Siege of Boralus"},
                {"COT", "City of Threads"},
                {"AK", "Ara-Kara, City of Echoes"},
                {"GB", "Grim Batol"},
                {"SV", "The Stonevault"},
                {"DB", "The Dawnbreaker"}
            };
    }

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
            if (DungeonToolTip.Visibility == Visibility.Visible) {
                DungeonToolTip.Visibility = Visibility.Collapsed;
                Party.Children.Clear();
                return; 
            }

            Grid g = sender as Grid;
            Dungeon d = getDungeon(MythicPlusGlobals.dungeonAlias[g.Name]);

            // Handle no keys for dungeon

            // ForNum chests add <Image Source="ImageResources/Dungeons/Star.png" Width="16"/> to ChestSP
            //for (int i = 0; i < d.) Need to get chesting values from other API?

            DungeonName.Text = d.Name.Value;

            string region = API_Globals.character.Region;

            foreach (DungeonCharacter dc in d.Characters)
            {
                dc.Spec = API_Globals.SpecDict[dc.Spec.Id];//await API_Request.LoadSpec(region, dc.Spec.Id);
            }

            IEnumerable<DungeonCharacter> dunChars = d.Characters.OrderByDescending(c => c.Spec.Role.Name);

            foreach (DungeonCharacter dc in dunChars)
            {
                Grid memberGrid = new Grid();

                Image roleIcon = new Image();
                roleIcon.Source = API_Request.RenderImage($"ImageResources/RoleIcons/{dc.Spec.Role.Name}.png", 64, 64);
                roleIcon.HorizontalAlignment = HorizontalAlignment.Left;
                memberGrid.Children.Add(roleIcon);

                TextBlock t = new TextBlock();
                t.Style = Resources["PartyMember"] as Style;
                t.Text = $"{dc.Info.Name} - {dc.Spec.Name} {dc.Spec.Class.Name}"; // Gets spec when dungeon was run
                t.Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(dc.Spec.Class.getColor()));
                memberGrid.Children.Add(t);

                Party.Children.Add(memberGrid);
            }

            Score1_FG.Text = d.Level.ToString();
            Score1_BG.Text = Score1_FG.Text;
            Score1.Text = d.Rating.Value.ToString("0.0");
            Score1_Time.Text = "BEST OVERALL - " + d.GetTime();

            DungeonToolTip.Visibility = Visibility.Visible;
        }

        public Dungeon getDungeon(string dungeonName)
        {
            foreach (Dungeon d in API_Globals.character.KeyProfile.Dungeons)
            {
                if (d.Name.Value == dungeonName)
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