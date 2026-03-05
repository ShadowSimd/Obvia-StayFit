using Kinect_WPF_Natif.Model;
using Kinect_WPF_Natif.Model.DTO;
using Kinect_WPF_Natif.Model.ML;
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
using static Kinect_WPF_Natif.Model.ML.MoveHandler;

namespace Kinect_WPF_Natif.View.CreateSong
{
    /// <summary>
    /// Logique d'interaction pour CreateSongWindow.xaml
    /// </summary>
    public partial class CreateSongWindow : Window
    {
        private SongSelectItem _currentSong = null;

        public CreateSongWindow()
        {
            InitializeComponent();
            InitializeMoves();
            InitializeSongSelector();
        }

        /// <summary>
        ///     Simon Déry - 4 mars 2026
        ///     Permet de créer des boutons contenant les moves diposnibles de l'IA
        /// </summary>
        private void InitializeMoves()
        {
            List<Move> availableMoves = MoveHandler.GetAllMoves()
                .Where(m => m.MoveId != Moves.None).ToList();

            MovesControl.ItemsSource = availableMoves;
        }

        /// <summary>
        ///     Simon Déry - 4 mars 2026
        ///     Initialise le song selector
        /// </summary>
        private void InitializeSongSelector()
        {
            List<SongSelectItem> songs = Constants.AVAILABLE_SONGS;
            SongSelector.ItemsSource = songs;
            SongSelector.SelectedIndex = 0;
        }

        public void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton?.DataContext is Move selectedMove)
            {
                lblConsole.Content = $"MoveButton_Click: Clicked on {selectedMove.DisplayName}";
            }
            else
                lblConsole.Content = "MoveButton_Click : Not Found";
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSong != null)
            {
                lblConsole.Content = "btnCreer_Click : A song is already playing";
                return;
            }

            _currentSong = SongSelector.SelectedItem as SongSelectItem;
            lblConsole.Content = $"btnCreer_Click : Playing song: {_currentSong.Title}";

        }
    }
}
