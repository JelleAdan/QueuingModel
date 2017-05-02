using System;

namespace Queuing_Simulation
{
	public class DeterministicDistribution : Distribution
	{
		private double serviceTime;

		public DeterministicDistribution(Random rng, double serviceTime)
		{
			this.rng = rng;
			this.serviceTime = serviceTime;
			average = serviceTime;
			variance = 0;
            residual = (variance + average * average) / (2 * average);
		}

		public override double Next()
		{
			return serviceTime;
		}
	}
}