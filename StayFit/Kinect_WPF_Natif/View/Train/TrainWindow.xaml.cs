using Kinect_WPF_Natif.Model;
using Kinect_WPF_Natif.Model.DTO;
using Kinect_WPF_Natif.Model.Helpers;
using Kinect_WPF_Natif.Model.ML;
using Kinect_WPF_Natif.View;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kinect_WPF_Natif
{
    public partial class TrainWindow : Window
    {
        private KinectSensor _kinectSensor = null;
        private KinectHelper _kinectHelper = null;
        private ColorFrameReader _colorFrameReader = null;
        private BodyFrameReader _bodyFrameReader = null;
        AI _ai = new AI();

        /// <summary>
        ///     Ctor
        /// </summary>
        public TrainWindow()
        {
            InitializeComponent();
            SetupMoveSelector();
            InitializeKinect();
        }

        /// <summary>
        ///     Initialise le move selector
        /// </summary>
        private void SetupMoveSelector()
        {
            List<TrainableMoveDTO> trainableMoves = MoveHandler.GetAllMoves()
                .Select(m => new TrainableMoveDTO { Id = (int)m.MoveId, DisplayName = m.DisplayName }).ToList();
            MoveSelector.ItemsSource = trainableMoves;
            MoveSelector.SelectedIndex = 0;
        }

        /// <summary>
        ///     Initialise la kinect
        /// </summary>
        private void InitializeKinect()
        {
            _kinectSensor = KinectSensor.GetDefault();

            if (_kinectSensor != null)
            {
                _kinectSensor.IsAvailableChanged += KinectSensor_IsAvailableChanged;
                _kinectSensor.Open();

                _kinectHelper = new KinectHelper(_kinectSensor);

                _colorFrameReader = _kinectSensor.ColorFrameSource.OpenReader();
                _colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;

                _bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();
                _bodyFrameReader.FrameArrived += Bodyframe_FrameArrived;
            }
        }

        /// <summary>
        /// Événement lorsqu'une image couleur est créée par la kinect.
        /// </summary>
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

        /// <summary>
        /// Événement lorsqu'un squelette est détecté.
        /// </summary>
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

        /// <summary>
        /// Événement lancé lorsque la kinect est connectée ou déconnectée.
        /// </summary>
        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            txtConsole.Text += e.IsAvailable ? "\nKinect disponible" : "\nKinect indisponible";
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            if (_kinectSensor == null)
            {
                txtConsole.Text += "\nBouton Entrainement: Kinect introuvable";
                return;
            }

            if (_kinectHelper.Bodies.Count(s => s.IsTracked) == 0)
            {
                txtConsole.Text += "\nBouton Entrainement: Aucun corp détecté";
                return;
            }

            if (_kinectHelper.Bodies.Count(s => s.IsTracked) > 1)
            {
                txtConsole.Text += "\nTrop de corps détectés";
                return;
            }

            Body body = _kinectHelper.Bodies.First(b => b.IsTracked);
            TrainableMoveDTO selectedMove = MoveSelector.SelectedItem as TrainableMoveDTO;
            MoveHandler.Move move = MoveHandler.GetMove((MoveHandler.Moves)selectedMove.Id);
            _ai.AddTrainingData(move, body);
        }

        private void btnCreateModel_Click(object sender, RoutedEventArgs e)
        {
            if (_kinectSensor == null)
            {
                txtConsole.Text += "\nBouton Entrainement: Kinect introuvable";
                return;
            }

            _ai.InitializeModelFromData();
        }

        private void btnGuess_Click(object sender, RoutedEventArgs e)
        {
            if (_kinectSensor == null)
            {
                txtConsole.Text += "\nBouton Deviner: Kinect introuvable";
                return;
            }

            if (_kinectHelper.Bodies.Count(s => s.IsTracked) == 0)
            {
                txtConsole.Text += "\nBouton Deviner: Aucun corp détecté";
                return;
            }

            if (_kinectHelper.Bodies.Count(s => s.IsTracked) > 1)
            {
                txtConsole.Text += "\nTrop de corps détectés";
                return;
            }

            Body body = _kinectHelper.Bodies.First(b => b.IsTracked);
            MovePredictionResult res = _ai.Predict(body);

            txtConsole.Text += $"\nBouton Deviner : Le move est -> {res.Prediction}";
        }

        /// <summary>
        /// Fermer la connexion à la Kinect si on ferme l'écran.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
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

        }

    }
}