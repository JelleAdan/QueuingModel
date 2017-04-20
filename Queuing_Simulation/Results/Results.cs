using Queuing_Simulation.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    public class Results
    {
        private int R { get; set; }
        private int nrServers { get; set; }
        private int nrCustomers { get; set; }
        private int[,] nrArrivals { get; set; }
        private int[,] nrDepartures { get; set; }
        private double[] time { get; set; }
        private double[] sumLs { get; set; }
        private double[] sumLq { get; set; }
        private double[] sumS { get; set; }
        private double[] sumW { get; set; }
        private double[] wait { get; set; }
        private Dictionary<List<int>, int>[,] configurations { get; set; }

        public Results(int R, int nrServers, int nrCustomers)
        {
            this.R = R;
            this.nrServers = nrServers;
            this.nrCustomers = nrCustomers;
            nrArrivals = new int[R, nrCustomers];
            nrDepartures = new int[R, nrCustomers];
            time = new double[R];
            sumLs = new double[R];
            sumLq = new double[R];
            sumS = new double[R];
            sumW = new double[R];
            wait = new double[R];
            configurations = new Dictionary<List<int>, int>[R, nrCustomers];
            for(int r = 0; r < R; r++)
            {
                for(int i = 0; i < nrCustomers; i++)
                {
                    configurations[r, i] = new Dictionary<List<int>, int>(new ListComparer<int>());
                }
            }
        }

        public void Register(int r, Event e, CustomerQueue customerQueue, ServerQueue idleServerQueue)
        {
            sumLs[r] += customerQueue.GetLength() * (e.time - time[r]);
            sumLq[r] += (customerQueue.GetLength() - (nrServers - idleServerQueue.GetLength())) * (e.time - time[r]);
            if (e.type == Event.ARRIVAL)
            {
                nrArrivals[r, e.customer.cID]++;
                List<int> tmp = new List<int>();
                foreach(Customer customer in customerQueue.list)
                {
                    if(customer.server != null)
                    {
                        tmp.Add(customer.server.sID);
                    }
                    if(tmp.Count == (nrServers - idleServerQueue.list.Count))
                    {
                        break;
                    }
                }
                if (configurations[r, e.customer.cID].ContainsKey(tmp))
                {
                    configurations[r, e.customer.cID][tmp]++;
                }
                else
                {
                    configurations[r, e.customer.cID].Add(tmp, 1);
                }
            }
            else if (e.type == Event.DEPARTURE) 
            {
                nrDepartures[r, e.customer.cID]++;
                sumS[r] += e.customer.departureTime - e.customer.arrivalTime;
                sumW[r] += e.customer.serviceTime - e.customer.arrivalTime;
                if (e.customer.arrivalTime != e.customer.serviceTime)
                {
                    wait[r]++;
                }
            }
            time[r] = e.time;
        }

        public void GetMeans(double lambda = 0.5, double mu = 0.5)
        {
            int[] nrArrivalsTotal = new int[R];
            int[] nrDeparturesTotal = new int[R];
            for (int r = 0; r < R; r++)
            {
                for(int i = 0; i < nrCustomers; i++)
                {
                    nrArrivalsTotal[r] += nrArrivals[r, i];
                    nrDeparturesTotal[r] += nrDepartures[r, i];
                }
            }

            double runsumLs = new double();
            double runsumLs2 = new double();
            double runsumLq = new double();
            double runsumLq2 = new double();
            double runsumS = new double();
            double runsumS2 = new double();
            double runsumW = new double();
            double runsumW2 = new double();
            double runsumPW = new double();
            double runsumPW2 = new double();

            for(int r = 0; r < R; r++)
            {
                runsumLs += sumLs[r] / time[r];
                runsumLs2 += sumLs[r] / time[r] * sumLs[r] / time[r];
                runsumLq += sumLq[r] / time[r];
                runsumLq2 += sumLq[r] / time[r] * sumLq[r] / time[r];
                runsumS += sumS[r] / nrDeparturesTotal[r];
                runsumS2 += sumS[r] / nrDeparturesTotal[r] * sumS[r] / nrDeparturesTotal[r];
                runsumW += sumW[r] / nrDeparturesTotal[r];
                runsumW2 += sumW[r] / nrDeparturesTotal[r] * sumW[r] / nrDeparturesTotal[r];
                runsumPW += wait[r] / nrDeparturesTotal[r];
                runsumPW2 += wait[r] / nrDeparturesTotal[r] * wait[r] / nrDeparturesTotal[r];
            }

            double meanLs = runsumLs / R;
            double sigmaLs = Math.Sqrt(runsumLs2 / R - meanLs * meanLs);
            double ciLs = 1.96 * sigmaLs / Math.Sqrt(R);

            double meanLq = runsumLq / R;
            double sigmaLq = Math.Sqrt(runsumLq2 / R - meanLq * meanLq);
            double ciLq = 1.96 * sigmaLq / Math.Sqrt(R);

            double meanS = runsumS / R;
            double sigmaS = Math.Sqrt(runsumS2 / R - meanS * meanS);
            double ciS = 1.96 * sigmaS / Math.Sqrt(R);

            double meanW = runsumW / R;
            double sigmaW = Math.Sqrt(runsumW2 / R - meanW * meanW);
            double ciW = 1.96 * sigmaW / Math.Sqrt(R);

            double meanPW = runsumPW / R;
            double sigmaPW = Math.Sqrt(runsumPW2 / R - meanPW * meanPW);
            double ciPW = 1.96 * sigmaPW / Math.Sqrt(R);

            // Theoretic M|M|c
            double rho = lambda / mu / nrServers;
            double tmp = new double();
            if (nrServers > 1)
            {
                for (int k = 0; k < nrServers; k++)
                {
                    tmp += Math.Pow(nrServers * rho, k) / Factorial(k);
                }
            }
            else
            {
                tmp = 1;
            }
            double theoreticPW = 1 / (1 + (1 - rho) * Factorial(nrServers) / Math.Pow(nrServers * rho, nrServers) * tmp);
            theoreticPW = Math.Pow(nrServers * rho, nrServers) / Factorial(nrServers) / ((1 - rho) * tmp + Math.Pow(nrServers * rho, nrServers) / Factorial(nrServers));
            double theoreticLq = theoreticPW * rho / (1 - rho);
            double theoreticLs = theoreticLq + nrServers * rho;
            double theoreticW = theoreticPW / (1 - rho) / (nrServers * mu);
            theoreticW = theoreticPW / (nrServers * mu) + theoreticLq / (nrServers * mu);
            double theoreticS = (rho / (1 - rho) * theoreticPW + nrServers * rho) / lambda;

            Console.WriteLine("\t\t\t\tSimulation\t\t\tTheoretic");
            Console.WriteLine("E(customers in the system):\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.0000}", meanLs), String.Format("{0:0.0000}", ciLs), String.Format("{0:0.0000}", theoreticLs));
            Console.WriteLine("E(customers in the queue):\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.0000}", meanLq), String.Format("{0:0.0000}", ciLq), String.Format("{0:0.0000}", theoreticLq));
            Console.WriteLine("E(sojourn time):\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.0000}", meanS), String.Format("{0:0.0000}", ciS), String.Format("{0:0.0000}", theoreticS));
            Console.WriteLine("E(waiting time):\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.0000}", meanW), String.Format("{0:0.0000}", ciW), String.Format("{0:0.0000}", theoreticW));
            Console.WriteLine("P(wait):\t\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.0000}", meanPW), String.Format("{0:0.0000}", ciPW), String.Format("{0:0.0000}", theoreticPW));
            
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            // Second argument StreamWriter: true = append / false = overwrite
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(string.Concat(Environment.CurrentDirectory, "/Visualization/results.txt"), false))
            {
                file.WriteLine("\t\t\t\tSimulation\t\t\t\tTheoretic");
                file.WriteLine("E(customers in the system):\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.00000000}", meanLs), String.Format("{0:0.00000000}", ciLs), String.Format("{0:0.00000000}", theoreticLs));
                file.WriteLine("E(customers in the queue):\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.00000000}", meanLq), String.Format("{0:0.00000000}", ciLq), String.Format("{0:0.00000000}", theoreticLq));
                file.WriteLine("E(sojourn time):\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.00000000}", meanS), String.Format("{0:0.00000000}", ciS), String.Format("{0:0.00000000}", theoreticS));
                file.WriteLine("E(waiting time):\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.00000000}", meanW), String.Format("{0:0.00000000}", ciW), String.Format("{0:0.00000000}", theoreticW));
                file.WriteLine("P(wait):\t\t\t{0} \u00B1 {1} (95% C.I.)\t{2}", String.Format("{0:0.00000000}", meanPW), String.Format("{0:0.00000000}", ciPW), String.Format("{0:0.00000000}", theoreticPW));
            }

            //TEST
            PrintConfigurations();
        }

        public void PrintConfigurations()
        {
            // Translate configuration count to probability
            for (int r = 0; r < R; r++)
            {
                for (int cID = 0; cID < nrCustomers; cID++)
                {
                    foreach (KeyValuePair<List<int>, int> configuration in configurations[r, cID])
                    {
                        configurations[r, cID][configuration.Key] = configurations[r, cID][configuration.Key] / nrArrivals[r, cID];
                    }
                }
            }

            for (int r = 0; r < R; r++)
            {
                for(int cID = 0; cID < nrCustomers; cID++)
                {

                }
                
            }
        }

        private static int Factorial(int number)
        {
            if (number == 0) { return 1; } // 0! is defined as equal to 1
            int factorial = number;
            for (int i = number - 1; i > 0; i--)
            {
                factorial *= i;
            }
            return factorial;
        }

        public class ListComparer<T> : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<T> obj)
            {
                int hashcode = 0;
                foreach (T t in obj)
                {
                    hashcode ^= t.GetHashCode();
                }
                return hashcode;
            }
        }
    }
}
