using Natia.Neurall.Model;

namespace Natia.Neurall.Interfaces
{
    public interface INeuralMLPredict
    {
        void TrainFromDatabase();
        Task<NeuralPredictionOutput> Predict(NeuralInput input);
    }
}
