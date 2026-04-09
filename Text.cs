using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Autodesk.DesignScript.Geometry;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace DynamoText
{
    public static class Text
    {
        public static IEnumerable<Curve> FromStringOriginAndScale(
            string text,
            Point origin,
            double scale,
            string fontFamily = "Arial",
            bool bold = false,
            bool italic = false)
        {
            //http://msdn.microsoft.com/en-us/library/ms745816(v=vs.110).aspx

            var crvs = new List<Curve>();

            bool fontExists = Fonts.SystemFontFamilies
                .Any(f => f.Source.Equals(fontFamily, StringComparison.OrdinalIgnoreCase));
            if (!fontExists)
            {
                throw new ArgumentException(
                    string.Format("Font family \"{0}\" is not installed on this system. " +
                        "Use Text.GetInstalledFontNames() to list available fonts.", fontFamily));
            }

            var font = new System.Windows.Media.FontFamily(fontFamily);
            var fontStyle = italic ? FontStyles.Italic : FontStyles.Normal;
            var fontWeight = bold ? FontWeights.Bold : FontWeights.Medium;

            var formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(
                    font,
                    fontStyle,
                    fontWeight,
                    FontStretches.Normal),
                1,
                System.Windows.Media.Brushes.Black,
                1.0);

            // Build the geometry object that represents the text.
            var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
            foreach (var figure in textGeometry.GetFlattenedPathGeometry().Figures)
            {
                var init = figure.StartPoint;
                var a = figure.StartPoint;
                System.Windows.Point b;
                foreach (var segment in figure.GetFlattenedPathFigure().Segments)
                {
                    var lineSeg = segment as LineSegment;
                    if (lineSeg != null)
                    {
                        b = lineSeg.Point;
                        var crv = LineBetweenPoints(origin, scale, a, b);
                        a = b;
                        crvs.Add(crv);
                    }

                    var plineSeg = segment as PolyLineSegment;
                    if (plineSeg != null)
                    {
                        foreach (var segPt in plineSeg.Points)
                        {
                            var crv = LineBetweenPoints(origin, scale, a, segPt);
                            a = segPt;
                            crvs.Add(crv);
                        }
                    }

                }
            }

            return crvs;
        }

        public static IList<string> GetInstalledFontNames()
        {
            return Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static Line LineBetweenPoints(Point origin, double scale, System.Windows.Point a, System.Windows.Point b)
        {
            var pt1 = Point.ByCoordinates((a.X * scale) + origin.X, ((-a.Y + 1) * scale) + origin.Y, origin.Z);
            var pt2 = Point.ByCoordinates((b.X * scale) + origin.X, ((-b.Y + 1) * scale) + origin.Y, origin.Z);
            var crv = Line.ByStartPointEndPoint(pt1, pt2);
            return crv;
        }
    }
}
