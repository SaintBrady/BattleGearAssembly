﻿using System;
using System.Windows;
using System.Windows.Controls;

/* TODO
 * ADD ENCHANTS TO GEAR
 * RENDER BOX FOR GEAR ITEM ON HOVER
 * HYPERLINKS FOR CLICKING NAME AND GEAR ITEMS
 * ADDITIONAL TABS FOR IO SCORE, ETC
*/

namespace BattleGearAssembly
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ActiveFrame.Source = new Uri("pack://application:,,,/" + button.Name + ".xaml");
        }
    }
}