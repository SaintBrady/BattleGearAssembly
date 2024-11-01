using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

/* TODO
 * HYPERLINKS FOR CLICKING NAME AND GEAR ITEMS
*/

namespace BattleGearAssembly
{
    public static class Pages
    {
        public static Dictionary<string, Page> pages = new Dictionary<string, Page>()
        {
            { "Gear", null },
            { "MythicPlus", null },
        };

        public static string ActivePage = "Gear";
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await API_Request.RequestAsync();
            await API_Request.LoadSpecs();
        }

        public void MythicPlus_Enabled(bool enabled)
        {
            MythicPlus.IsEnabled = enabled;
        }

        private void ChangeWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Name == Pages.ActivePage) return;

            Pages.pages[Pages.ActivePage] = ActiveFrame.Content as Page;
            ActiveFrame.Source = new Uri("pack://application:,,,/" + button.Name + ".xaml");

            if (Pages.pages[button.Name] != null)
            {
                ActiveFrame.Content = Pages.pages[button.Name];
            }

            Pages.ActivePage = button.Name;
        }
    }
}