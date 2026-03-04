using Kinect_WPF_Natif.View.CreateSong;
using Kinect_WPF_Natif.View.Play;
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

namespace Kinect_WPF_Natif.View
{
    /// <summary>
    /// Logique d'interaction pour HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayWindow playWindow = new PlayWindow();
            hideCurrentAndShowNextWindow(playWindow);
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            TrainWindow trainWindow = new TrainWindow();
            hideCurrentAndShowNextWindow(trainWindow);
        }
        private void btnCreate_CLick(object sender, RoutedEventArgs e)
        {
            CreateSongWindow createSongWindow = new CreateSongWindow();
            hideCurrentAndShowNextWindow(createSongWindow);
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void hideCurrentAndShowNextWindow(Window w)
        {
            w.Closed += (s, args) =>
            {
                Show();
            };
            w.Show();
            Hide();
        }
    }
}
