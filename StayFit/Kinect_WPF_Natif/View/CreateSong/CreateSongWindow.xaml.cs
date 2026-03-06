using Kinect_WPF_Natif.Model;
using Kinect_WPF_Natif.Model.Data;
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
        private SongSelectItem _currentSong { get; set; } = null;
        private MediaPlayer _player = new MediaPlayer();
        private List<SongMoveTimestamp> _moveTimestamps = new List<SongMoveTimestamp>();


        public CreateSongWindow()
        {
            InitializeComponent();
            InitializeMoves();
            InitializeSongSelector();

            _player.MediaEnded += _player_MediaEnded;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            _player.Stop();
            _player.Close();
        }

        /// <summary>
        ///     Simon Déry - 5 mars 2026
        ///     Commence la chanson
        /// </summary>
        /// <param name="song"></param>
        private void StartSong(SongSelectItem song)
        {
            _currentSong = song;

            _player.Open(new Uri(song.Path, UriKind.RelativeOrAbsolute));
            _player.Volume = Constants.MEDIA_PLAYER_VOLUME;
            _player.Play();
        }

        private void _player_MediaEnded(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save your changes?",
                                          "Confirmation",
                                          MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                _currentSong = null;
                _moveTimestamps.Clear();
            }
            else
            {
                _currentSong.MoveTimestamps = _moveTimestamps;
                JsonHelper.SaveSongJson(_currentSong);
            }
        }

        public void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton?.DataContext is Move selectedMove)
            {
                if (_currentSong == null)
                {
                    lblConsole.Content = $"MoveButton_Click: Clicked on {selectedMove.DisplayName} but the song has not been selected !";
                    return;
                }

                TimeSpan timeStamp = _player.Position;
                if (timeStamp == TimeSpan.Zero)
                {
                    lblConsole.Content = $"MoveButton_Click: Clicked on {selectedMove.DisplayName} but timer position is invalid !";
                    return;
                }

                _moveTimestamps.Add(new SongMoveTimestamp { MoveId = selectedMove.MoveId, Time = timeStamp });
                lblConsole.Content = $"MoveButton_Click: Clicked on {selectedMove.DisplayName} at {timeStamp.TotalMilliseconds}ms";
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

            StartSong(SongSelector.SelectedItem as SongSelectItem);
            lblConsole.Content = $"btnCreer_Click : Playing song: {_currentSong.Title}";

        }
    }
}
