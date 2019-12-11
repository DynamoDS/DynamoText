﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Autodesk.DesignScript.Geometry;
using Point = Autodesk.DesignScript.Geometry.Point;
using DynamoServices;

namespace DynamoText
{
    public static class Text
    {
        [Obsolete("Try the new ByStringFontToleranceOriginScale node, which allows specifying the font and faceting tolerance, and organizes curves by letter.")]
        public static IEnumerable<Curve> FromStringOriginAndScale(string text, Point origin, double scale)
        {
            //http://msdn.microsoft.com/en-us/library/ms745816(v=vs.110).aspx

            var crvs = new List<Curve>();

            var font = new System.Windows.Media.FontFamily("Arial");
            var fontStyle = FontStyles.Normal;
            var fontWeight = FontWeights.Medium;

            //if (Bold == true) fontWeight = FontWeights.Bold;
            //if (Italic == true) fontStyle = FontStyles.Italic;

            // Create the formatted text based on the properties set.
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
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text. 
                );

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

        public static IEnumerable<IEnumerable<Curve>> ByStringFontToleranceOriginScale(string text, string fontName, float tolerance, Point origin, double scale)
        {
            //http://msdn.microsoft.com/en-us/library/ms745816(v=vs.110).aspx

            // make a two-dimensional list: each letter, and its constituent curves
            List<List<Curve>> stringCurves = new List<List<Curve>>();

            var font = new System.Windows.Media.FontFamily(fontName);
            var fontStyle = FontStyles.Normal;
            var fontWeight = FontWeights.Medium;

            //if (Bold == true) fontWeight = FontWeights.Bold;
            //if (Italic == true) fontStyle = FontStyles.Italic;

            // Create the formatted text based on the properties set.
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
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text. 
                );

            // Build the geometry object that represents the text.
            var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));

            //for (var i = 0; i < textGeometry.GetFlattenedPathGeometry(tolerance, System.Windows.Media.ToleranceType.Absolute).Figures.Count; i ++)
            foreach (var figure in textGeometry.GetFlattenedPathGeometry(tolerance, System.Windows.Media.ToleranceType.Absolute).Figures)
            {
                var init = figure.StartPoint;
                var a = figure.StartPoint;
                System.Windows.Point b;
                foreach (var segment in figure.GetFlattenedPathFigure(tolerance, System.Windows.Media.ToleranceType.Absolute).Segments)
                {

                    // the curves of each letter
                    var letterCurves = new List<Curve>();

                    var lineSeg = segment as LineSegment;
                    if (lineSeg != null)
                    {
                        b = lineSeg.Point;
                        var crv = LineBetweenPoints(origin, scale, a, b);
                        a = b;
                        letterCurves.Add(crv);
                    }

                    var plineSeg = segment as PolyLineSegment;
                    if (plineSeg != null)
                    {
                        foreach (var segPt in plineSeg.Points)
                        {
                            var crv = LineBetweenPoints(origin, scale, a, segPt);
                            a = segPt;
                            letterCurves.Add(crv);
                        }
                    }

                    stringCurves.Add(letterCurves);
                }
            }

            return stringCurves;
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
