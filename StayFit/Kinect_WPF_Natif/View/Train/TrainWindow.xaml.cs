using Microsoft.Kinect;
using System;
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
        #region Constants

        public static readonly double DPI = 96.0;
        public static readonly PixelFormat FORMAT = PixelFormats.Bgra32;
        private const int BODY_ELLIPSE_SIZE = 25;

        #endregion

        private KinectSensor _kinectSensor = null;
        private ColorFrameReader _colorFrameReader = null;
        private BodyFrameReader _bodyFrameReader = null;

        private WriteableBitmap _bitmap = null;
        private byte[] _picPixels = null;


        /// <summary>
        /// ctor
        /// </summary>
        public TrainWindow()
        {
            InitializeComponent();
            _kinectSensor = KinectSensor.GetDefault();

            if (_kinectSensor != null)
            {
                _kinectSensor.IsAvailableChanged += KinectSensor_IsAvailableChanged;
                _kinectSensor.Open();

                FrameDescription colorFrameDescription = _kinectSensor.ColorFrameSource.FrameDescription;
                _picPixels = new byte[colorFrameDescription.Width * colorFrameDescription.Height * 4];
                _bitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, DPI, DPI, FORMAT, null);

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
                    ShowColorFrame(colorFrame);
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
                    Body[] squelettes = new Body[bodyFrame.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(squelettes);

                    canvas.Children.Clear();

                    foreach (Body squelette in squelettes.Where(b => b.IsTracked))
                    {
                        foreach(Joint j in squelette.Joints.Values)
                        {
                            if (j.TrackingState == TrackingState.Tracked)
                                DrawJoint(j, Colors.Red, BODY_ELLIPSE_SIZE);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Affiche une image couleur
        /// </summary>
        private void ShowColorFrame(ColorFrame colorFrame)
        {
            FrameDescription frameDescription = colorFrame.FrameDescription;

            colorFrame.CopyConvertedFrameDataToArray(_picPixels, ColorImageFormat.Bgra);

            RenderPixelArray(_picPixels, frameDescription);
        }

        /// <summary>
        /// Convertit une position par rapport à la caméra(x,y) par rapport à l'image affichée (utilisant Depth Space pour l'alignement).
        /// </summary>
        public Point GetPoint(CameraSpacePoint position)
        {
            Point point = new Point();

            ColorSpacePoint colorPoint = _kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(position);

            point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
            point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

            point.X = point.X / 1920.0 * canvas.Width;
            point.Y = point.Y / 1080.0 * canvas.Height;

            return point;
        }

        /// <summary>
        /// Dessine une élispe sur le joint du squelette
        /// </summary>
        private void DrawJoint(Joint joint, Color color, int size)
        {
            if (joint.Position.X != 0 && joint.Position.Y != 0 && joint.Position.Z != 0)
            {
                // Convertir la position du joint en coordonnées d'écran
                Point point = GetPoint(joint.Position);

                // Créer un cercle à la position du joint
                Ellipse ellipse = new Ellipse
                {
                    Fill = new SolidColorBrush(color),
                    Width = size,
                    Height = size
                };

                // Positionner le cercle sur l'élément de dessin Canvas
                Canvas.SetLeft(ellipse, point.X - size / 2);
                Canvas.SetTop(ellipse, point.Y - size / 2);

                // Ajouter le cercle à l'élément de dessin Canvas
                canvas.Children.Add(ellipse);
            }
        }

        /// <summary>
        /// Optimisation utilisé pour remplacer en mémoire l'image de pixel plutot que de recréer une nouvelle image et de l'assigner à imgCameraKinet
        /// </summary>
        private void RenderPixelArray(byte[] pixels, FrameDescription currentFrameDescription)
        {
            _bitmap.Lock();
            _bitmap.WritePixels(new Int32Rect(0, 0, currentFrameDescription.Width, currentFrameDescription.Height), pixels, currentFrameDescription.Width * 4, 0);
            _bitmap.Unlock();
            imgCameraKinect.Source = _bitmap;
        }

        /// <summary>
        /// Événement lancé lorsque la kinect est connectée ou déconnectée.
        /// </summary>
        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            txtConsole.Text += e.IsAvailable ? "\nKinect disponible" : "\nKinect indisponible";
        }

        /// <summary>
        /// Fermer la connexion à la Kinect si on ferme l'écran.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_kinectSensor != null)
            {
                _colorFrameReader.FrameArrived -= ColorFrameReader_FrameArrived;
                _bodyFrameReader.FrameArrived -= Bodyframe_FrameArrived;
                _kinectSensor.Close();
            }
        }
    }
}