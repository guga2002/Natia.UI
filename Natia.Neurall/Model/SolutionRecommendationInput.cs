using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natia.Neurall.Model
{
    public class SolutionRecommendationInput
    {
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public string ChannelName { get; set; }
        public string Satellite { get; set; }
        public string Priority { get; set; }

        public string SuggestedSolution { get; set; } = "Unknown";
    }
}
