using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kinect_WPF_Natif
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        ///     Simon Déry - 3 mars 2026
        ///     Centralise la logique de fermetture de la kinect
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            Microsoft.Kinect.KinectSensor sensor = Microsoft.Kinect.KinectSensor.GetDefault();
            if (sensor != null && sensor.IsOpen)
            {
                sensor.Close();
            }

            base.OnExit(e);
        }
    }
}
