using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Natia.Core.Context;
using Natia.Neurall.Interfaces;
using Natia.Neurall.Model;
using System.IO;
using System.Linq;

namespace Natia.Neurall.Services
{
    public class NeuralMLPredict : INeuralMLPredict
    {
        private ITransformer? _model;
        private SpeakerDbContext _context;

        public NeuralMLPredict(SpeakerDbContext context)
        {
            _context = context;
        }

        public void TrainFromDatabase()
        {

        }

        private void RetrainModel()
        {

        }

        private void TrainModel(List<NeuralInput> data)
        {
        }

        public async Task<NeuralPredictionOutput> Predict(NeuralInput input)
        {
            var historicalData = await _context.Neuralls.ToListAsync();
            var result = new NeuralPredictionOutput { AnomalyDetails = string.Empty };

            // ანომალიის ქულა
            double anomalyScore = 0.0;

            // 1. შეცდომების სიგრძის ანალიზი
            anomalyScore += EvaluateErrorDetailsLength(input, historicalData, result);

            // 2. ბოლო დროინდელი შეცდომების შემოწმება
            anomalyScore += CheckRecentErrors(input, historicalData, result);

            // 3. ტექსტის მსგავსების შემო

            // 4. იშვიათი არხების შემოწმება
            anomalyScore += CheckRareChannels(input, historicalData, result);

            // ანომალიის გამოვლენა ქულის მიხედვით
            result.IsAnomalous = anomalyScore > 0.5;

            if (result.IsAnomalous)
            {
                // ყველა ჩანაწერის "კრიტიკულობის" ფლაგის გაუქმება
                foreach (var record in historicalData)
                {
                    record.IsCritical = false;
                }
                await _context.SaveChangesAsync();
            }

            return result;
        }

        // 1. შეცდომების სიგრძის ანალიზი
        private double EvaluateErrorDetailsLength(NeuralInput input, List<Core.Entities.Neurall> historicalData, NeuralPredictionOutput result)
        {
            var lengths = historicalData.Select(h => h.ErrorDetails?.Length ?? 0).ToList();
            if (lengths.Count == 0) return 0;

            var avgLength = lengths.Average();
            var stdDev = Math.Sqrt(lengths.Average(v => Math.Pow(v - avgLength, 2)));

            var inputLength = input.ErrorDetails?.Length ?? 0;
            if (Math.Abs(inputLength - avgLength) > 2 * stdDev)
            {
                result.AnomalyDetails += "მოცემული პრობლემის სიგრძე მნიშვნელოვნად განსხვავდება ისტორიული ჩანაწერების საშუალო სიგრძისგან.\n";
                return 0.3; // ანომალიის ქულა
            }
            return 0;
        }

        // 2. ბოლო დროინდელი შეცდომების შემოწმება
        private double CheckRecentErrors(NeuralInput input, List<Core.Entities.Neurall> historicalData, NeuralPredictionOutput result)
        {
            var recentErrors = historicalData
                .Where(h => h.ErrorMessage == input.ErrorMessage)
                .Take(10)
                .ToList();

            if (!recentErrors.Any())
            {
                result.AnomalyDetails += "მოცემული შეცდომა ბოლო დროს არ დაფიქსირებულა და შესაძლოა ახალი იყოს.\n";
                return 0.3; // ანომალიის ქულა
            }
            return 0;
        }

        // 3. ტექსტის მსგავსების შემოწმება

        // 4. იშვიათი არხების შემოწმება
        private double CheckRareChannels(NeuralInput input, List<Core.Entities.Neurall> historicalData, NeuralPredictionOutput result)
        {
            if (!historicalData.Any(h => h.ChannelName == input.ChannelName))
            {
                result.AnomalyDetails += $"'{input.ChannelName}'-ზე პირველად დაპიქსირდა პრობლემა, გადაამოწმე";
                return 0.2; // ანომალიის ქულა
            }
            return 0;
        }

        // კოსინუსური მსგავსების გამოთვლა
        private double CalculateCosineSimilarity(string text1, string text2)
        {
            if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2))
                return 0;

            var vector1 = text1.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            var vector2 = text2.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

            var commonKeys = vector1.Keys.Intersect(vector2.Keys);
            var dotProduct = commonKeys.Sum(key => vector1[key] * vector2[key]);

            var magnitude1 = Math.Sqrt(vector1.Values.Sum(v => v * v));
            var magnitude2 = Math.Sqrt(vector2.Values.Sum(v => v * v));

            return dotProduct / (magnitude1 * magnitude2);
        }

    }
}
