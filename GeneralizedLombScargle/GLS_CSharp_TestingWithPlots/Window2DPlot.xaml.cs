using OxyPlot;
using OxyPlot.Series;
using System.Numerics;
using System.Reflection;
using System.Windows;

namespace GLS_CSharp_TestingWithPlots
{

    /// <summary>
    ///     Class Window2DPlot.
    /// </summary>
    public partial class Window2DPlot : Window
    {
        public Window2DPlot()
        {
            Model = new PlotModel();
        }
        public PlotModel Model { get; set; }

        public static void ShowPlot(IEnumerable<IEnumerable<Vector2>> listOfArrayOfPoints,
            IEnumerable<bool> makeLines,
            IEnumerable<MarkerType> markers)
        {
            var window = new Window2DPlot();
            var makeLineEnumerator = makeLines.GetEnumerator();
            var markerEnumerator = markers.GetEnumerator();
            var colorEnumerator = GetOxyColors().GetEnumerator();
            foreach (var points in listOfArrayOfPoints)
            {
                colorEnumerator.MoveNext();
                var color = colorEnumerator.Current;
                makeLineEnumerator.MoveNext();
                var makeLine = makeLineEnumerator.Current;
                markerEnumerator.MoveNext();
                var marker = markerEnumerator.Current;
                if (makeLine)
                    AddLineSeriesToModel(points, marker, color, window.Model);
                else
                    AddScatterSeriesToModel(points, marker, color, window.Model);
            }
            window.InitializeComponent();
            window.ShowDialog();
        }

        /// <summary>
        ///     Adds the line series to model.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="closeShape">if set to <c>true</c> [close shape].</param>
        /// <param name="marker">The marker.</param>
        private static void AddLineSeriesToModel(IEnumerable<Vector2> points, MarkerType marker,
            OxyColor color, PlotModel model)
        {
            var series = new LineSeries
            {
                MarkerType = marker,
                Color = color
            };

            foreach (var point in points)
                series.Points.Add(new DataPoint(point.X, point.Y));
            model.Series.Add(series);
        }

        /// <summary>
        ///     Adds the scatter series to model.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="marker">The marker.</param>
        private static void AddScatterSeriesToModel(IEnumerable<Vector2> points, MarkerType marker, OxyColor color, PlotModel model)
        {
            var series = new LineSeries
            {
                MarkerType = marker,
                Color = color
            };
            foreach (var point in points)
                series.Points.Add(new DataPoint(point.X, point.Y));
            model.Series.Add(series);
        }

        private static IEnumerable<OxyColor> GetOxyColors()
        {
            var t = typeof(OxyColors);
            var r = new Random();
            return t.GetRuntimeFields().Where(fi => fi.IsPublic && fi.IsStatic && fi.GetValue(null) is OxyColor)
                    .OrderBy(fi => r.Next())
                    .Where(fi =>
                    {
                        var color = (OxyColor)fi.GetValue(null);
                        return color.R+color.G+color.B <200;
                    })
                    .Select(fi => (OxyColor)fi.GetValue(null));
        }
    }
}