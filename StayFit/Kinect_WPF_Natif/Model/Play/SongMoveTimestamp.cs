using Kinect_WPF_Natif.Model.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kinect_WPF_Natif.Model.ML.MoveHandler;

namespace Kinect_WPF_Natif.Model.Play
{
    public enum MoveScore
    {
        NotEvaluated,
        Miss,
        Ok,
        Good,
        Perfect
    }
    public class SongMoveTimestamp
    {
        public TimeSpan Time { get; set; }
        public Moves MoveId { get; set; }
    }

    public class ActiveSongMoveStatus
    {
        public SongMoveTimestamp SongMoveTimestamp { get; set; }

        public bool IsSpawned { get; set; } = false;
        public bool IsEvaluated { get; set; } = false;
        public MoveScore Score { get; set; } = MoveScore.NotEvaluated;
        public List<MovePredictionWithBestScore> PredictionResults { get; set; } = new List<MovePredictionWithBestScore>();

        public ActiveSongMoveStatus(SongMoveTimestamp songMoveTimestamp)
        {
            SongMoveTimestamp = songMoveTimestamp;
        }
    }
}
