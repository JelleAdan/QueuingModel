using System;

namespace Queuing_Simulation
{
    public class NormalDistribution : Distribution
    {
		private double mu { get; set; }
		private double sigma { get; set; }

		public NormalDistribution(Random rng, double mu, double sigma)
        {
            this.rng = rng;
            this.mu = mu;
            this.sigma = sigma;
            average = mu;
            variance = sigma * sigma;
            residual = (variance + average * average) / (2 * average);
        }

        public override double Next()
        {
            return mu + sigma * (Math.Sqrt(-2 * Math.Log(rng.MyNextDouble())) * Math.Cos(2 * Math.PI * rng.MyNextDouble()));
        }
    }
}