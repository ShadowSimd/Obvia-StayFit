using Kinect_WPF_Natif.Model.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model
{
    public static class Constants
    {
        private const string InitialPath = "Audio/Songs";
        public static List<SongSelectItem> AVAILABLE_SONGS = new List<SongSelectItem>
        {
            new SongSelectItem {Difficulty = 1, Path = $"{InitialPath}/To_Brazil.mp3", SongId = 1, Title = "Vengaboys - To Brazil!" }
        };
    }
}
