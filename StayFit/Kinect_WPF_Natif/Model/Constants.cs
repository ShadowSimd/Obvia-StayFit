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
            new SongSelectItem {Difficulty = 3, Path = $"{InitialAudioPath}/To_Brazil.mp3", SongId = 1, Title = "Vengaboys - To Brazil!" },
            new SongSelectItem {Difficulty = 5, Path = $"{InitialAudioPath}/Pushup_Test.mp3", SongId = 2, Title = "Pushup Test - FitnessGram" },
            new SongSelectItem {Difficulty = 4, Path = $"{InitialAudioPath}/Kickstart_My_Heart.mp3", SongId = 3, Title = "Kickstart My Heart - Mötley Crüe" }

        };

        public const double MEDIA_PLAYER_VOLUME = 0.05;

        #endregion

        #region Songs

        public const string SongDataJsonPath = "/SongData";

        #endregion

        #region AI

        public const string MODEL_SAVE_PATH = "STAYFIT_PREDICTION_MODEL.zip";

        #endregion
    }
}
