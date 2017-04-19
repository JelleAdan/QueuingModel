﻿using Queuing_Simulation.Objects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0}QUEUING SIMULATION \n{1}\nInitialization started...", new String('\u2500', 80), new String('\u2500', 80));

            // Number of runs
            int R = 5;

            Random rng = new Random();

            // Initialize customers
            double[] arrivalDistributionParameters = new double[] { 2 };
            int customerTypes = arrivalDistributionParameters.Length;
            ExponentialDistribution[] arrivalDistributions = new ExponentialDistribution[arrivalDistributionParameters.GetLength(0)];
            for (int i = 0; i < customerTypes; i++)
            {
                arrivalDistributions[i] = new ExponentialDistribution(rng, arrivalDistributionParameters[i]);
            }

            // Initialize servers
            double[] serviceDistributionParameters = new double[] { 1, 1, 1, 1 };
            int serverTypes = serviceDistributionParameters.Length;
            ExponentialDistribution[] serviceDistributions = new ExponentialDistribution[serviceDistributionParameters.GetLength(0)];
            for (int i = 0; i < serverTypes; i++)
            {
                serviceDistributions[i] = new ExponentialDistribution(rng, serviceDistributionParameters[i]);
            }
            

            // Server-Customer Eligibility
            bool[][] eligibility = new bool[serverTypes][];
            for (int i = 0; i < serverTypes; i++) 
            {
                bool[] tmp = new bool[customerTypes];
                for (int j = 0; j < customerTypes; j++) 
                {
                    tmp[j] = true;
                }
                eligibility[i] = tmp;
            }

            // Time span of a run
            double T = 10E3;

            Console.WriteLine("Initialization complete. \nSimulation started...");
            Stopwatch stopwatch = new Stopwatch();
            double elapsedTime = new double();

            Results results = new Results(R, serverTypes, customerTypes);

            // Number of threads n + 1, where n is the number of cores
            Parallel.For(0, R, new ParallelOptions { MaxDegreeOfParallelism = 3 }, r =>
            //for (int r = 0; r < R; r++)
            {
                stopwatch.Start();

                double t = 0;

                FutureEvents futureEvents = new FutureEvents();
                for (int i = 0; i < customerTypes; i++) 
                {
                    Customer customer = new Customer(i);
                    futureEvents.Add(new Event(Event.ARRIVAL, arrivalDistributions[i].Next(), customer));
                }

                CustomerQueue customerQueue = new CustomerQueue();

                ServerQueue idleServerQueue = new ServerQueue();
                for (int i = 0; i < serverTypes; i++) 
                {
                    Server server = new Server(i, eligibility[i]);
                    idleServerQueue.Add(server);
                }
                
                while (t < T)
                {
                    Event e = futureEvents.Next();
                    t = e.time;

                    results.Register(r, e, customerQueue, idleServerQueue);

                    if (e.type == Event.ARRIVAL)
                    {
                        customerQueue.CustomerCheckIn(e.customer, t);
                        Server server = idleServerQueue.FindServer(e.customer.cID);
                        if (server != null) // Eligible server available
                        {
                            idleServerQueue.Remove(server);
                            e.customer.server = server;
                            e.customer.serviceTime = t; 
                            e.customer.departureTime = t + serviceDistributions[server.sID].Next();
                            futureEvents.Add(new Event(Event.DEPARTURE, e.customer.departureTime, e.customer));
                        }
                        futureEvents.Add(new Event(Event.ARRIVAL, t + arrivalDistributions[e.customer.cID].Next(), new Customer(e.customer.cID)));
                    }
                    else if (e.type == Event.DEPARTURE) 
                    {
                        Customer customer = customerQueue.FindCustomer(e.customer.server);
                        if (customer != null)
                        {
                            customer.serviceTime = t;
                            customer.departureTime = t + serviceDistributions[customer.server.sID].Next();
                            futureEvents.Add(new Event(Event.DEPARTURE, customer.departureTime, customer));
                        }
                        else
                        {
                            idleServerQueue.Add(e.customer.server);
                        }
                        customerQueue.CustomerCheckOut(e.customer);
                    }
                    else
                    {
                        Console.WriteLine("Invalid event type.");
                    }
                    // Show progress in console
                    Console.Write("\rRun {0}/{1}\t\tProgress {2}%\t Elapsed time {3} seconds", r + 1, R, Math.Round(t / T * 100, 0), stopwatch.ElapsedMilliseconds / 1000);
                }
                Console.Write("\n");
                elapsedTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
                //}
            });

            Console.WriteLine("Simulation complete. Total elapsed time {0} minutes", elapsedTime / 1000 / 60);
            Console.WriteLine("\n{0}RESULTS\n{0}", new String('\u2500', 80), new String('\u2500', 80));
            results.GetMeans(arrivalDistributionParameters[0], serviceDistributionParameters[0]);
            Console.WriteLine("\n{0}", new String('\u2500', 80));
            Visualize:
            Console.WriteLine("Visualize results? (yes/no)");
            switch (Console.ReadLine())
            {
                case "yes":
                    Visualization.VisualizeTIKZ.PrintCSV();
                    Visualization.VisualizeTIKZ.Visualize();
                    break;
                case "no":
                    break;
                default:
                    Console.Write("Invalid answer. ");
                    goto Visualize;
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}