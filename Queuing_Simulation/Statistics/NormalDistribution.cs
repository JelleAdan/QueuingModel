using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    public class NormalDistribution : Distribution
    {
        private double mu;
        private double sigma;
        public NormalDistribution(Random rng, double mu, double sigma)
        {
            this.rng = rng;
            this.mu = mu;
            this.sigma = sigma;
        }

        public double GetAverage()
        {
            average = mu;
            return average;
        }

        public double GetVariance()
        {
            variance = sigma * sigma;
            return variance;
        }

        public double Next()
        {
            double tmp = new double();
            return tmp;
        }
    }
}
