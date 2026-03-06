using Kinect_WPF_Natif.Model;
using Kinect_WPF_Natif.Model.Data;
using Kinect_WPF_Natif.Model.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

                SongSelectItem loadedSong = InitializeSongData(selectedSong.SongId);
                if (loadedSong != null)
                {
                    SongOngoingPage workoutPage = new SongOngoingPage(loadedSong);
                    NavigationService.Navigate(workoutPage);

                }
            }

        }

        /// <summary>
        ///     Permet d'initialiser l'information d'une chason
        /// </summary>
        /// <returns>True: data trouvé, false : data introuvable</returns>
        private SongSelectItem InitializeSongData(int sondId)
        {
            SongSelectItem loadedSong = JsonHelper.ChargerFichier(sondId);

            if (loadedSong == null)
            {
                MessageBox.Show("Cette chanson ne contient pas d'information", "ERROR", MessageBoxButton.OK);

                // Navigation is requested, but the object still exists!
                NavigationService.Navigate(new SongSelectPage());
                return null;
            }
            return loadedSong;
        }
    }
}
