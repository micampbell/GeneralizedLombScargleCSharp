// See https://aka.ms/new-console-template for more information
using GLS_CSharp_Testing;
using System.Numerics;

Console.WriteLine(" ----------------- Testing for GLS Periodogram -----------------");

Console.WriteLine("\n\n\n Multiharmonics Test");
(double[] frequencies, double[] powers) = MultiHarmonics.Test(out var times, out var values);

Console.WriteLine("\n\n\n Harmonic With Noise Test");
(frequencies, powers) = HarmonicWithNoise.Test(out times, out values);

Console.WriteLine("\n\n\n Data Series Test");
(frequencies, powers) = DataSeriesExample.Test(out times, out values);

