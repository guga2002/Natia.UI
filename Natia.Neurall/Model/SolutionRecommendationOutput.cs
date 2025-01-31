using Microsoft.ML.Data;

namespace Natia.Neurall.Model
{
    public class SolutionRecommendationOutput
    {
        [ColumnName("PredictedLabel")]
        public string SuggestedSolution { get; set; }
    }
}
