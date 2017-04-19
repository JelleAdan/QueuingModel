using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    public class Distribution
    {
        public Random rng { get; set; }
        public double average { get; set; }
        public double variance { get; set; }
    }
}
