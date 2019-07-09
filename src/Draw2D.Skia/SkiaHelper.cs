﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D
{
    internal class SkiaHelper
    {
        internal static void GetTransform(StretchMode mode, SKRect element, SKRect panel, out double ox, out double oy, out double zx, out double zy)
        {
            ox = element.Left;
            oy = element.Top;
            zx = 1.0;
            zy = 1.0;
            switch (mode)
            {
                default:
                case StretchMode.None:
                    break;
                case StretchMode.Center:
                    ox = element.Left + (element.Width - panel.Width) / 2;
                    oy = element.Top + (element.Height - panel.Height) / 2;
                    break;
                case StretchMode.Fill:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
                case StretchMode.Uniform:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    zx = Math.Min(zx, zy);
                    zy = zx;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
                case StretchMode.UniformToFill:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    zx = Math.Max(zx, zy);
                    zy = zx;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
            }
        }

        internal static SKFontStyleSlant ToSKFontStyleSlant(FontStyleSlant slant)
        {
            switch (slant)
            {
                default:
                case FontStyleSlant.Upright:
                    return SKFontStyleSlant.Upright;
                case FontStyleSlant.Italic:
                    return SKFontStyleSlant.Italic;
                case FontStyleSlant.Oblique:
                    return SKFontStyleSlant.Oblique;
            }
        }

        internal static SKTypeface ToSKTypeface(Typeface typeface)
        {
            return SKTypeface.FromFamilyName(
                typeface.FontFamily,
                typeface.FontWeight,
                typeface.FontWidth,
                ToSKFontStyleSlant(typeface.FontSlant));
        }

        internal static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        internal static SKStrokeCap ToSKStrokeCap(StrokeCap cap)
        {
            switch (cap)
            {
                default:
                case StrokeCap.Butt:
                    return SKStrokeCap.Butt;
                case StrokeCap.Round:
                    return SKStrokeCap.Round;
                case StrokeCap.Square:
                    return SKStrokeCap.Square;
            }
        }

        internal static SKStrokeJoin ToSKStrokeJoin(StrokeJoin join)
        {
            switch (join)
            {
                default:
                case StrokeJoin.Miter:
                    return SKStrokeJoin.Miter;
                case StrokeJoin.Round:
                    return SKStrokeJoin.Round;
                case StrokeJoin.Bevel:
                    return SKStrokeJoin.Bevel;
            }
        }

        internal static SKPaint ToSKPaintPen(ShapeStyle style, double scale)
        {
            double strokeWidth = style.StrokeWidth;
            double strokeMiter = style.StrokeMiter;
            if (style.IsScaled)
            {
                strokeWidth /= scale;
                strokeMiter /= scale;
            }
            return new SKPaint()
            {
                IsAntialias = style.IsAntialias,
                IsStroke = true,
                StrokeWidth = (float)(strokeWidth),
                StrokeCap = ToSKStrokeCap(style.StrokeCap),
                StrokeJoin = ToSKStrokeJoin(style.StrokeJoin),
                StrokeMiter = (float)(strokeMiter),
                Color = ToSKColor(style.Stroke),
                //Shader = SKShader.CreateColor(ToSKColor(style.Stroke)),
                PathEffect = null,
                Style = SKPaintStyle.Stroke
            };
        }

        internal static SKPaint ToSKPaintBrush(ArgbColor color, bool isAntialias)
        {
            return new SKPaint()
            {
                IsAntialias = isAntialias,
                IsStroke = false,
                Color = ToSKColor(color),
                //Shader = SKShader.CreateColor(ToSKColor(color)),
                TextAlign = SKTextAlign.Left,
                Style = SKPaintStyle.Fill
            };
        }

        internal static SKPaint ToSKPaintBrush(TextStyle style)
        {
            return new SKPaint()
            {
                IsAntialias = style.IsAntialias,
                IsStroke = false,
                LcdRenderText = style.LcdRenderText,
                SubpixelText = style.SubpixelText,
                Color = ToSKColor(style.Stroke),
                //Shader = SKShader.CreateColor(ToSKColor(style.Stroke)),
                TextAlign = SKTextAlign.Left,
                Style = SKPaintStyle.Fill
            };
        }

        internal static void ToSKPaintPenUpdate(SKPaint paint, ShapeStyle style, double scale)
        {
            double strokeWidth = style.StrokeWidth;
            //double strokeMiter = style.StrokeMiter;
            if (style.IsScaled)
            {
                strokeWidth /= scale;
                //strokeMiter /= scale;
            }
            paint.StrokeWidth = (float)(strokeWidth);
            //paint.StrokeCap = ToSKStrokeCap(style.StrokeCap),
            //paint.StrokeJoin = ToSKStrokeJoin(style.StrokeJoin),
            //paint.StrokeMiter = (float)(strokeMiter),
            //paint.Color = ToSKColor(style.Stroke);
            //paint.Shader = SKShader.CreateColor(ToSKColor(style.Stroke));
        }

        internal static void ToSKPaintBrushUpdate(SKPaint paint, ArgbColor color)
        {
            //paint.Color = ToSKColor(color);
            //paint.Shader = SKShader.CreateColor(ToSKColor(color));
        }

        internal static SKPoint ToSKPoint(IPointShape point, double dx, double dy)
        {
            return new SKPoint((float)(point.X + dx), (float)(point.Y + dy));
        }

        internal static SKRect ToSKRect(double left, double top, double right, double bottom)
        {
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKRect ToSKRect(IPointShape p1, IPointShape p2, double dx, double dy)
        {
            double x1 = p1.X + dx;
            double y1 = p1.Y + dy;
            double x2 = p2.X + dx;
            double y2 = p2.Y + dy;
            double left = Math.Min(x1, x2);
            double top = Math.Min(y1, y2);
            double right = left + Math.Abs(Math.Max(x1, x2) - left);
            double bottom = top + Math.Abs(Math.Max(y1, y2) - top);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKMatrix Multiply(SKMatrix value1, SKMatrix value2)
        {
            return ToSKMatrix(
                (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY);
        }

        internal static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
        {
            return new SKMatrix
            {
                ScaleX = (float)m11,
                SkewX = (float)m21,
                TransX = (float)m31,
                SkewY = (float)m12,
                ScaleY = (float)m22,
                TransY = (float)m32,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        internal static SKPathFillType ToSKPathFillType(PathFillType fillType)
        {
            switch (fillType)
            {
                default:
                case PathFillType.Winding:
                    return SKPathFillType.Winding;
                case PathFillType.EvenOdd:
                    return SKPathFillType.EvenOdd;
                case PathFillType.InverseWinding:
                    return SKPathFillType.InverseWinding;
                case PathFillType.InverseEvenOdd:
                    return SKPathFillType.InverseEvenOdd;
            }
        }

        internal static PathFillType ToPathFillType(SKPathFillType fillType)
        {
            switch (fillType)
            {
                default:
                case SKPathFillType.Winding:
                    return PathFillType.Winding;
                case SKPathFillType.EvenOdd:
                    return PathFillType.EvenOdd;
                case SKPathFillType.InverseWinding:
                    return PathFillType.InverseWinding;
                case SKPathFillType.InverseEvenOdd:
                    return PathFillType.InverseEvenOdd;
            }
        }

        internal static SKPathOp ToSKPathOp(PathOp op)
        {
            switch (op)
            {
                default:
                case PathOp.Difference:
                    return SKPathOp.Difference;
                case PathOp.Intersect:
                    return SKPathOp.Intersect;
                case PathOp.Union:
                    return SKPathOp.Union;
                case PathOp.Xor:
                    return SKPathOp.Xor;
                case PathOp.ReverseDifference:
                    return SKPathOp.ReverseDifference;
            }
        }

        internal static void AddLine(IToolContext context, LineShape line, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(line.StartPoint, dx, dy));
            geometry.LineTo(ToSKPoint(line.Point, dx, dy));
        }

        internal static void AddCubic(IToolContext context, CubicBezierShape cubicBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(cubicBezier.StartPoint, dx, dy));
            geometry.CubicTo(
                ToSKPoint(cubicBezier.Point1, dx, dy),
                ToSKPoint(cubicBezier.Point2, dx, dy),
                ToSKPoint(cubicBezier.Point3, dx, dy));
        }

        internal static void AddQuad(IToolContext context, QuadraticBezierShape quadraticBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(quadraticBezier.StartPoint, dx, dy));
            geometry.QuadTo(
                ToSKPoint(quadraticBezier.Point1, dx, dy),
                ToSKPoint(quadraticBezier.Point2, dx, dy));
        }

        internal static void AddConic(IToolContext context, ConicShape conic, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(conic.StartPoint, dx, dy));
            geometry.ConicTo(
                ToSKPoint(conic.Point1, dx, dy),
                ToSKPoint(conic.Point2, dx, dy),
                (float)conic.Weight);
        }

        internal static void AddRect(IToolContext context, RectangleShape rectangle, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
            if (rectangle.RadiusX > 0.0 && rectangle.RadiusY > 0.0)
            {
                geometry.AddRoundRect(rect, (float)rectangle.RadiusX, (float)rectangle.RadiusY, SKPathDirection.Clockwise);
            }
            else
            {
                geometry.AddRect(rect, SKPathDirection.Clockwise);
            }
        }

        internal static void AddCircle(IToolContext context, CircleShape circle, double dx, double dy, SKPath geometry)
        {
            var distance = circle.StartPoint.ToPoint2().DistanceTo(circle.Point.ToPoint2());
            geometry.AddCircle((float)circle.StartPoint.X, (float)circle.StartPoint.Y, (float)distance, SKPathDirection.Clockwise);
        }

        internal static void AddOval(IToolContext context, EllipseShape ellipse, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(ellipse.StartPoint, ellipse.Point, dx, dy);
            geometry.AddOval(rect, SKPathDirection.Clockwise);
        }

        internal static void AddText(IToolContext context, Text text, IPointShape startPoint, IPointShape point, TextStyle style, double dx, double dy, SKPath geometry)
        {
            using (var typeface = ToSKTypeface(style.Typeface))
            using (var paint = ToSKPaintBrush(style))
            {
                paint.Typeface = typeface;
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)style.FontSize;

                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        paint.TextAlign = SKTextAlign.Left;
                        break;
                    case HAlign.Center:
                        paint.TextAlign = SKTextAlign.Center;
                        break;
                    case HAlign.Right:
                        paint.TextAlign = SKTextAlign.Right;
                        break;
                }

                var metrics = paint.FontMetrics;
                var mAscent = metrics.Ascent;
                var mDescent = metrics.Descent;
                var rect = ToSKRect(startPoint, point, dx, dy);
                float x = rect.Left;
                float y = rect.Top;
                float width = rect.Width;
                float height = rect.Height;

                switch (style.VAlign)
                {
                    default:
                    case VAlign.Top:
                        y -= mAscent;
                        break;
                    case VAlign.Center:
                        y += (height / 2.0f) - (mAscent / 2.0f) - mDescent / 2.0f;
                        break;
                    case VAlign.Bottom:
                        y += height - mDescent;
                        break;
                }

                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        // x = x;
                        break;
                    case HAlign.Center:
                        x += width / 2.0f;
                        break;
                    case HAlign.Right:
                        x += width;
                        break;
                }

                using (var outlineGeometry = paint.GetTextPath(text.Value, x, y))
                using (var fillGeometry = paint.GetFillPath(outlineGeometry))
                {
                    geometry.AddPath(fillGeometry, SKPathAddMode.Append);
                }
            }
        }

        internal static void AddText(IToolContext context, TextShape text, double dx, double dy, SKPath geometry)
        {
            var style = context?.StyleLibrary?.Get(text.StyleId);
            if (style != null)
            {
                AddText(context, text.Text, text.StartPoint, text.Point, style.TextStyle, dx, dy, geometry);
            }
        }

        internal static void AddImage(IToolContext context, ImageShape rectangle, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
            geometry.AddRect(rect, SKPathDirection.Clockwise);
        }

        internal static void AddPath(IToolContext context, PathShape path, double dx, double dy, SKPath geometry)
        {
            geometry.FillType = ToSKPathFillType(path.FillType);

            foreach (var shape in path.Shapes)
            {
                if (shape is FigureShape figure)
                {
                    AddFigure(context, figure, dx, dy, geometry);
                }
            }
        }

        internal static void AddFigure(IToolContext context, IList<IBaseShape> shapes, bool isClosed, double dx, double dy, SKPath geometry)
        {
            bool isFirstShape = true;

            foreach (var shape in shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(line.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.LineTo(ToSKPoint(line.Point, dx, dy));
                        }
                        break;
                    case CubicBezierShape cubicBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(cubicBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.CubicTo(
                                ToSKPoint(cubicBezier.Point1, dx, dy),
                                ToSKPoint(cubicBezier.Point2, dx, dy),
                                ToSKPoint(cubicBezier.Point3, dx, dy));
                        }
                        break;
                    case QuadraticBezierShape quadraticBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(quadraticBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.QuadTo(
                                ToSKPoint(quadraticBezier.Point1, dx, dy),
                                ToSKPoint(quadraticBezier.Point2, dx, dy));
                        }
                        break;
                    case ConicShape conic:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(conic.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.ConicTo(
                                ToSKPoint(conic.Point1, dx, dy),
                                ToSKPoint(conic.Point2, dx, dy),
                                (float)conic.Weight);
                        }
                        break;
                }
            }

            if (!isFirstShape && isClosed)
            {
                geometry.Close();
            }
        }

        internal static void AddFigure(IToolContext context, FigureShape figure, double dx, double dy, SKPath geometry)
        {
            AddFigure(context, figure.Shapes, figure.IsClosed, dx, dy, geometry);
        }

        internal static void AddFigure(IToolContext context, GroupShape group, double dx, double dy, SKPath geometry)
        {
            AddFigure(context, group.Shapes, false, dx, dy, geometry);
        }

        internal static void AddFigure(IToolContext context, ReferenceShape reference, double dx, double dy, SKPath geometry)
        {
            AddShape(context, reference.Template, dx + reference.X, dy + reference.Y, geometry);
        }

        internal static bool AddShape(IToolContext context, IBaseShape shape, double dx, double dy, SKPath geometry)
        {
            switch (shape)
            {
                case LineShape line:
                    AddLine(context, line, dx, dy, geometry);
                    return true;
                case CubicBezierShape cubicBezier:
                    AddCubic(context, cubicBezier, dx, dy, geometry);
                    return true;
                case QuadraticBezierShape quadraticBezier:
                    AddQuad(context, quadraticBezier, dx, dy, geometry);
                    return true;
                case ConicShape conic:
                    AddConic(context, conic, dx, dy, geometry);
                    return true;
                case FigureShape figure:
                    AddFigure(context, figure, dx, dy, geometry);
                    return true;
                case PathShape path:
                    AddPath(context, path, dx, dy, geometry);
                    return true;
                case RectangleShape rectangle:
                    AddRect(context, rectangle, dx, dy, geometry);
                    return true;
                case CircleShape circle:
                    AddCircle(context, circle, dx, dy, geometry);
                    return true;
                case EllipseShape ellipse:
                    AddOval(context, ellipse, dx, dy, geometry);
                    return true;
                case IPointShape point:
                    AddShape(context, point, dx, dy, geometry);
                    return true;
                case TextShape text:
                    AddText(context, text, dx, dy, geometry);
                    return true;
                case ImageShape image:
                    AddImage(context, image, dx, dy, geometry);
                    return true;
                case GroupShape group:
                    AddFigure(context, group, dx, dy, geometry);
                    return true;
                case ReferenceShape reference:
                    AddFigure(context, reference, dx, dy, geometry);
                    return true;
            };
            return false;
        }

        internal static SKPath ToStrokePath(IToolContext context, ShapeStyle style, SKPath geometry)
        {
            using (var paint = ToSKPaintPen(style, 1.0))
            {
                return paint.GetFillPath(geometry, 1.0f);
            }
        }

        internal static SKPath ToFillPath(IToolContext context, ShapeStyle style, SKPath geometry)
        {
            using (var paint = ToSKPaintBrush(style.Fill, style.IsAntialias))
            {
                return paint.GetFillPath(geometry, 1.0f);
            }
        }

        internal static SKPath ToPath(string svgPathData)
        {
            return SKPath.ParseSvgPathData(svgPathData);
        }

        internal static SKPath Op(SKPathOp op, IList<SKPath> paths)
        {
            if (paths == null || paths.Count <= 0)
            {
                return null;
            }

            if (paths.Count == 1)
            {
                using (var empty = new SKPath() { FillType = paths[0].FillType })
                {
                    return empty.Op(paths[0], op);
                }
            }
            else
            {
                var haveResult = false;
                var result = new SKPath(paths[0]) { FillType = paths[0].FillType };

                for (int i = 1; i < paths.Count; i++)
                {
                    var next = result.Op(paths[i], op);
                    if (next != null)
                    {
                        result.Dispose();
                        result = next;
                        haveResult = true;
                    }
                }

                return haveResult ? result : null;
            }
        }

        internal static PathShape ToPathShape(IToolContext context, SKPath path, ShapeStyle style, IBaseShape pointTemplate)
        {
            var pathShape = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillType = ToPathFillType(path.FillType),
                Text = new Text(),
                StyleId = style.Title
            };

            var figureShape = default(FigureShape);

            using (var iterator = path.CreateRawIterator())
            {
                var points = new SKPoint[4];
                var pathVerb = SKPathVerb.Move;
                var firstPoint = new SKPoint();
                var lastPoint = new SKPoint();

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            {
                                figureShape = new FigureShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    Shapes = new ObservableCollection<IBaseShape>(),
                                    IsFilled = true,
                                    IsClosed = false
                                };
                                figureShape.Owner = pathShape;
                                pathShape.Shapes.Add(figureShape);
                                firstPoint = lastPoint = points[0];
                            }
                            break;
                        case SKPathVerb.Line:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var lineShape = new LineShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                lineShape.Owner = figureShape;
                                lineShape.StartPoint.Owner = lineShape;
                                lineShape.Point.Owner = lineShape;
                                figureShape.Shapes.Add(lineShape);
                                lastPoint = points[1];
                            }
                            break;
                        case SKPathVerb.Cubic:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var cubicBezierShape = new CubicBezierShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Point3 = new PointShape(points[3].X, points[3].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                cubicBezierShape.Owner = figureShape;
                                cubicBezierShape.StartPoint.Owner = cubicBezierShape;
                                cubicBezierShape.Point1.Owner = cubicBezierShape;
                                cubicBezierShape.Point2.Owner = cubicBezierShape;
                                cubicBezierShape.Point3.Owner = cubicBezierShape;
                                figureShape.Shapes.Add(cubicBezierShape);
                                lastPoint = points[3];
                            }
                            break;
                        case SKPathVerb.Quad:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var quadraticBezierShape = new QuadraticBezierShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                quadraticBezierShape.Owner = figureShape;
                                quadraticBezierShape.StartPoint.Owner = quadraticBezierShape;
                                quadraticBezierShape.Point1.Owner = quadraticBezierShape;
                                quadraticBezierShape.Point2.Owner = quadraticBezierShape;
                                figureShape.Shapes.Add(quadraticBezierShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Conic:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var conicShape = new ConicShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Weight = iterator.ConicWeight(),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                conicShape.Owner = figureShape;
                                conicShape.StartPoint.Owner = conicShape;
                                conicShape.Point1.Owner = conicShape;
                                conicShape.Point2.Owner = conicShape;
                                figureShape.Shapes.Add(conicShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Close:
                            {
#if USE_CLOSE_SHAPE
                                var line = new LineShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = pathShape.GetLastPoint(),
                                    Point = pathShape.GetFirstPoint(),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                line.Owner = figureShape;
                                line.StartPoint.Owner = line;
                                line.Point.Owner = line;
                                figureShape.Shapes.Add(line);
#else
                                figureShape.IsClosed = true;
                                firstPoint = lastPoint = new SKPoint(0, 0);
#endif
                            }
                            break;
                    }
                }
            }

            return pathShape;
        }

        internal static void ToSvgPathData(IToolContext context, Text text, IPointShape startPoint, IPointShape point, TextStyle style, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(text?.Value))
            {
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    AddText(context, text, startPoint, point, style, 0.0, 0.0, geometry);
                    sb.AppendLine(geometry.ToSvgPathData());
                }
            }
        }

        internal static void ToSvgPathData(IToolContext context, IBaseShape shape, StringBuilder sb)
        {
            switch (shape)
            {
                case LineShape line:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddLine(context, line, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case CubicBezierShape cubicBezier:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddCubic(context, cubicBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case QuadraticBezierShape quadraticBezier:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddQuad(context, quadraticBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case ConicShape conic:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddConic(context, conic, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case PathShape pathShape:
                    {
                        using (var geometry = new SKPath() { FillType = SkiaHelper.ToSKPathFillType(pathShape.FillType) })
                        {
                            AddPath(context, pathShape, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddRect(context, rectangle, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.StyleLibrary?.Get(rectangle.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, rectangle.Text, rectangle.StartPoint, rectangle.Point, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
                case CircleShape circle:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddCircle(context, circle, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.StyleLibrary?.Get(circle.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, circle.Text, circle.StartPoint, circle.Point, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                        {
                            AddOval(context, ellipse, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.StyleLibrary?.Get(ellipse.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, ellipse.Text, ellipse.StartPoint, ellipse.Point, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
#if USE_SVG_POINT
                case IPointShape point:
                    {
                        if (point.Template != null)
                        {
                            ToSvgPathData(context, point.Template, sb);
                        }
                    }
                    break;
#endif
                case GroupShape group:
                    {
                        foreach (var groupShape in group.Shapes)
                        {
                            ToSvgPathData(context, groupShape, sb);
                        }
                    }
                    break;
                case TextShape text:
                    {
                        var style = context?.StyleLibrary?.Get(text.StyleId);
                        if (style != null)
                        {
                            ToSvgPathData(context, text.Text, text.StartPoint, text.Point, style.TextStyle, sb);
                        }
                    }
                    break;
            };
        }

        internal static SKPicture SvgToSKPicture(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                using (var stream = new StringReader(xml))
                using (var reader = XmlReader.Create(stream))
                {
                    var svg = new SkiaSharp.Extended.Svg.SKSvg();
                    return svg.Load(reader);
                }
            }
            return null;
        }

        internal static SKPicture ToSKPicture(string path)
        {
            var extension = Path.GetExtension(path);

            if (string.Compare(extension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var xml = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(xml))
                {
                    return SvgToSKPicture(xml);
                }
                return null;
            }

            using (var recorder = new SKPictureRecorder())
            {
                var image = SKImage.FromEncodedData(path);
                var rect = new SKRect(0f, 0f, (float)image.Width, (float)image.Height);
                var canvas = recorder.BeginRecording(rect);
                canvas.DrawImage(image, rect);
                return recorder.EndRecording();
            }
        }
    }
}
