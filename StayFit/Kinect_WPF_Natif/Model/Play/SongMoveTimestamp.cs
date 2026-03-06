using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kinect_WPF_Natif.Model.ML.MoveHandler;

namespace Kinect_WPF_Natif.Model.Play
{
    public class SongMoveTimestamp
    {
        public TimeSpan Time { get; set; }
        public Moves MoveId { get; set; }
    }
}
