using Microsoft.ML;
using Microsoft.ML.Trainers;
using Natia.Core.Context;
using Natia.Neurall.Interfaces;
using Natia.Neurall.Model;

namespace Natia.Neurall.Services;

public class SolutionRecommendationService : ISolutionRecommendationService
{
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private readonly SpeakerDbContext _context;
    private const string ModelPath = "SolutionRecommendationModel.zip";


    public SolutionRecommendationService(SpeakerDbContext context)
    {
        _mlContext = new MLContext();
        _context = context;
    }

    public void TrainModelFromDatabase()
    {
        var currentTime = DateTime.Now;

        if (!File.Exists(ModelPath))
        {
            Console.WriteLine("Model file not found. Training model...");
            RetrainModel();
        }
        else
        {
            var lastTrainedTime = File.GetLastWriteTime(ModelPath);

            if ((currentTime - lastTrainedTime).TotalHours >= 24)
            {
                Console.WriteLine("24 hours have passed since last model update. Retraining model...");
                RetrainModel();
            }
            else
            {
                Console.WriteLine($"Model was recently trained at {lastTrainedTime}. No retraining needed.");
            }
        }
    }

    private void RetrainModel()
    {
        var data = _context.Neuralls
            .Where(io => io.SuggestedSolution != null)
            .Select(io => new SolutionRecommendationInput
            {
                ErrorMessage = io.ErrorMessage,
                ErrorDetails = io.ErrorDetails,
                ChannelName = io.ChannelName,
                Satellite = io.Satellite,
                Priority = io.Priority.ToString(),
                SuggestedSolution = io.SuggestedSolution
            }).ToList();

        if (data.Any())
        {
            TrainModel(data);
            Console.WriteLine("Model retrained successfully.");
        }
        else
        {
            Console.WriteLine("No data available for retraining.");
        }
    }




    private void TrainModel(List<SolutionRecommendationInput> data)
    {
        Console.WriteLine("Building training pipeline...");

        try
        {

            var dat = data.Select(io => new SolutionRecommendationInput
            {
                ErrorMessage = io.ErrorMessage,
                ErrorDetails = io.ErrorDetails,
                ChannelName = io.ChannelName,
                Priority = io.Priority,
                Satellite = io.Satellite,
                SuggestedSolution = io.SuggestedSolution
            });

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "TextFeatures",
                    inputColumnName: nameof(SolutionRecommendationInput.ErrorDetails))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "ErrorMessageEncoded",
                    inputColumnName: nameof(SolutionRecommendationInput.ErrorMessage)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "ChannelNameEncoded",
                    inputColumnName: nameof(SolutionRecommendationInput.ChannelName)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "SatelliteEncoded",
                    inputColumnName: nameof(SolutionRecommendationInput.Satellite)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "PriorityEncoded",
                    inputColumnName: nameof(SolutionRecommendationInput.Priority)))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "TextFeatures", "ErrorMessageEncoded", "ChannelNameEncoded", "SatelliteEncoded", "PriorityEncoded"))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "Label",
                    inputColumnName: nameof(SolutionRecommendationInput.SuggestedSolution)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated(
                    new SdcaNonCalibratedMulticlassTrainer.Options
                    {
                        MaximumNumberOfIterations = 50
                    }))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue(
                    outputColumnName: "PredictedLabel"));

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Training the model...");
            _model = pipeline.Fit(trainingData);

            stopwatch.Stop();
            Console.WriteLine($"Training completed in {stopwatch.ElapsedMilliseconds / 1000.0} seconds");

            Console.WriteLine("Saving trained model...");
            _mlContext.Model.Save(_model, trainingData.Schema, ModelPath);
            Console.WriteLine($"Model saved successfully to {ModelPath}.");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



    public SolutionRecommendationOutput Predict(SolutionRecommendationInput input)
    {
        if (_model == null)
        {
            Console.WriteLine("Loading pre-trained model...");
            if (File.Exists(ModelPath))
            {
                _model = _mlContext.Model.Load(ModelPath, out var _);
                Console.WriteLine("Model loaded successfully.");
            }
            else
            {
                throw new InvalidOperationException("Model not trained or saved. Please train the model first.");
            }
        }

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<SolutionRecommendationInput, SolutionRecommendationOutput>(_model);
        return predictionEngine.Predict(input);
    }
}
