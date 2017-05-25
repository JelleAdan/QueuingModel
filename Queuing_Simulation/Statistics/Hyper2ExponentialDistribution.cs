using System;

namespace Queuing_Simulation
{
    public class Hyper2ExponentialDistribution : Distribution
    {
        private double[] lambda { get; set; }
        private double[] prob { get; set; }

        public Hyper2ExponentialDistribution(Random rng, double average, double cvar)
        {
            this.rng = rng;
            prob = new double[2];
            prob[0] = 0.5 * (1 + Math.Sqrt((Math.Pow(cvar, 2) - 1) / (Math.Pow(cvar, 2) + 1))); // Balanced means
            prob[1] = 1 - prob[0];
            lambda = new double[2];
            lambda[0] = 2 * prob[0] / average;
            lambda[1] = 2 * prob[1] / average;
            this.average = average;
            variance = 2 * (prob[0] / lambda[0] / lambda[0] + prob[1] / lambda[1] / lambda[1]) - average * average;
            this.cvar = cvar;
            residual = (variance + average * average) / (2 * average);
        }

        public override double Next()
        {
            double a = rng.NextDouble();
            int index = 0;
            if(a < prob[0])
            {
                index = 0;
            }
            else
            {
                index = 1;
            }
            //for (int i = 0; i < prob.Length; i++)
            //{
            //    if (a < prob[i])
            //    {
            //        index = i;
            //        break;
            //    }
            //}
            return -Math.Log(rng.MyNextDouble()) / lambda[index];
        }
    }
}