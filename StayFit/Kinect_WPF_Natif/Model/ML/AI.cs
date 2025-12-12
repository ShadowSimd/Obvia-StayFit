using Kinect_WPF_Natif.Model.DTO;
using Kinect_WPF_Natif.Model.ML;
using Microsoft.Kinect;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kinect_WPF_Natif.Model.ML.MoveHandler;

namespace Kinect_WPF_Natif.Model
{
    public class AI : IAI
    {
        private List<MoveData> _movesData = new List<MoveData>();
        private PredictionEngine<MoveData, MovePredictionResult> _moveAI = null;

        /// <summary>
        ///     Simon Déry - 11 décembre 2025
        ///     Permet d'ajouter un move au training data set
        /// </summary>
        /// <param name="move"></param>
        /// <param name="body"></param>
        public void AddTrainingData(MoveHandler.Move move, Body body)
        {

            MoveData moveData = createMoveData(move, body);
            _movesData.Add(moveData);
        }

        /// <summary>
        ///     Simon Déry - 11 décembre 2025
        ///     Permet de créer le modèle
        /// </summary>
        public void InitializeModelFromData()
        {
            if (_movesData.Count == 0)
                return;

            MLContext mlContext = new MLContext();

            IDataView dataView = mlContext.Data.LoadFromEnumerable(_movesData);

            Microsoft.ML.Data.EstimatorChain<Microsoft.ML.Transforms.KeyToValueMappingTransformer> pipeline =
                mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "KeyedLabel",
                    inputColumnName: "Label"
                )
                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "MoveJoints",
                    "ConfidenceScore"
                    )
                )
                .Append(mlContext.MulticlassClassification.Trainers.NaiveBayes(
                    labelColumnName: "KeyedLabel",
                    featureColumnName: "Features")
                )
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                    outputColumnName: nameof(MovePredictionResult.Prediction),
                    inputColumnName: "PredictedLabel"
                ));

            var preview = dataView.Preview(maxRows: 5);
            string asd = string.Join(", ", preview.Schema.Select(c => c.Name));


            Microsoft.ML.Data.TransformerChain<Microsoft.ML.Transforms.KeyToValueMappingTransformer> trainedModel = pipeline.Fit(dataView);

            var output = trainedModel.GetOutputSchema(dataView.Schema);

            _moveAI = mlContext.Model.CreatePredictionEngine<MoveData, MovePredictionResult>(trainedModel);
        }

        /// <summary>
        ///     Simon Déry - 12 décembre 2025
        ///     Permet de deviner quel est le move
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public MovePredictionResult Predict(Body body)
        {
            if (_moveAI == null)
                new Exception("AI pipeline was never initiated");

            MoveData moveData = createMoveData(null, body);
            MovePredictionResult prediction = _moveAI.Predict(moveData);

            return prediction;
        }

        /// <summary>
        ///     Simon Déry - 12 décembre 2025
        ///     Permet de créer un objet de Type MoveData
        /// </summary>
        /// <param name="move"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private MoveData createMoveData(MoveHandler.Move move, Body body)
        {
            return new MoveData()
            {
                MoveLabel = move?.Label,
                MoveJoints = bodyJointsToTrainingData(body.Joints),
                ConfidenceScore = bodyJointsToConfidenceData(body.Joints)
            };
        }

        /// <summary>
        ///     Simon Déry - 11 décembre 2025
        ///     Permet de transformer les joints de la kinect en data utilisable pour l'AI    
        /// </summary>
        /// <param name="joints"></param>
        /// <returns></returns>
        private float[] bodyJointsToTrainingData(IReadOnlyDictionary<JointType, Joint> joints)
        {
            float[] filteredData = joints
                    .SelectMany(pair => new[]
                    {
                        pair.Value.Position.X,
                        pair.Value.Position.Y,
                        pair.Value.Position.Z
                    })
                    .ToArray();

            return filteredData;
        }

        /// <summary>
        ///     Simon Déry - 12 décembre 2025
        ///     Permet de transformer le tracking states des joints de la kinect en data de "confidence" utilisable pour l'AI   
        /// </summary>
        /// <param name="joints"></param>
        /// <returns></returns>
        private float[] bodyJointsToConfidenceData(IReadOnlyDictionary<JointType, Joint> joints)
        {
            float[] confidenceScores = joints
                .SelectMany(c => new[]
                {
                    (float)c.Value.TrackingState
                })
                .ToArray();

            return confidenceScores;
        }
    }
}
