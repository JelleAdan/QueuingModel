using System;

namespace Queuing_Simulation
{
	public static class ExtensionMethods
	{
		public static double NextGaussian(this Random rng)
		{
			return Math.Sqrt(-2 * Math.Log(rng.NextDouble())) * Math.Cos(2 * Math.PI * rng.NextDouble());
		}

		public static bool NextBernoulli(this Random rng, double p)
		{
			if (rng.NextDouble() < p)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}