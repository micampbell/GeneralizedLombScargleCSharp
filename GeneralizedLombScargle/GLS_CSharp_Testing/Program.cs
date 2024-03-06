// See https://aka.ms/new-console-template for more information
using GLS_CSharp_Testing;

Console.WriteLine(" ----------------- Testing for GLS Periodogram -----------------");

(double[] frequencies, double[] powers) = HarmonicWithNoise.Test();