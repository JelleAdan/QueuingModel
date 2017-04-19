using System;

namespace Queuing_Simulation
{
    public class ExponentialDistribution : Distribution
    {
		private double lambda { get; set; }

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
