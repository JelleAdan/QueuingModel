using Queuing_Simulation.Objects;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0}QUEUING SIMULATION\n{0}", new String('\u2500', 80));

            Console.WriteLine("Simulation started...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (double lambda = 0; lambda < 10; lambda = lambda + 0.2)
            {
                Random rng = new Random();

                // Initialize customers
                double[] arrivalDistributionParameters = new double[] { lambda };
                int nrCustomers = arrivalDistributionParameters.Length;
                Distribution[] arrivalDistributions = new Distribution[nrCustomers];
                for (int i = 0; i < nrCustomers; i++)
                {
                    arrivalDistributions[i] = new ExponentialDistribution(rng, arrivalDistributionParameters[i]);
                }

                // Initialize servers
                double[] serviceDistributionParameters = new double[] { 2.5, 2.5, 2.5, 2.5 };
                int nrServers = serviceDistributionParameters.Length;
                Distribution[] serviceDistributions = new Distribution[nrServers];
                for (int i = 0; i < nrServers; i++)
                {
                    serviceDistributions[i] = new ExponentialDistribution(rng, serviceDistributionParameters[i]);
                }

                // Determine utilization M|G|c 
                double utilization = arrivalDistributionParameters[0] * serviceDistributions[0].average / nrServers;

                // Server-Customer Eligibility
                bool[][] eligibility = new bool[nrServers][];
                for (int i = 0; i < nrServers; i++)
                {
                    bool[] tmp = new bool[nrCustomers];
                    for (int j = 0; j < nrCustomers; j++)
                    {
                        tmp[j] = true;
                    }
                    eligibility[i] = tmp;
                }

                // Runs (Number and time span resp.)
                int R = 6;
                double T = 10E3;

                Results results = new Results(R, nrServers, nrCustomers);

                Parallel.For(0, R, new ParallelOptions { MaxDegreeOfParallelism = 3 }, r => // MaxDegreeOfParallelism n + 1, where n is the number of cores
                //for (int r = 0; r < R; r++)
                {
                    double t = 0;

                    FutureEvents futureEvents = new FutureEvents();
                    for (int i = 0; i < nrCustomers; i++)
                    {
                        Customer customer = new Customer(i);
                        futureEvents.Add(new Event(Event.ARRIVAL, arrivalDistributions[i].Next(), customer));
                    }

                    CustomerQueue customerQueue = new CustomerQueue();

                    ServerQueue idleServerQueue = new ServerQueue();
                    for (int i = 0; i < nrServers; i++)
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
                        //Console.Write("\rRun {0}/{1}\t\tProgress {2}%\t Elapsed time {3} seconds", r + 1, R, Math.Round(t / T * 100, 0), stopwatch.ElapsedMilliseconds / 1000);
                    }
                    //}
                });
                results.GetMeans(utilization, arrivalDistributions, serviceDistributions, false);
                Console.Write("\rUtilization: {0:0.00}", utilization);
            }

            Console.WriteLine("\nSimulation complete.\nTotal elapsed time {0:0.00} minutes", stopwatch.ElapsedMilliseconds / 1000 / 60);

            //Visualize:
            //Console.WriteLine("Visualize results? (yes/no)");
            //switch (Console.ReadLine())
            //{
            //    case "yes":
            //        Visualization.VisualizeTIKZ.PrintCSV();
            //        Visualization.VisualizeTIKZ.Visualize();
            //        break;
            //    case "no":
            //        break;
            //    default:
            //        Console.Write("Invalid answer. ");
            //        goto Visualize;
            //}

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadLine();
        }
    }
}
