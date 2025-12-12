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
        public string Prediction { get; set; }

        [ColumnName("Score")]
        public float[] Scores { get; set; }

        [ColumnName("Probability")]
        public float[] Probabilities { get; set; }
    }
}
