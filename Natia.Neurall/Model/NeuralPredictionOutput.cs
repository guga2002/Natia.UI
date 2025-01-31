using Microsoft.ML.Data;

namespace Natia.Neurall.Model
{
    public class NeuralPredictionOutput
    {
        public bool IsAnomalous { get; set; }

        public string AnomalyDetails { get; set; }
    }
}
