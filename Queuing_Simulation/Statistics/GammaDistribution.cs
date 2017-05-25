using System;

namespace Queuing_Simulation
{
	public class GammaDistribution : Distribution
	{
		private double alpha { get; set; }
		private double beta { get; set; }

		public GammaDistribution(Random rng, double alpha, double beta)
		{
			this.rng = rng;
			this.alpha = alpha;
			this.beta = beta;
			average = alpha / beta;
			variance = alpha / beta / beta;
            residual = (variance + average * average) / (2 * average);
        }

        public override double Next()
		{
			double shape = alpha;
			double scale = 1.0 / beta;

			if (shape < 1)
			{
				while (true)
				{
					double u = rng.MyNextDouble();
					double bGS = 1 + shape / Math.E;
					double p = bGS * u;

					if (p <= 1)
					{
						double x = Math.Pow(p, 1 / shape);
						double u2 = rng.NextDouble();

						if (u2 > Math.Exp(-x))
						{
							continue;
						}
						else
						{
							return scale * x;
						}
					}
					else
					{
						double x = -1 * Math.Log((bGS - p) / shape);
						double u2 = rng.MyNextDouble();

						if (u2 > Math.Pow(x, shape - 1))
						{
							continue;
						}
						else
						{
							return scale * x;
						}
					}
				}
			}

			double d = shape - 0.333333333333333333;
			double c = 1 / (3 * Math.Sqrt(d));

			while (true)
			{
				double x = rng.NextGaussian();
				double v = (1 + c * x) * (1 + c * x) * (1 + c * x);

				if (v <= 0)
				{
					continue;
				}

				double x2 = x * x;
				double u = rng.NextDouble();

				if (u < 1 - 0.0331 * x2 * x2)
				{
					return scale * d * v;
				}

				if (Math.Log(u) < 0.5 * x2 + d * (1 - v + Math.Log(v)))
				{
					return scale * d * v;
				}
			}
		}
	}
}