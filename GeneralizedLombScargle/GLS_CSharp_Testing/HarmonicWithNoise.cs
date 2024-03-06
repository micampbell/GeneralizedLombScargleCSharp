using GeneralizedLombScargle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLS_CSharp_Testing
{
    internal static class HarmonicWithNoise
    {
        internal static (double[] frequencies, double[] powers) Test(out IList<double> times, out IList<double> values)
        {
            var amplitude = 7.0;
            var phase = 3*Math.PI / 5;
            var frequency = 1.8;
            var offset = 4.5;
            var noiseAmplitude = 2;
            var rng = new Random();
            var n = 1000;
            times = new double[n];
            values = new double[n];
            for (int i = 0; i < n; i++)
            {
                times[i] = i / 500.0;
                values[i] = offset + amplitude * Math.Sin(2 * Math.PI * frequency * times[i] + phase) + noiseAmplitude * rng.NextDouble();
            }
            var periodogram = new Periodogram(0.0, 5, frequencyStepSize: 0.01);
            var powers = periodogram.CalculatePowers(times, values);

            var power = periodogram.GetLargestHarmonic(out var predictedFrequency, out var predictedAmplitude, out var predictedPhase, out var predictedOffset);
            Console.WriteLine("best power: " + power);
            Console.WriteLine("frequency: predicted = " + predictedFrequency + "  ;   actual = " + frequency);
            Console.WriteLine("amplitude: predicted = " + predictedAmplitude + "  ;   actual = " + amplitude);
            Console.WriteLine("phase: predicted = " + predictedPhase + "  ;   actual = " + phase);
            Console.WriteLine("offset: predicted = " + predictedOffset + "  ;   actual = " + offset);
            return (periodogram.Frequencies, powers);
        }
    }
}
