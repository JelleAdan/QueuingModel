using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    public class ExponentialDistribution : Distribution
    {
        private double lambda;
        public ExponentialDistribution(Random rng, double lambda)
        {
            this.rng = rng;
            this.lambda = lambda;
        }

        public double GetAverage()
        {
            average = 1 / lambda;
            return average;
        }

        public double GetVariance()
        {
            variance = 1 / lambda / lambda;
            return variance;
        }

        public double Next()
        {
            return -Math.Log(rng.NextDouble()) / lambda;
        }
    }
}
