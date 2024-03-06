using GeneralizedLombScargle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GLS_CSharp_Testing
{
    internal static class MultiHarmonics
    {
        internal static (double[] frequencies, double[] powers) Test(out IList<double> times, out IList<double> values)
        {
            var frequencies = new[] { 1.1, 9.9, 40.0 };
            var amplitudes = new[] { 7.3, 4.4, 2.1 };
            var phases = new[] { 2.0, 1.0, 0.5 };

            var maxAt = Enumerable.Range(0, amplitudes.Length).MaxBy(i => amplitudes[i]);
            var maxFrequency = frequencies[maxAt];
            var maxAmplitude = amplitudes[maxAt];
            var maxPhase = phases[maxAt];


            var offset = -3.0;
            var noiseAmplitude = 0.000025;
            var rng = new Random();
            var n = 1000;
            times = new double[n];
            values = new double[n];
            for (int i = 0; i < n; i++)
            {
                times[i] = i / 100.0;
                for (int j = 0; j < amplitudes.Length; j++)
                {
                    values[i] += amplitudes[j] * Math.Sin(2 * Math.PI *frequencies[j] * times[i] + phases[j]);
                }
                values[i] += offset + noiseAmplitude * rng.NextDouble();
            }
            var periodogram = new Periodogram(0.0, 50, frequencyStepSize: 0.01);
            var powers = periodogram.CalculatePowers(times, values);

            var power = periodogram.GetLargestHarmonic(out var predictedFrequency, out var predictedAmplitude, out var predictedPhase, out var predictedOffset);
            Console.WriteLine("best power: " + power);
            Console.WriteLine("frequency: predicted = " + predictedFrequency + "  ;   actual = " + maxFrequency);
            Console.WriteLine("amplitude: predicted = " + predictedAmplitude + "  ;   actual = " + maxAmplitude);
            Console.WriteLine("phase: predicted = " + predictedPhase + "  ;   actual = " + maxPhase);
            Console.WriteLine("offset: predicted = " + predictedOffset + "  ;   actual = " + offset);
            return (periodogram.Frequencies, powers);
        }
    }
}
