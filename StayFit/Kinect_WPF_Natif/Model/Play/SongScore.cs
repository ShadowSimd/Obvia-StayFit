using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model.Play
{
    public class SongScore
    {

        public string Name { get; set; }
        public int Score { get; private set; } = 0;
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public List<ActiveSongMoveStatus> SongMoveStatus { get; set; }

        #region UI Variables
        private bool _scoreChagned = false;
        public bool ScoreChanged { 
            get
            {
                if (_scoreChagned)
                {
                    _scoreChagned = false;
                    return true;
                }
                return false;
            }
        }
        public MoveScore LastMoveScore { get; private set; } = MoveScore.NotEvaluated;
        #endregion

        public void UpdateScore(MoveScore score)
        {
            if (score == MoveScore.NotEvaluated)
                return;

            _scoreChagned = true;
            LastMoveScore = score;

            if (score == MoveScore.Miss)
            {
                Combo = 0;
                return;
            }

            Combo++;
            MaxCombo = Math.Max(Combo, MaxCombo);

            int scoreBaseValue = 0;

            switch (score)
            {
                case MoveScore.NotEvaluated:
                    break;
                case MoveScore.Miss:
                    break;
                case MoveScore.Ok:
                    scoreBaseValue = 25;
                    break;
                case MoveScore.Good:
                    scoreBaseValue = 50;
                    break;
                case MoveScore.Perfect:
                    scoreBaseValue = 100;
                    break;
            }

            Score += scoreBaseValue * Combo;
        }
    }
}
