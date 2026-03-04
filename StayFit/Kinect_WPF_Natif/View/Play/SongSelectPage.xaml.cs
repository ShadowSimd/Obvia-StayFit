using Kinect_WPF_Natif.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinect_WPF_Natif.View.Play
{
    /// <summary>
    /// Logique d'interaction pour SongSelect.xaml
    /// </summary>
    public partial class SongSelectPage : Page
    {
        public SongSelectPage()
        {
            InitializeComponent();
            LoadSongsSongSelect();
        }


        private void LoadSongsSongSelect()
        {
            List<SongSelectItem> songSelect = Constants.AVAILABLE_SONGS;

            SongListBox.ItemsSource = songSelect;
        }

        private void btnBackButton_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private void SongListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongListBox.SelectedItem is SongSelectItem selectedSong)
            {
                SongListBox.SelectedIndex = -1;

                SongOngoingPage workoutPage = new SongOngoingPage(selectedSong.SongId);
                NavigationService.Navigate(workoutPage);
            }

        }
    }
}
