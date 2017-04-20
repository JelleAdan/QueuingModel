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
            average = mu;
            variance = sigma * sigma;
        }

        public override double Next()
        {
            double tmp = new double();
            return tmp;
        }
    }
}
