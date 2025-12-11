using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model.ML
{
    public class MovePredictionResult
    {
        [ColumnName("PredictedMove")]
        public string Preduction { get; set; }

        public float[] Scores { get; set; }
    }
}
