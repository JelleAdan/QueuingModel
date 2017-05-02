using System;

namespace Queuing_Simulation
{
	public class UniformDistribution : Distribution
	{
		private double a { get; set; }
		private double b { get; set; }

		public UniformDistribution(Random rng, double a, double b)
		{
			this.rng = rng;
			this.a = a;
			this.b = b;
			average = 0.5 * (a + b);
			variance = (1 / 12) * (b - a) * (b - a);
            residual = (variance + average * average) / (2 * average);
        }

        public override double Next()
		{
			return a + (b - a) * rng.NextDouble();
		}
	}
}