using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model.ML
{
    public interface IAI
    {
        void AddTrainingData(MoveHandler.Move move, Body body);
        
        void InitializeModelFromData();

        MovePredictionResult Predict(Body body);
    }
}
