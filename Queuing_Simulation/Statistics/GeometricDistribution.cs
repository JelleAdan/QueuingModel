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
			average = p;
			variance = p;
		}

		public override double Next()
		{
			return Math.Log(rng.NextDouble())) / (Math.Log(1 - p);
		}
	}
}