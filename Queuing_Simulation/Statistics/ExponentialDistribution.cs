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
            average = 1 / lambda;
            variance = 1 / lambda / lambda;
        }

        public override double Next()
        {
			return -Math.Log(rng.NextDouble()) / lambda;
        }
    }
}