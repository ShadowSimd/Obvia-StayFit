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
using System.Windows.Media.Animation;
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
        private AI _ai = new AI();
        private SongScore _score = new SongScore(); 

        public SongOngoingPage(SongSelectItem loadedSong)
        {
            InitializeComponent();
            _currentSong = loadedSong;
            _score.SongMoveStatus = _currentSong.MoveTimestamps.Select(mts => new ActiveSongMoveStatus(mts)).ToList();
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
            _ai.InitializeModelFromSavedData();

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
            ActiveSongMoveStatus nextMove = _score.SongMoveStatus.FirstOrDefault(mts => !mts.IsSpawned && _player.Position.TotalMilliseconds + Constants.ImageScrollTime >= mts.SongMoveTimestamp.Time.TotalMilliseconds);

            if (nextMove != null)
            {
                SpawnMovingImage(Constants.ImageScrollTime / 1000, nextMove.SongMoveTimestamp.MoveId);
                nextMove.IsSpawned = true;
            }

            if (_score.ScoreChanged)
            {
                lblScore.Content = $"{_score.Score.ToString().PadLeft(7, '0')}";
                lblCombo.Content = $"{_score.Combo}x | Max: {_score.MaxCombo}";
                lblTestResult.Content = $"{_score.LastMoveScore.ToString()}";
                lblTestResult.Foreground = _score.LastMoveScore == MoveScore.Miss ? System.Windows.Media.Brushes.Red :
                    _score.LastMoveScore == MoveScore.Ok ? System.Windows.Media.Brushes.Orange :
                    _score.LastMoveScore == MoveScore.Good ? System.Windows.Media.Brushes.Green : 
                    System.Windows.Media.Brushes.Cyan;
            }
        }

        private void SpawnMovingImage(int transitionDurationSeconds, Moves moveId)
        {
            BitmapSource cachedMoveImg = _cachedMovedImages[moveId];

            double canvaHeight = canvaMoveImg.ActualHeight;
            double canvaWidth = canvaMoveImg.ActualWidth;
            double imgSize = canvaHeight / 3;

            System.Windows.Controls.Image moveImg = new System.Windows.Controls.Image
            {
                Source = cachedMoveImg,
                Width = imgSize,
                Height = imgSize,
                Stretch = Stretch.Uniform
            };

            canvaMoveImg.Children.Add(moveImg);

            double startX = canvaMoveImg.ActualWidth;
            Canvas.SetLeft(moveImg, 0);
            Canvas.SetTop(moveImg, canvaHeight / 2 - imgSize / 2);

            DoubleAnimation moveLeftAnimation = new DoubleAnimation
            {
                From = -imgSize,
                To = canvaWidth - imgSize,
                Duration = TimeSpan.FromSeconds(transitionDurationSeconds)
            };

            moveLeftAnimation.Completed += (s, e) => { canvaMoveImg.Children.Remove(moveImg); };

            moveImg.BeginAnimation(Canvas.LeftProperty, moveLeftAnimation);
        }

        private void GameLoop_PredictionLogic()
        {

            List<ActiveSongMoveStatus> moveToEvaluate = _score.SongMoveStatus.Where(mts => !mts.IsEvaluated && _player.Position.TotalMilliseconds + Constants.PredictionBuffer >= mts.SongMoveTimestamp.Time.TotalMilliseconds).ToList();
            if (moveToEvaluate.Count == 0)
            {
                lblTestMove.Content = $"Not evaluating";
                return;
            }

            foreach(ActiveSongMoveStatus nextMove in moveToEvaluate)
            {
                if (_player.Position.TotalMilliseconds <= nextMove.SongMoveTimestamp.Time.TotalMilliseconds + Constants.PredictionBuffer)
                {
                    Body body = _kinectHelper.Bodies.FirstOrDefault(b => b.IsTracked);
                    if (body == null)
                    {
                        lblTestMove.Content = $"No body";
                        continue;
                    }

                    MovePredictionResult prediction = _ai.Predict(body);
                    MovePredictionWithBestScore strippedPrediction = new MovePredictionWithBestScore
                    {
                        Prediction = prediction.Prediction,
                        Score = prediction.Scores.Max()
                    };
                    nextMove.PredictionResults.Add(strippedPrediction);
                    lblTestMove.Content = $"{prediction.Prediction} ({strippedPrediction.Score})";
                    continue;
                }

                Move correctMove = MoveHandler.GetMove(nextMove.SongMoveTimestamp.MoveId);
                List<MovePredictionWithBestScore> correctPredictions = nextMove.PredictionResults.Where(b => b.Prediction == correctMove.Label).ToList();
                if (correctPredictions.Count == 0)
                    nextMove.Score = MoveScore.Miss;
                else
                {
                    float aiPredictionBestScore = correctPredictions.Max(mp => mp.Score);
                    if (aiPredictionBestScore > -35)
                        nextMove.Score = MoveScore.Perfect;
                    else if (aiPredictionBestScore > -45)
                        nextMove.Score = MoveScore.Good;
                    else
                        nextMove.Score = MoveScore.Ok;
                }
                
                nextMove.IsEvaluated = true;
                _score.UpdateScore(nextMove.Score);
            }
        }

        private void Bodyframe_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    _kinectHelper.ShowBodiesOnCanva(bodyFrame, canvas);

                    GameLoop_PredictionLogic();
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
