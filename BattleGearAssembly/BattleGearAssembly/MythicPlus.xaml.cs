using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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

namespace BattleGearAssembly
{
    /// <summary>
    /// Interaction logic for MythicPlus.xaml
    /// </summary>
    public partial class MythicPlus : Page
    {
        public MythicPlus()
        {
            InitializeComponent();
        }

        public async void ShowMythicPlus(object sender, RoutedEventArgs e)
        {
            Character character = API_Globals.character;
            await API_Request.LoadMythicPlus(Globals.API_TOKEN, character.MythicPlus.Url);
        }
    }
}
