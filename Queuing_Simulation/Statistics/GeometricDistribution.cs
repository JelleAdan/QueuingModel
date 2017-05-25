using System;

namespace Queuing_Simulation
{
	public class GeometricDistribution : Distribution
	{
		private double p { get; set; }

		public GeometricDistribution(Random rng, double p)
		{
			this.rng = rng;
			this.p = p;
			average = 1 / p;
            variance = (1 - p) / p / p;
            residual = (variance + average * average) / (2 * average);
        }

        public override double Next()
		{
			return Math.Log(rng.MyNextDouble()) / Math.Log(1 - p);
		}
	}
}