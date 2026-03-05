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
        #region Images

        public const string InitialMoveImagePath = "pack://application:,,,/Images/Moves";

        #endregion

        #region Audio

        private const string InitialAudioPath = "Audio/Songs";
        public static List<SongSelectItem> AVAILABLE_SONGS = new List<SongSelectItem>
        {
            new SongSelectItem {Difficulty = 1, Path = $"{InitialAudioPath}/To_Brazil.mp3", SongId = 1, Title = "Vengaboys - To Brazil!" }
        };

        #endregion
    }
}
