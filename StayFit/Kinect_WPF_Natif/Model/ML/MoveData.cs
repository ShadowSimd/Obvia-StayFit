using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model.ML
{
    public class MoveData
    {
        [VectorType(75)]
        [LoadColumn(0)]
        public float[] MoveJoints { get; set; }

        [VectorType(25)]
        [LoadColumn(1)]
        public float[] ConfidenceScore { get; set; }

        [LoadColumn(2), ColumnName("Label")]
        public string MoveLabel { get; set; }
    }
}
