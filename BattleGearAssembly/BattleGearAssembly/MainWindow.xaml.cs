using System;
using System.Windows;
using System.Windows.Controls;

/* TODO
 * HYPERLINKS FOR CLICKING NAME AND GEAR ITEMS
*/

namespace BattleGearAssembly
{
    public static class Pages
    {
        public static Page GearPage;
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            
            //!!!// Eventually move to dictionary when enough pages are added //!!!//

            if (button.Name != "Gear")
            {
                Pages.GearPage = ActiveFrame.Content as Page;
            }

            ActiveFrame.Source = new Uri("pack://application:,,,/" + button.Name + ".xaml");

            if (button.Name == "Gear")
            {
                ActiveFrame.Content = Pages.GearPage;
            }
        }
    }
}