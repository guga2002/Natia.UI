using Natia.Neurall.Model;

namespace Natia.Neurall.Interfaces
{
    public interface ISolutionRecommendationService
    {
        void TrainModelFromDatabase();
        SolutionRecommendationOutput Predict(SolutionRecommendationInput input);
    }
}
