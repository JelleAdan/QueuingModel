using System;

namespace Queuing_Simulation
{
	public abstract class Distribution
    {
        public Random rng { get; set; }
        public double average { get; set; }
        public double variance { get; set; }
        public abstract double Next();
    }
}