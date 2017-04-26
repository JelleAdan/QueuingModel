using System;

namespace Queuing_Simulation
{
	public class HyperExponentialDistribution : Distribution
	{
		private double[] lambda { get; set; }
		private double[] prob { get; set; }

		public HyperExponentialDistribution(Random rng, double[] lambda, double[] prob)
		{
			this.rng = rng;
            Array.Sort(prob, lambda);
			this.prob = prob;
			this.lambda = lambda;
			for (int i = 0; i < prob.Length; i++)
			{
				average += prob[i] / lambda[i];
				variance += prob[i] / lambda[i] / lambda[i];
			}
			variance = 2 * variance - average * average;
		}

		public override double Next()
		{
			double a = rng.NextDouble();
			int index = 0;
			for (int i = 0; i < prob.Length; i++)
			{
				if (a < prob[i])
				{
					index = i;
					break;
				}
			}
			return -Math.Log(rng.NextDouble()) / lambda[index];
		}
	}
}