using Kinect_WPF_Natif.Model.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kinect_WPF_Natif.View.Play
{
    /// <summary>
    /// Logique d'interaction pour PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow : Window
    {
        public PlayWindow()
        {
            InitializeComponent();
            LoadSongsSongSelect();
        }

        private void LoadSongsSongSelect()
        {
            List<SongSelectItem> songSelect = new List<SongSelectItem>()
            {
                new SongSelectItem { Title = "Chanson #1", Difficulty = 1, GameId = 1 },
                new SongSelectItem { Title = "Chanson #2", Difficulty = 2, GameId = 2 },
                new SongSelectItem { Title = "Chanson #3", Difficulty = 3, GameId = 3 },
                new SongSelectItem { Title = "Chanson #4", Difficulty = 4, GameId = 4 },
                new SongSelectItem { Title = "Chanson #5", Difficulty = 5, GameId = 5 },
            };

            SongListBox.ItemsSource = songSelect;
        }

        private void btnBackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
    }
}
