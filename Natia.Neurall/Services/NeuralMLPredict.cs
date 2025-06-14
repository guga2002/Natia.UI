using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Neurall.Interfaces;
using Natia.Neurall.Model;

namespace Natia.Neurall.Services;

public class NeuralMLPredict : INeuralMLPredict
{
    private readonly SpeakerDbContext _context;
    private ITransformer? _model;

    public NeuralMLPredict(SpeakerDbContext context)
    {
        _context = context;
    }

    public void TrainFromDatabase()
    {
    }

    public async Task<NeuralPredictionOutput> Predict(NeuralInput input)
    {
        var historicalData = await _context.Neuralls.ToListAsync();
        var result = new NeuralPredictionOutput { AnomalyDetails = string.Empty };

        double anomalyScore = 0;

        anomalyScore += EvaluateErrorDetailsLength(input, historicalData, result);
        anomalyScore += CheckRecentErrors(input, historicalData, result);
        anomalyScore += EvaluateTextSimilarity(input, historicalData, result);
        anomalyScore += CheckRareChannels(input, historicalData, result);
        anomalyScore += EvaluateTopicFrequency(input, historicalData, result);
        anomalyScore += EvaluateCriticalityDeviation(input, historicalData, result);
        anomalyScore += EvaluateSatelliteAnomaly(input, historicalData, result);
        anomalyScore += CheckRecentFrequency(input, historicalData, result);


        result.IsAnomalous = anomalyScore > 0.6;

        if (result.IsAnomalous)
        {
            foreach (var record in historicalData)
                record.IsCritical = false;

            await _context.SaveChangesAsync();
        }

        return result;
    }

    private double EvaluateErrorDetailsLength(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var lengths = history.Select(h => h.ErrorDetails?.Length ?? 0).ToList();
        if (lengths.Count == 0) return 0;

        var avg = lengths.Average();
        var stdDev = Math.Sqrt(lengths.Average(v => Math.Pow(v - avg, 2)));
        var inputLen = input.ErrorDetails?.Length ?? 0;

        if (Math.Abs(inputLen - avg) > 2 * stdDev)
        {
            result.AnomalyDetails += "მოცემული პრობლემის სიგრძე მნიშვნელოვნად განსხვავდება ისტორიული ჩანაწერებისგან.\n";
            return 0.3;
        }
        return 0;
    }

    private double CheckRecentErrors(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var recent = history
            .Where(h => h.ErrorMessage == input.ErrorMessage)
            .OrderByDescending(h => h.Id)
            .Take(10)
            .ToList();

        if (!recent.Any())
        {
            result.AnomalyDetails += "მოცემული შეცდომა ბოლო დროს არ დაფიქსირებულა.\n";
            return 0.3;
        }
        return 0;
    }

    private double EvaluateTextSimilarity(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var similarities = history
            .Where(h => !string.IsNullOrWhiteSpace(h.ErrorDetails))
            .Select(h => CalculateCosineSimilarity(h.ErrorDetails!, input.ErrorDetails ?? string.Empty))
            .ToList();

        if (similarities.Count == 0) return 0;

        var maxSim = similarities.Max();

        if (maxSim < 0.5)
        {
            result.AnomalyDetails += "შეტყობინება მნიშვნელოვნად განსხვავდება წინა შეტყობინებებისგან.\n";
            return 0.3;
        }
        return 0;
    }

    private double CheckRareChannels(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        if (!history.Any(h => h.ChannelName == input.ChannelName))
        {
            result.AnomalyDetails += $"'{input.ChannelName}'-ზე პირველად დაფიქსირდა პრობლემა.\n";
            return 0.2;
        }
        return 0;
    }

    private double CalculateCosineSimilarity(string text1, string text2)
    {
        var v1 = text1.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        var v2 = text2.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

        var commonKeys = v1.Keys.Intersect(v2.Keys);
        var dotProduct = commonKeys.Sum(k => v1[k] * v2[k]);

        var magnitude1 = Math.Sqrt(v1.Values.Sum(v => v * v));
        var magnitude2 = Math.Sqrt(v2.Values.Sum(v => v * v));

        if (magnitude1 == 0 || magnitude2 == 0) return 0;
        return dotProduct / (magnitude1 * magnitude2);
    }

    private double EvaluateTopicFrequency(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var topicCount = history.Count(h => h.WhatWasTopic == input.WhatWasTopic);
        var total = history.Count;

        if (topicCount == 0 || (double)topicCount / total < 0.05)
        {
            result.AnomalyDetails += $"თემა '{input.WhatWasTopic}' იშვიათია.\n";
            return 0.2;
        }
        return 0;
    }

    private double EvaluateCriticalityDeviation(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var similar = history.Where(h => h.ErrorMessage == input.ErrorMessage).ToList();
        if (similar.Count > 5)
        {
            var criticalRate = similar.Count(h => h.IsCritical) / (double)similar.Count;
            if (criticalRate < 0.1 && input.Priority == Priority.კრიტიკული)
            {
                result.AnomalyDetails += "ისტორიულად მსგავსი შეტყობინებები არ იყო კრიტიკული, მაგრამ ახლა არის.\n";
                return 0.2;
            }
        }
        return 0;
    }

    private double EvaluateSatelliteAnomaly(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var satCount = history.Count(h => h.Satellite == input.Satellite);
        if (satCount == 0)
        {
            result.AnomalyDetails += $"'{input.Satellite}' სადგური პირველად ფიქსირდება.\n";
            return 0.2;
        }
        return 0;
    }

    private double CheckRecentFrequency(NeuralInput input, List<Core.Entities.Neurall> history, NeuralPredictionOutput result)
    {
        var recentWindow = DateTime.UtcNow.AddDays(-20);
        var recentMatches = history
            .Where(h => h.ErrorMessage == input.ErrorMessage && h.ActionDate >= recentWindow)
            .ToList();

        if (recentMatches.Count == 0)
        {
            result.AnomalyDetails += "ბოლო დღეებში მსგავსი შეტყობინება არ დაფიქსირებულა.\n";
            return 0.2;
        }
        return 0;
    }


}
