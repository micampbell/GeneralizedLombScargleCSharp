using GeneralizedLombScargle;
using System.IO;

namespace GLS_CSharp_Testing
{
    internal static class DataSeriesExample
    {
        internal static (double[] frequencies, double[] powers) Test(out IList<double> times, 
            out IList<double> values)
        {
            FileInfo[] files;
            var dir = new DirectoryInfo(".");
            do
            {
                 files = dir.GetFiles("*.csv");
                if (files.Length > 0)
                    break;
                dir = dir.Parent;
            }
            while (dir != null);
             times  = new List<double>();
            values = new List<double>();
            var errors = new List<double>();
            foreach (var file in files)
            {
                Console.WriteLine(file.FullName);
                var lines=  File.ReadAllLines(file.FullName);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    times.Add(double.Parse(parts[0]));
                    values.Add(double.Parse(parts[1]));
                    errors.Add(double.Parse(parts[2]));
                }
            }
            var periodogram = new Periodogram(0.001, 0.008, numberOfFrequencySteps: 5000);
            var powers = periodogram.CalculatePowers(times, values);

            var power = periodogram.GetLargestHarmonic(out var predictedFrequency, out var predictedAmplitude, out var predictedPhase, out var predictedOffset);
            Console.WriteLine("best power: " + power);
            Console.WriteLine("frequency: predicted = " + predictedFrequency );
            Console.WriteLine("amplitude: predicted = " + predictedAmplitude);
            Console.WriteLine("phase: predicted = " + predictedPhase);
            Console.WriteLine("offset: predicted = " + predictedOffset);
            return (periodogram.Frequencies, powers);
        }
    }
}
