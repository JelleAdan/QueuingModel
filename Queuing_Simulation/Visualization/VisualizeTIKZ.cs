using System;
using System.Diagnostics;

namespace Queuing_Simulation.Visualization
{
    public class VisualizeTIKZ
    {
        public static void PrintCSV()
        {
            // Dummy graph
            double[] x = new double[10];
            double[] y = new double[10];
            for(int i = 0; i < 10; i++)
            {
                x[i] = i;
                y[i] = x[i];
            }

            // The using statement automatically flushes AND CLOSES the stream and calls IDisposable.Dispose on the stream object
            // NOTE: do not use FileStream for text files because it writes bytes, but StreamWriter encodes the output as text
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(string.Concat(Environment.CurrentDirectory, "/Visualization/test.csv")))
            {
                for (int i = 0; i < 10; i++)
                {
                    file.WriteLine(x[i] + ", " + y[i]);
                }
            }
        }

        public static void Visualize()
        {
            // Texify
            Process texify = new Process();
            texify.StartInfo.WorkingDirectory = string.Concat(Environment.CurrentDirectory, "/Visualization/");
            texify.StartInfo.FileName = "cmd.exe";
            texify.StartInfo.Arguments = "/C texify.exe --pdf --batch --tex-option=--interaction=nonstopmode --tex-option=--synctex=-1 \"" + texify.StartInfo.WorkingDirectory + "Visualization.tex\"";
            texify.Start();
            texify.WaitForExit();
            // Open PDF
            Process openPDF = new Process();
            openPDF.StartInfo.FileName = string.Concat(Environment.CurrentDirectory, "/Visualization/Visualization.pdf");
            openPDF.Start();
            // Clean up
            Process clean = new Process();
            clean.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            clean.StartInfo.WorkingDirectory = string.Concat(Environment.CurrentDirectory, "/Visualization/");
            clean.StartInfo.FileName = "cmd.exe";
            clean.StartInfo.Arguments = "/C del Visualization.aux Visualization.bbl Visualization.blg Visualization.log Visualization.synctex";
            clean.Start();
        }
    }
}
