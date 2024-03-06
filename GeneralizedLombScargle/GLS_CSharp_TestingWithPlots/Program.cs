// See https://aka.ms/new-console-template for more information
using GLS_CSharp_Testing;
using GLS_CSharp_TestingWithPlots;
using OxyPlot;
using System.Numerics;

ActivateConsoleToCreateWindows();

Console.WriteLine("========= Testing for GLS Periodogram With Plotting ========= ");


Console.WriteLine("\n\n\n Multiharmonics Test");
(double[] frequencies, double[] powers) = MultiHarmonics.Test(out var times, out var values);

var plotPoints = times.Zip(values, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);

plotPoints = frequencies.Zip(powers, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);


Console.WriteLine("\n\n\n Harmonic With Noise Test");
(frequencies, powers) = HarmonicWithNoise.Test(out times, out values);

plotPoints = times.Zip(values, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);

plotPoints = frequencies.Zip(powers, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);



Console.WriteLine("\n\n\n Data Series Test");
(frequencies, powers) = DataSeriesExample.Test(out times, out values);

 plotPoints = times.Zip(values, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);

plotPoints = frequencies.Zip(powers, (x, y) => new Vector2((float)x, (float)y)).ToArray();
Window2DPlot.ShowPlot([plotPoints], [true], [MarkerType.Circle]);





void ActivateConsoleToCreateWindows()
{
    Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
}