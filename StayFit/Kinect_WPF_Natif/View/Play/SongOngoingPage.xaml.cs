using Kinect_WPF_Natif.Model.Helpers;
using Microsoft.Kinect;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Logique d'interaction pour SongOngoingPage.xaml
    /// </summary>
    public partial class SongOngoingPage : Page
    {

        private KinectSensor _kinectSensor = null;
        private ColorFrameReader _colorFrameReader = null;
        private BodyFrameReader _bodyFrameReader = null;
        private KinectHelper _kinectHelper = null;
        Body[] _bodies = null;

        public SongOngoingPage(int songId)
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Initialise ce qui est nécessaire lorsque la page est loadé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeKinect();
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

                _kinectHelper = new KinectHelper(_kinectSensor, 10);

                _colorFrameReader = _kinectSensor.ColorFrameSource.OpenReader();
                _colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;

                _bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();
                _bodyFrameReader.FrameArrived += Bodyframe_FrameArrived;
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
        }
    }
}
