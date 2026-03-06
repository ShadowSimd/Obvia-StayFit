using Kinect_WPF_Natif.Model;
using Kinect_WPF_Natif.Model.Data;
using Kinect_WPF_Natif.Model.Helpers;
using Kinect_WPF_Natif.Model.ML;
using Kinect_WPF_Natif.Model.Play;
using Microsoft.Kinect;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using static Kinect_WPF_Natif.Model.ML.MoveHandler;

namespace Kinect_WPF_Natif.View.Play
{
    /// <summary>
    /// Logique d'interaction pour SongOngoingPage.xaml
    /// </summary>
    public partial class SongOngoingPage : Page
    {

        private KinectSensor _kinectSensor = null;
        private ColorFrameReader _colorFrameReader = null;
        private BodyFrameReader _bodyFrameReader = null;
        private KinectHelper _kinectHelper = null;
        private MediaPlayer _player = new MediaPlayer();
        private SongSelectItem _currentSong = null;
        private bool _gameStarted = false;
        private Dictionary<Moves, BitmapSource> _cachedMovedImages = new Dictionary<Moves, BitmapSource>();
        Body[] _bodies = null;

        public SongOngoingPage(SongSelectItem loadedSong)
        {
            InitializeComponent();
            _currentSong = loadedSong;
        }

        /// <summary>
        ///     Initialise ce qui est nécessaire lorsque la page est loadé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeKinect();
            LoadMoveImages();

            CompositionTarget.Rendering += GameLoop_UIFrame;
        }

        /// <summary>
        ///     Simon Déry - 4 mars 2026
        ///     Initialise la kinect
        /// </summary>
        private void InitializeKinect()
        {
            _kinectSensor = KinectSensor.GetDefault();

            if (_kinectSensor != null)
            {
                _kinectSensor.IsAvailableChanged += KinectSensor_IsAvailableChanged;
                _kinectSensor.Open();

                _kinectHelper = new KinectHelper(_kinectSensor, 10);

                _colorFrameReader = _kinectSensor.ColorFrameSource.OpenReader();
                _colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;

                _bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();
                _bodyFrameReader.FrameArrived += Bodyframe_FrameArrived;
            }
        }

        /// <summary>
        ///     Simon Déry - 5 mars 2026
        ///     Load les images en cache pour permettre de changer rapidement
        /// </summary>
        private void LoadMoveImages()
        {
            List<Move> availableMoves = MoveHandler.GetAllMoves()
                .ToList();

            foreach (Move move in availableMoves)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(move.ImagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();
                bitmap.Freeze();

                _cachedMovedImages[move.MoveId] = bitmap;
            }
        }

        /// <summary>
        ///     Ferme la kinect lorsque la page est fermée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_colorFrameReader != null)
            {
                _colorFrameReader.FrameArrived -= ColorFrameReader_FrameArrived;
                _colorFrameReader.Dispose();
            }

            if (_bodyFrameReader != null)
            {
                _bodyFrameReader.FrameArrived -= Bodyframe_FrameArrived;
                _bodyFrameReader.Dispose();
            }

            if (_kinectSensor != null)
            {
                _kinectSensor.IsAvailableChanged -= KinectSensor_IsAvailableChanged;
            }

            _player.Stop();
            _player.Close();

            CompositionTarget.Rendering -= GameLoop_UIFrame;
        }

        /// <summary>
        ///     Simon Déry - 3 mars 2026
        ///     Démarre la chanson
        /// </summary>
        private void StartSong()
        {
            _gameStarted = true;

            _player.Open(new Uri(_currentSong.Path, UriKind.RelativeOrAbsolute));
            _player.Volume = 0.05;
            _player.Play();
        }

        /// <summary>
        ///     Simon Déry - 5 mars 2026
        ///     Chaque frame, ceci est call. Doit être utilisé pour les updates de UI uniquement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void GameLoop_UIFrame(object sender, EventArgs e)
        {
            if (!_player.HasAudio || _player.Position == TimeSpan.Zero)
                return;


            TimeSpan currentMoveTime = _player.Position;
            SongMoveTimestamp nextMove = _currentSong.MoveTimestamps.FirstOrDefault(mts => _player.Position.TotalMilliseconds + 500 >= mts.Time.TotalMilliseconds && _player.Position.TotalMilliseconds - 500 <= mts.Time.TotalMilliseconds);

            if (nextMove == null)
            {
                imgMove.Source = _cachedMovedImages[Moves.None];
                return;
            }
            else if (imgMove.Source != _cachedMovedImages[nextMove.MoveId])
                imgMove.Source = _cachedMovedImages[nextMove.MoveId];
        }

        private void Bodyframe_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    _kinectHelper.ShowBodiesOnCanva(bodyFrame, canvas);
                }
            }
        }

        private void ColorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    _kinectHelper.ShowColorFrame(colorFrame, imgCameraKinect);
                }
            }
        }

        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            lblKinectStatus.Content = e.IsAvailable ? "Disponible" : "Indisponible";
            lblKinectStatus.Foreground = e.IsAvailable ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

            if (e.IsAvailable && !_gameStarted)
                StartSong();
        }
    }
}
