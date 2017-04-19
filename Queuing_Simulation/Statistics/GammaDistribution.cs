using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    public class GammaDistribution : Distribution
    {
        private double alpha;
        private double beta;
        public GammaDistribution(Random rng, double alpha, double beta)
        {
            this.rng = rng;
            this.alpha = alpha;
            this.beta = beta;
        }

        public double GetAverage()
        {
            average = alpha / beta;
            return average;
        }

        public double GetVariance()
        {
            variance = alpha / beta / beta;
            return variance;
        }

        public double Next()
        {
            int k = (int)Math.Floor(alpha);
            double a = alpha - k;
            double y = 0;
            double e = Math.E;
            if (a > 0)
            {
                while (y == 0)
                {
                    double u = rng.NextDouble();
                    double p = u * (e + a) / e;
                    if (p <= 1)
                    {
                        double x = Math.Pow(p, 1 / a);
                        double u1 = rng.NextDouble();
                        if (u1 <= Math.Exp(-x))
                        { // Accept
                            y = x;
                        }
                    }
                    else
                    {
                        double x = -Math.Log(p / a);
                        double u1 = rng.NextDouble();
                        if (u1 <= Math.Pow(x, a - 1))
                        { // Accept
                            y = x;
                        }
                    }
                }
            }
            double product = 1;
            for (int i = 0; i < k; i++)
            {
                product = product * rng.NextDouble();
            }
            return ((y - Math.Log(product)) / beta);
        }
    }
}
