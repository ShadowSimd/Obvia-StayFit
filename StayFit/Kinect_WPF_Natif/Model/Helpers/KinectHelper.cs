using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kinect_WPF_Natif.Model.Helpers
{
    /// <summary>
    ///     Simon Déry - 3 mars 2026
    ///     Centralise la logique de gestion de la kinect et d'affichage des informations de celle-ci
    /// </summary>
    public class KinectHelper
    {
        #region Constants

        public static readonly double DPI = 96.0;
        public static readonly PixelFormat FORMAT = PixelFormats.Bgra32;
        public const int BODY_ELLIPSE_SIZE = 25;

        #endregion

        private WriteableBitmap _bitmap = null;
        private byte[] _imgPixels = null;
        private KinectSensor _kinect = null;

        public Body[] Bodies { get; private set; } = null;

        /// <summary>
        ///     Simon Déry - 3 mars 2026
        ///     Ctor
        /// </summary>
        /// <param name="kinect"></param>
        public KinectHelper(KinectSensor kinect)
        {
            _kinect = kinect;

            FrameDescription colorFrameDescription = _kinect.ColorFrameSource.FrameDescription;
            _imgPixels = new byte[colorFrameDescription.Width * colorFrameDescription.Height * 4];
            _bitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, DPI, DPI, FORMAT, null);
            Bodies = new Body[_kinect.BodyFrameSource.BodyCount];
        }

        /// <summary>
        /// Affiche une image couleur
        /// </summary>
        public void ShowColorFrame(ColorFrame colorFrame, Image image)
        {
            FrameDescription frameDescription = colorFrame.FrameDescription;

            colorFrame.CopyConvertedFrameDataToArray(_imgPixels, ColorImageFormat.Bgra);

            _bitmap.Lock();
            _bitmap.WritePixels(new Int32Rect(0, 0, frameDescription.Width, frameDescription.Height), _imgPixels, frameDescription.Width * 4, 0);
            _bitmap.Unlock();

            if (image.Source != _bitmap)
            {
                image.Source = _bitmap;
            }
        }

        public void ShowBodiesOnCanva(BodyFrame bodyFrame, Canvas targetCanvas)
        {
            bodyFrame.GetAndRefreshBodyData(Bodies);

            targetCanvas.Children.Clear();

            foreach (Body squelette in Bodies.Where(b => b.IsTracked))
            {
                foreach (Joint j in squelette.Joints.Values)
                {
                    if (j.TrackingState == TrackingState.Tracked)
                        DrawJoint(j, Colors.Red, BODY_ELLIPSE_SIZE, targetCanvas);
                }
            }
        }

        /// <summary>
        /// Dessine une élispe sur le joint du squelette
        /// </summary>
        private void DrawJoint(Joint joint, Color color, int size, Canvas canvas)
        {
            if (joint.Position.X != 0 && joint.Position.Y != 0 && joint.Position.Z != 0)
            {
                // Convertir la position du joint en coordonnées d'écran
                Point point = GetPoint(joint.Position, canvas.Width, canvas.Height);

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
        /// Convertit une position par rapport à la caméra(x,y) par rapport à l'image affichée (utilisant Depth Space pour l'alignement).
        /// </summary>
        private Point GetPoint(CameraSpacePoint position, double canvasWidth, double canvasHeight)
        {
            Point point = new Point();

            ColorSpacePoint colorPoint = _kinect.CoordinateMapper.MapCameraPointToColorSpace(position);

            point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
            point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

            point.X = point.X / 1920.0 * canvasWidth;
            point.Y = point.Y / 1080.0 * canvasHeight;

            return point;
        }
    }
}
