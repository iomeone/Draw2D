using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Svg;
using Svg.Css;
using Svg.DataTypes;
using Svg.ExCSS;
using Svg.ExCSS.Model;
using Svg.ExCSS.Model.Extensions;
using Svg.Exceptions;
using Svg.ExtensionMethods;
using Svg.Document_Structure;
using Svg.FilterEffects;
using Svg.Pathing;
using Svg.Transforms;

namespace SvgDemo
{
    /// The <see cref="SvgElement"/> object graph.
    /// +---abstract class <see cref="SvgElement"/>
    /// |   +---class <see cref="SvgClipPath"/>
    /// |   +---class <see cref="SvgFragment"/>
    /// |       \---class <see cref="SvgDocument"/>
    /// |   +---class <see cref="SvgMask"/>
    /// |   +---class <see cref="SvgDefinitionList"/>
    /// |   +---class <see cref="SvgDescription"/>
    /// |   +---class <see cref="SvgDocumentMetadata"/>
    /// |   +---class <see cref="SvgTitle"/>
    /// |   +---class <see cref="SvgMergeNode"/>
    /// |   +---class <see cref="SvgFilter"/>
    /// |   +---class <see cref="NonSvgElement"/>
    /// |   +---class <see cref="SvgGradientStop"/>
    /// |   +---class <see cref="SvgUnknownElement"/>
    /// |   +---class <see cref="SvgFont"/>
    /// |   +---class <see cref="SvgFontFace"/>
    /// |   +---class <see cref="SvgFontFaceSrc"/>
    /// |   +---class <see cref="SvgFontFaceUri"/>
    /// |   +---abstract class <see cref="SvgVisualElement"/>
    /// |   |   +---class <see cref="SvgImage"/>
    /// |   |   +---class <see cref="SvgSwitch"/>
    /// |   |   +---class <see cref="SvgSymbol"/>
    /// |   |   +---class <see cref="SvgUse"/>
    /// |   |   +---class <see cref="SvgForeignObject"/>
    /// |   |   +---abstract class <see cref="SvgPathBasedElement"/>
    /// |   |       +---<see cref="SvgCircle"/>
    /// |   |       +---<see cref="SvgEllipse"/>
    /// |   |       +---<see cref="SvgRectangle"/>
    /// |   |       +---<see cref="SvgMarker"/>
    /// |   |       +---<see cref="SvgGlyph"/>
    /// |   |       +---abstract class <see cref="SvgMarkerElement"/>
    /// |   |           +---class <see cref="SvgGroup"/>
    /// |   |           +---class <see cref="SvgLine"/>
    /// |   |           +---class <see cref="SvgPath"/>
    /// |   |           \---class <see cref="SvgPolygon"/>
    /// |   |               \---class <see cref="SvgPolyline"/>
    /// |   \-------abstract class <see cref="SvgTextBase"/>
    /// |           +----class <see cref="SvgText"/>
    /// |           +----class <see cref="SvgTextPath"/>
    /// |           +----class <see cref="SvgTextRef"/>
    /// |           \----class <see cref="SvgTextSpan"/>
    /// +---abstract class <see cref="SvgFilterPrimitive"/>
    /// |   +---class <see cref="SvgColourMatrix"/>
    /// |   +---class <see cref="SvgGaussianBlur"/>
    /// |   +---class <see cref="SvgMerge"/>
    /// |   \---class <see cref="SvgOffset"/>
    /// +---abstract class <see cref="SvgPaintServer"/>
    /// |   +---class <see cref="SvgColourServer"/>
    /// |   +---class <see cref="SvgDeferredPaintServer"/>
    /// |   +---class <see cref="SvgFallbackPaintServer"/>
    /// |   \---class <see cref="SvgPatternServer"/>
    /// |       \---abstract class <see cref="SvgGradientServer"/>
    /// |           +---class <see cref="SvgLinearGradientServer"/>
    /// |           \---class <see cref="SvgRadialGradientServer"/>
    /// \---abstract class <see cref="SvgKern"/>
    ///     +---class <see cref="SvgVerticalKern"/>
    ///     \---class <see cref="SvgHorizontalKern"/>

    /// The <see cref="SvgPathSegment"/> object graph.
    /// +---abstract class <see cref="SvgPathSegment"/>
    ///     +---class <see cref="SvgArcSegment"/>
    ///     +---class <see cref="SvgClosePathSegment"/>
    ///     +---class <see cref="SvgCubicCurveSegment"/>
    ///     +---class <see cref="SvgLineSegment"/>
    ///     +---class <see cref="SvgMoveToSegment"/>
    ///     \---class <see cref="SvgQuadraticCurveSegment"/>

    public class SvgDebug
    {
        public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor ElementColor { get; set; } = ConsoleColor.Red;
        public ConsoleColor HeaderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor AttributeColor { get; set; } = ConsoleColor.Blue;
        public string IndentTab { get; set; } = "  ";
        public bool PrintSvgElementAttributesEnabled { get; set; } = true;
        public bool PrintSvgElementCustomAttributesEnabled { get; set; } = true;
        public bool PrintSvgElementChildrenEnabled { get; set; } = true;
        public bool PrintSvgElementNodesEnabled { get; set; } = true;

        private string GetElementName(SvgElement svgElement)
        {
            var attr = TypeDescriptor.GetAttributes(svgElement).OfType<SvgElementAttribute>().SingleOrDefault();
            if (attr != null)
            {
                return attr.ElementName;
            }
            return "unknown";
        }

        private string Format(float value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void WriteLine(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }

        public void PrintAttributes(SvgClipPath svgClipPath, string indentLine, string indentAttribute)
        {
            if (svgClipPath.ClipPathUnits != SvgCoordinateUnits.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}clipPathUnits: {svgClipPath.ClipPathUnits}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgFragment svgFragment, string indentLine, string indentAttribute)
        {
            if (svgFragment.X != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}x: {Format(svgFragment.X)}", AttributeColor);
            }

            if (svgFragment.Y != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}y: {Format(svgFragment.Y)}", AttributeColor);
            }

            if (svgFragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}width: {Format(svgFragment.Width)}", AttributeColor);
            }

            if (svgFragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}height: {Format(svgFragment.Height)}", AttributeColor);
            }

            if (svgFragment.Overflow != SvgOverflow.Inherit && svgFragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}overflow: {svgFragment.Overflow}", AttributeColor);
            }

            if (svgFragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgFragment.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}viewBox: {Format(viewBox.MinX)} {Format(viewBox.MinY)} {Format(viewBox.Width)} {Format(viewBox.Height)}", AttributeColor);
            }

            if (svgFragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgFragment.AspectRatio.Align != @default.Align
                 || svgFragment.AspectRatio.Slice != @default.Slice
                 || svgFragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}preserveAspectRatio: {svgFragment.AspectRatio}", AttributeColor);
                }
            }

            if (svgFragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}font-size: {svgFragment.FontSize}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgFragment.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}font-family: {svgFragment.FontFamily}", AttributeColor);
            }

            if (svgFragment.SpaceHandling != XmlSpaceHandling.@default && svgFragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}space: {svgFragment.SpaceHandling}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgMask svgMask, string indentLine, string indentAttribute)
        {
        }

        public void PrintAttributes(SvgDefinitionList svgDefinitionList, string indentLine, string indentAttribute)
        {
        }

        public void PrintAttributes(SvgDescription svgDescription, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgDescription.Content))
            {
                if (svgDescription.Children.Count == 0)
                {
                    WriteLine($"{indentLine}{indentAttribute}Content: |", AttributeColor);
                    WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgDescription.Content}", AttributeColor);
                }
            }
        }

        public void PrintAttributes(SvgDocumentMetadata svgDocumentMetadata, string indentLine, string indentAttribute)
        {
        }

        public void PrintAttributes(SvgTitle svgTitle, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgTitle.Content))
            {
                if (svgTitle.Children.Count == 0)
                {
                    WriteLine($"{indentLine}{indentAttribute}Content: |", AttributeColor);
                    WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgTitle.Content}", AttributeColor);
                }
            }
        }

        public void PrintAttributes(SvgMergeNode svgMergeNode, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgMergeNode.Input))
            {
                WriteLine($"{indentLine}{indentAttribute}in: {svgMergeNode.Input}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgFilter svgFilter, string indentLine, string indentAttribute)
        {
            WriteLine($"{indentLine}{indentAttribute}x: {Format(svgFilter.X)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y: {Format(svgFilter.Y)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}width: {Format(svgFilter.Width)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}height: {Format(svgFilter.Height)}", AttributeColor);

            if (svgFilter.ColorInterpolationFilters != SvgColourInterpolation.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}color-interpolation-filters: {svgFilter.ColorInterpolationFilters}", AttributeColor);
            }
        }

        public void PrintAttributes(NonSvgElement nonSvgElement, string indentLine, string indentAttribute)
        {
            WriteLine($"{indentLine}{indentAttribute}Name: {nonSvgElement.Name}", AttributeColor);
        }

        public void PrintAttributes(SvgGradientStop svgGradientStop, string indentLine, string indentAttribute)
        {
            if (svgGradientStop.Offset != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}offset: {svgGradientStop.Offset}", AttributeColor);
            }

            if (svgGradientStop.StopColor != null && svgGradientStop.StopColor != SvgColourServer.NotSet)
            {
                PrintSvgPaintServerServer(svgGradientStop.StopColor, "stop-color", indentLine, indentAttribute);
            }

            if (svgGradientStop.Opacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}stop-opacity: {Format(svgGradientStop.Opacity)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgUnknownElement svgUnknownElement, string indentLine, string indentAttribute)
        {
        }

        public void PrintAttributes(SvgFont svgFont, string indentLine, string indentAttribute)
        {
            if (svgFont.HorizAdvX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}horiz-adv-x: {Format(svgFont.HorizAdvX)}", AttributeColor);
            }

            if (svgFont.HorizOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}horiz-origin-x: {Format(svgFont.HorizOriginX)}", AttributeColor);
            }

            if (svgFont.HorizOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}horiz-origin-y: {Format(svgFont.HorizOriginY)}", AttributeColor);
            }

            if (svgFont.VertAdvY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-adv-y: {Format(svgFont.VertAdvY)}", AttributeColor);
            }

            if (svgFont.VertOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-origin-x: {Format(svgFont.VertOriginX)}", AttributeColor);
            }

            if (svgFont.VertOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-origin-y: {Format(svgFont.VertOriginY)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgFontFace svgFontFace, string indentLine, string indentAttribute)
        {
            if (svgFontFace.Alphabetic != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}alphabetic: {Format(svgFontFace.Alphabetic)}", AttributeColor);
            }

            if (svgFontFace.Ascent != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}ascent: {Format(svgFontFace.Ascent)}", AttributeColor);
            }

            if (svgFontFace.AscentHeight != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}ascent-height: {Format(svgFontFace.AscentHeight)}", AttributeColor);
            }

            if (svgFontFace.Descent != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}descent: {Format(svgFontFace.Descent)}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgFontFace.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}font-family: {svgFontFace.FontFamily}", AttributeColor);
            }

            if (svgFontFace.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}font-size: {svgFontFace.FontSize}", AttributeColor);
            }

            if (svgFontFace.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indentLine}{indentAttribute}font-style: {svgFontFace.FontStyle}", AttributeColor);
            }

            if (svgFontFace.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}font-variant: {svgFontFace.FontVariant}", AttributeColor);
            }

            if (svgFontFace.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}font-weight: {svgFontFace.FontWeight}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgFontFace.Panose1))
            {
                WriteLine($"{indentLine}{indentAttribute}panose-1: {svgFontFace.Panose1}", AttributeColor);
            }

            if (svgFontFace.UnitsPerEm != 1000f)
            {
                WriteLine($"{indentLine}{indentAttribute}units-per-em: {Format(svgFontFace.UnitsPerEm)}", AttributeColor);
            }

            if (svgFontFace.XHeight != float.MinValue)
            {
                WriteLine($"{indentLine}{indentAttribute}x-height: {Format(svgFontFace.XHeight)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgFontFaceSrc svgFontFaceSrc, string indentLine, string indentAttribute)
        {
        }

        public void PrintAttributes(SvgFontFaceUri svgFontFaceUri, string indentLine, string indentAttribute)
        {
            if (svgFontFaceUri.ReferencedElement != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgFontFaceUri.ReferencedElement}", AttributeColor);
            }
        }

        public void PrintSvgVisualElementAttributes(SvgVisualElement svgVisualElement, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgVisualElement.Clip))
            {
                WriteLine($"{indentLine}{indentAttribute}clip: {svgVisualElement.Clip}", AttributeColor);
            }

            if (svgVisualElement.ClipPath != null)
            {
                WriteLine($"{indentLine}{indentAttribute}clip-path: {svgVisualElement.ClipPath}", AttributeColor);
            }

            if (svgVisualElement.ClipRule != SvgClipRule.NonZero)
            {
                WriteLine($"{indentLine}{indentAttribute}clip-rule: {svgVisualElement.ClipRule}", AttributeColor);
            }

            if (svgVisualElement.Filter != null)
            {
                WriteLine($"{indentLine}{indentAttribute}filter: {svgVisualElement.Filter}", AttributeColor);
            }

            // Style

            if (svgVisualElement.Visible != true)
            {
                WriteLine($"{indentLine}{indentAttribute}visibility: {svgVisualElement.Visible}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgVisualElement.Display))
            {
                WriteLine($"{indentLine}{indentAttribute}display: {svgVisualElement.Display}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgVisualElement.EnableBackground))
            {
                WriteLine($"{indentLine}{indentAttribute}enable-background: {svgVisualElement.EnableBackground}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgImage svgImage, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgImage, indentLine, indentAttribute);

            if (svgImage.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgImage.AspectRatio.Align != @default.Align
                 || svgImage.AspectRatio.Slice != @default.Slice
                 || svgImage.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}preserveAspectRatio: {svgImage.AspectRatio}", AttributeColor);
                }
            }

            WriteLine($"{indentLine}{indentAttribute}x: {Format(svgImage.X)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y: {Format(svgImage.Y)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}width: {Format(svgImage.Width)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}height: {Format(svgImage.Height)}", AttributeColor);

            if (svgImage.Href != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgImage.Href}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgSwitch svgSwitch, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgSwitch, indentLine, indentAttribute);
        }

        public void PrintAttributes(SvgSymbol svgSymbol, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgSymbol, indentLine, indentAttribute);

            if (svgSymbol.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgSymbol.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}viewBox: {viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", AttributeColor);
            }

            if (svgSymbol.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgSymbol.AspectRatio.Align != @default.Align
                 || svgSymbol.AspectRatio.Slice != @default.Slice
                 || svgSymbol.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}preserveAspectRatio: {svgSymbol.AspectRatio}", AttributeColor);
                }
            }
        }

        public void PrintAttributes(SvgUse svgUse, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgUse, indentLine, indentAttribute);

            if (svgUse.ReferencedElement != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgUse.ReferencedElement}", AttributeColor);
            }

            WriteLine($"{indentLine}{indentAttribute}x: {Format(svgUse.X)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y: {Format(svgUse.Y)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}width: {Format(svgUse.Width)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}height: {Format(svgUse.Height)}", AttributeColor);
        }

        public void PrintAttributes(SvgForeignObject svgForeignObject, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgForeignObject, indentLine, indentAttribute);
        }

        public void PrintSvgPathBasedElementAttributes(SvgPathBasedElement svgPathBasedElement, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgPathBasedElement, indentLine, indentAttribute);
        }

        public void PrintAttributes(SvgCircle svgCircle, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgCircle, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}cx: {Format(svgCircle.CenterX)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}cy: {Format(svgCircle.CenterY)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}r: {Format(svgCircle.Radius)}", AttributeColor);
        }

        public void PrintAttributes(SvgEllipse svgEllipse, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgEllipse, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}cx: {Format(svgEllipse.CenterX)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}cy: {Format(svgEllipse.CenterY)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}rx: {Format(svgEllipse.RadiusX)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}ry: {Format(svgEllipse.RadiusY)}", AttributeColor);
        }

        public void PrintAttributes(SvgRectangle svgRectangle, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgRectangle, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}x: {Format(svgRectangle.X)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y: {Format(svgRectangle.Y)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}width: {Format(svgRectangle.Width)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}height: {Format(svgRectangle.Height)}", AttributeColor);

            if (svgRectangle.CornerRadiusX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}rx: {Format(svgRectangle.CornerRadiusX)}", AttributeColor);
            }

            if (svgRectangle.CornerRadiusY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}ry: {Format(svgRectangle.CornerRadiusY)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgMarker svgMarker, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgMarker, indentLine, indentAttribute);

            if (svgMarker.RefX != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}refX: {svgMarker.RefX}", AttributeColor);
            }

            if (svgMarker.RefY != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}refY: {svgMarker.RefY}", AttributeColor);
            }

            if (svgMarker.Orient != null)
            {
                var orient = svgMarker.Orient;
                if (orient.IsAuto == false)
                {
                    if (orient.Angle != 0f)
                    {
                        WriteLine($"{indentLine}{indentAttribute}orient: {Format(orient.Angle)}", AttributeColor);
                    }
                }
                else
                {
                    WriteLine($"{indentLine}{indentAttribute}orient: {(orient.IsAutoStartReverse ? "auto-start-reverse" : "auto")}", AttributeColor);
                }
            }

            if (svgMarker.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}overflow: {svgMarker.Overflow}", AttributeColor);
            }

            if (svgMarker.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgMarker.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}viewBox: {viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", AttributeColor);
            }

            if (svgMarker.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgMarker.AspectRatio.Align != @default.Align
                 || svgMarker.AspectRatio.Slice != @default.Slice
                 || svgMarker.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}preserveAspectRatio: {svgMarker.AspectRatio}", AttributeColor);
                }
            }

            if (svgMarker.MarkerWidth != 3f)
            {
                WriteLine($"{indentLine}{indentAttribute}markerWidth: {svgMarker.MarkerWidth}", AttributeColor);
            }

            if (svgMarker.MarkerHeight != 3f)
            {
                WriteLine($"{indentLine}{indentAttribute}markerHeight: {svgMarker.MarkerHeight}", AttributeColor);
            }

            if (svgMarker.MarkerUnits != SvgMarkerUnits.StrokeWidth)
            {
                WriteLine($"{indentLine}{indentAttribute}markerUnits: {svgMarker.MarkerUnits}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgGlyph svgGlyph, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgGlyph, indentLine, indentAttribute);

            if (svgGlyph.PathData != null)
            {
                PrintAttributes(svgGlyph.PathData, indentLine, indentAttribute);
            }

            if (!string.IsNullOrEmpty(svgGlyph.GlyphName))
            {
                WriteLine($"{indentLine}{indentAttribute}glyph-name: {svgGlyph.GlyphName}", AttributeColor);
            }

            if (svgGlyph.HorizAdvX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}horiz-adv-x: {Format(svgGlyph.HorizAdvX)}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgGlyph.Unicode))
            {
                WriteLine($"{indentLine}{indentAttribute}unicode: {svgGlyph.Unicode}", AttributeColor);
            }

            if (svgGlyph.VertAdvY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-adv-y: {Format(svgGlyph.VertAdvY)}", AttributeColor);
            }

            if (svgGlyph.VertOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-origin-x: {Format(svgGlyph.VertOriginX)}", AttributeColor);
            }

            if (svgGlyph.VertOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}vert-origin-y: {Format(svgGlyph.VertOriginY)}", AttributeColor);
            }
        }

        public void PrintSvgMarkerElementAttributes(SvgMarkerElement svgMarkerElement, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgMarkerElement, indentLine, indentAttribute);

            if (svgMarkerElement.MarkerEnd != null)
            {
                WriteLine($"{indentLine}{indentAttribute}marker-end: {svgMarkerElement.MarkerEnd}", AttributeColor);
            }

            if (svgMarkerElement.MarkerMid != null)
            {
                WriteLine($"{indentLine}{indentAttribute}marker-mid: {svgMarkerElement.MarkerMid}", AttributeColor);
            }

            if (svgMarkerElement.MarkerStart != null)
            {
                WriteLine($"{indentLine}{indentAttribute}marker-start: {svgMarkerElement.MarkerStart}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgGroup svgGroup, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgGroup, indentLine, indentAttribute);
        }

        public void PrintAttributes(SvgLine svgLine, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgLine, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}x1: {svgLine.StartX}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y1: {svgLine.StartY}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}x2: {svgLine.EndX}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y2: {svgLine.EndY}", AttributeColor);
        }

        public void PrintAttributes(SvgPathSegmentList svgPathSegmentList, string indentLine, string indentAttribute)
        {
            if (svgPathSegmentList != null)
            {
                WriteLine($"{indentLine}{indentAttribute}d: |", AttributeColor);

                foreach (var svgSegment in svgPathSegmentList)
                {
                    switch (svgSegment)
                    {
                        case SvgArcSegment svgArcSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgArcSegment}", AttributeColor);
                            break;
                        case SvgClosePathSegment svgClosePathSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgClosePathSegment}", AttributeColor);
                            break;
                        case SvgCubicCurveSegment svgCubicCurveSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgCubicCurveSegment}", AttributeColor);
                            break;
                        case SvgLineSegment svgLineSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgLineSegment}", AttributeColor);
                            break;
                        case SvgMoveToSegment svgMoveToSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgMoveToSegment}", AttributeColor);
                            break;
                        case SvgQuadraticCurveSegment svgQuadraticCurveSegment:
                            WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgQuadraticCurveSegment}", AttributeColor);
                            break;
                        default:
                            WriteLine($"ERROR: Unknown path segment type: {svgSegment.GetType()}", ErrorColor);
                            break;
                    }
                }
            }
        }

        public void PrintAttributes(SvgPath svgPath, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPath, indentLine, indentAttribute);

            if (svgPath.PathData != null)
            {
                PrintAttributes(svgPath.PathData, indentLine, indentAttribute);
            }

            if (svgPath.PathLength != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}pathLength: {Format(svgPath.PathLength)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgPolygon svgPolygon, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPolygon, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}points: {svgPolygon.Points}", AttributeColor);
        }

        public void PrintAttributes(SvgPolyline svgPolyline, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPolyline, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}points: {svgPolyline.Points}", AttributeColor);
        }

        public void PrintSvgTextBaseAttributes(SvgTextBase svgTextBase, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgTextBase, indentLine, indentAttribute);

            if (!string.IsNullOrEmpty(svgTextBase.Text))
            {
                if (svgTextBase.Children.Count == 0)
                {
                    WriteLine($"{indentLine}{indentAttribute}Content: |", AttributeColor);
                    WriteLine($"{indentLine}{indentAttribute}{IndentTab}{svgTextBase.Text}", AttributeColor);
                }
            }

            if (svgTextBase.X != null && svgTextBase.X.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}x: {svgTextBase.X}", AttributeColor);
            }

            if (svgTextBase.Dx != null && svgTextBase.Dx.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}dx: {svgTextBase.Dx}", AttributeColor);
            }

            if (svgTextBase.Y != null && svgTextBase.Y.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}y: {svgTextBase.Y}", AttributeColor);
            }

            if (svgTextBase.Dy != null && svgTextBase.Dy.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}dy: {svgTextBase.Dy}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgTextBase.Rotate))
            {
                WriteLine($"{indentLine}{indentAttribute}rotate: {svgTextBase.Rotate}", AttributeColor);
            }

            if (svgTextBase.TextLength != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}textLength: {svgTextBase.TextLength}", AttributeColor);
            }

            if (svgTextBase.LengthAdjust != SvgTextLengthAdjust.Spacing)
            {
                WriteLine($"{indentLine}{indentAttribute}lengthAdjust: {svgTextBase.LengthAdjust}", AttributeColor);
            }

            if (svgTextBase.LetterSpacing != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}letter-spacing: {svgTextBase.LetterSpacing}", AttributeColor);
            }

            if (svgTextBase.WordSpacing != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}word-spacing: {svgTextBase.WordSpacing}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgText svgText, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgText, indentLine, indentAttribute);
        }

        public void PrintAttributes(SvgTextPath svgTextPath, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextPath, indentLine, indentAttribute);

            if (svgTextPath.StartOffset != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}startOffset: {svgTextPath.StartOffset}", AttributeColor);
            }

            if (svgTextPath.Method != SvgTextPathMethod.Align)
            {
                WriteLine($"{indentLine}{indentAttribute}method: {svgTextPath.Method}", AttributeColor);
            }

            if (svgTextPath.Spacing != SvgTextPathSpacing.Exact)
            {
                WriteLine($"{indentLine}{indentAttribute}spacing: {svgTextPath.Spacing}", AttributeColor);
            }

            if (svgTextPath.ReferencedPath != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgTextPath.ReferencedPath}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgTextRef svgTextRef, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextRef, indentLine, indentAttribute);

            if (svgTextRef.ReferencedElement != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgTextRef.ReferencedElement}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgTextSpan svgTextSpan, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextSpan, indentLine, indentAttribute);
        }

        public void PrintSvgFilterPrimitiveAttributes(SvgFilterPrimitive svgFilterPrimitive, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgFilterPrimitive.Input))
            {
                WriteLine($"{indentLine}{indentAttribute}in: {svgFilterPrimitive.Input}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(svgFilterPrimitive.Result))
            {
                WriteLine($"{indentLine}{indentAttribute}result: {svgFilterPrimitive.Result}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgColourMatrix svgColourMatrix, string indentLine, string indentAttribute)
        {
            PrintSvgFilterPrimitiveAttributes(svgColourMatrix, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}type: {svgColourMatrix.Type}", AttributeColor);

            if (!string.IsNullOrEmpty(svgColourMatrix.Values))
            {
                WriteLine($"{indentLine}{indentAttribute}values: {svgColourMatrix.Values}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgGaussianBlur svgGaussianBlur, string indentLine, string indentAttribute)
        {
            PrintSvgFilterPrimitiveAttributes(svgGaussianBlur, indentLine, indentAttribute);

            if (svgGaussianBlur.StdDeviation > 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}stdDeviation: {Format(svgGaussianBlur.StdDeviation)}", AttributeColor);
            }
        }

        public void PrintAttributes(SvgMerge svgMerge, string indentLine, string indentAttribute)
        {
            PrintSvgFilterPrimitiveAttributes(svgMerge, indentLine, indentAttribute);
        }

        public void PrintAttributes(SvgOffset svgOffset, string indentLine, string indentAttribute)
        {
            PrintSvgFilterPrimitiveAttributes(svgOffset, indentLine, indentAttribute);

            if (svgOffset.Dx != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}dx: {svgOffset.Dx}", AttributeColor);
            }

            if (svgOffset.Dy != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}dy: {svgOffset.Dy}", AttributeColor);
            }
        }

        public void PrintSvgPaintServerServer(SvgPaintServer svgPaintServer, string attribute, string indentLine, string indentAttribute)
        {
            switch (svgPaintServer)
            {
                case SvgColourServer svgColourServer:
                    PrintAttributes(svgColourServer, indentLine, indentAttribute, attribute);
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    PrintAttributes(svgDeferredPaintServer, indentLine, indentAttribute, attribute);
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    PrintAttributes(svgFallbackPaintServer, indentLine, indentAttribute, attribute);
                    break;
                case SvgPatternServer svgPatternServer:
                    PrintAttributes(svgPatternServer, indentLine, indentAttribute);
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    PrintAttributes(svgLinearGradientServer, indentLine, indentAttribute);
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    PrintAttributes(svgRadialGradientServer, indentLine, indentAttribute);
                    break;
                default:
                    WriteLine($"ERROR: Unknown paint server type: {svgPaintServer.GetType()}", ErrorColor);
                    break;
            }
        }

        public void PrintAttributes(SvgColourServer svgColourServer, string indentLine, string indentAttribute, string attribute)
        {
            WriteLine($"{indentLine}{indentAttribute}{attribute}: {svgColourServer.ToString()}", AttributeColor);
        }

        public void PrintAttributes(SvgDeferredPaintServer svgDeferredPaintServer, string indentLine, string indentAttribute, string attribute)
        {
            WriteLine($"{indentLine}{indentAttribute}{attribute}: {svgDeferredPaintServer.GetType()}", AttributeColor);
        }

        public void PrintAttributes(SvgFallbackPaintServer svgFallbackPaintServer, string indentLine, string indentAttribute, string attribute)
        {
            WriteLine($"{indentLine}{indentAttribute}{attribute}: {svgFallbackPaintServer.GetType()}", AttributeColor);
        }

        public void PrintAttributes(SvgPatternServer svgPatternServer, string indentLine, string indentAttribute)
        {
            if (svgPatternServer.Overflow != SvgOverflow.Inherit && svgPatternServer.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}overflow: {svgPatternServer.Overflow}", AttributeColor);
            }

            if (svgPatternServer.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgPatternServer.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}viewBox: {Format(viewBox.MinX)} {Format(viewBox.MinY)} {Format(viewBox.Width)} {Format(viewBox.Height)}", AttributeColor);
            }

            if (svgPatternServer.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgPatternServer.AspectRatio.Align != @default.Align
                 || svgPatternServer.AspectRatio.Slice != @default.Slice
                 || svgPatternServer.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}preserveAspectRatio: {svgPatternServer.AspectRatio}", AttributeColor);
                }
            }

            WriteLine($"{indentLine}{indentAttribute}width: {Format(svgPatternServer.Width)}", AttributeColor);

            if (svgPatternServer.PatternUnits != SvgCoordinateUnits.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}patternUnits: {svgPatternServer.PatternUnits}", AttributeColor);
            }

            if (svgPatternServer.PatternContentUnits != SvgCoordinateUnits.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}patternContentUnits: {svgPatternServer.PatternContentUnits}", AttributeColor);
            }

            WriteLine($"{indentLine}{indentAttribute}height: {Format(svgPatternServer.Height)}", AttributeColor);

            WriteLine($"{indentLine}{indentAttribute}x: {Format(svgPatternServer.X)}", AttributeColor);
            WriteLine($"{indentLine}{indentAttribute}y: {Format(svgPatternServer.Y)}", AttributeColor);

            if (svgPatternServer.InheritGradient != null)
            {
                WriteLine($"{indentLine}{indentAttribute}href: {svgPatternServer.InheritGradient}", AttributeColor);
            }

            PrintSvgTransformCollection(svgPatternServer.PatternTransform, indentLine, indentAttribute, "patternTransform");
        }

        public void PrintSvgGradientServerAttributes(SvgGradientServer svgGradientServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public void PrintAttributes(SvgLinearGradientServer svgLinearGradientServer, string indentLine, string indentAttribute)
        {
            PrintSvgGradientServerAttributes(svgLinearGradientServer, indentLine, indentAttribute);

            // TODO:
        }

        public void PrintAttributes(SvgRadialGradientServer svgRadialGradientServer, string indentLine, string indentAttribute)
        {
            PrintSvgGradientServerAttributes(svgRadialGradientServer, indentLine, indentAttribute);

            // TODO:
        }

        public void PrintSvgKernAttributes(SvgKern svgKern, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public void PrintAttributes(SvgVerticalKern svgVerticalKern, string indentLine, string indentAttribute)
        {
            PrintSvgKernAttributes(svgVerticalKern, indentLine, indentAttribute);

            // TODO:
        }

        public void PrintAttributes(SvgHorizontalKern svgHorizontalKern, string indentLine, string indentAttribute)
        {
            PrintSvgKernAttributes(svgHorizontalKern, indentLine, indentAttribute);

            // TODO:
        }

        public void PrintSvgTransformAttributes(SvgTransform svgTransform, string indentLine, string indentAttribute)
        {
            switch (svgTransform)
            {
                case SvgMatrix svgMatrix:
                    {
                        WriteLine($"{indentLine}matrix:", ElementColor);
                        WriteLine($"{indentLine}{indentAttribute}type: {svgMatrix.GetType().Name}", AttributeColor);
                        var points = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "{0}, {1}, {2}, {3}, {4}, {5}",
                                        svgMatrix.Points[0],
                                        svgMatrix.Points[1],
                                        svgMatrix.Points[2],
                                        svgMatrix.Points[3],
                                        svgMatrix.Points[4],
                                        svgMatrix.Points[5]);
                        WriteLine($"{indentLine}{indentAttribute}points: {points}", AttributeColor);
                    }
                    break;
                case SvgRotate svgRotate:
                    {
                        WriteLine($"{indentLine}rotate:", ElementColor);
                        WriteLine($"{indentLine}{indentAttribute}type: {svgRotate.GetType().Name}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}Angle: {Format(svgRotate.Angle)}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}CenterX: {Format(svgRotate.CenterX)}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}CenterY: {Format(svgRotate.CenterY)}", AttributeColor);
                    }
                    break;
                case SvgScale svgScale:
                    {
                        WriteLine($"{indentLine}scale:", ElementColor);
                        WriteLine($"{indentLine}{indentAttribute}type: {svgScale.GetType().Name}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}X: {Format(svgScale.X)}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}Y: {Format(svgScale.Y)}", AttributeColor);
                    }
                    break;
                case SvgShear svgShear:
                    {
                        WriteLine($"{indentLine}shear:", ElementColor);
                        WriteLine($"{indentLine}{indentAttribute}type: {svgShear.GetType().Name}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}X: {Format(svgShear.X)}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}Y: {Format(svgShear.Y)}", AttributeColor);
                    }
                    break;
                case SvgSkew svgSkew:
                    {
                        if (svgSkew.AngleY == 0)
                        {
                            WriteLine($"{indentLine}skewX:", ElementColor);
                            WriteLine($"{indentLine}{indentAttribute}type: {svgSkew.GetType().Name}", AttributeColor);
                            WriteLine($"{indentLine}{indentAttribute}AngleX: {Format(svgSkew.AngleX)}", AttributeColor);
                        }
                        else
                        {
                            WriteLine($"{indentLine}skewY:", ElementColor);
                            WriteLine($"{indentLine}{indentAttribute}type: {svgSkew.GetType().Name}", AttributeColor);
                            WriteLine($"{indentLine}{indentAttribute}AngleY: {Format(svgSkew.AngleY)}", AttributeColor);
                        }
                    }
                    break;
                case SvgTranslate svgTranslate:
                    {
                        WriteLine($"{indentLine}translate:", ElementColor);
                        WriteLine($"{indentLine}{indentAttribute}type: {svgTranslate.GetType().Name}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}X: {Format(svgTranslate.X)}", AttributeColor);
                        WriteLine($"{indentLine}{indentAttribute}Y: {Format(svgTranslate.Y)}", AttributeColor);
                    }
                    break;
                default:
                    WriteLine($"ERROR: Unknown transform type: {svgTransform.GetType()}", ErrorColor);
                    break;
            }
        }

        public void PrintSvgTransformCollection(SvgTransformCollection svgTransformCollection, string indentLine, string indentAttribute, string attribute)
        {
            if (svgTransformCollection != null && svgTransformCollection.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}{attribute}:", AttributeColor);

                foreach (var svgTransform in svgTransformCollection)
                {
                    PrintSvgTransformAttributes(svgTransform, indentLine + indentAttribute + IndentTab, indentAttribute);
                }
            }
        }

        public void PrintSvgElement(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            string elementIndent = indentLine;

            WriteLine($"{elementIndent}{GetElementName(svgElement)}:", ElementColor);
            WriteLine($"{indentLine}{indentAttribute}type: {svgElement.GetType().Name}", AttributeColor);

            indentLine += IndentTab;

            if (PrintSvgElementAttributesEnabled)
            {
                WriteLine($"{indentLine}Attributes:", HeaderColor);

                // Transforms Attributes

                PrintSvgTransformCollection(svgElement.Transforms, indentLine, indentAttribute, "transforms");

                // Attributes

                if (!string.IsNullOrEmpty(svgElement.ID))
                {
                    WriteLine($"{indentLine}{indentAttribute}id: {svgElement.ID}", AttributeColor);
                }

                if (svgElement.SpaceHandling != XmlSpaceHandling.@default && svgElement.SpaceHandling != XmlSpaceHandling.inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}space: {svgElement.SpaceHandling}", AttributeColor);
                }

                if (svgElement.Color != null && svgElement.Color != SvgColourServer.NotSet)
                {
                    PrintSvgPaintServerServer(svgElement.Color, "color", indentLine, indentAttribute);
                }

                // Style Attributes

                if (svgElement.Fill != null && svgElement.Fill != SvgColourServer.NotSet)
                {
                    PrintSvgPaintServerServer(svgElement.Fill, "fill", indentLine, indentAttribute);
                }

                if (svgElement.Stroke != null)
                {
                    PrintSvgPaintServerServer(svgElement.Stroke, "stroke", indentLine, indentAttribute);
                }

                if (svgElement.FillRule != SvgFillRule.NonZero)
                {
                    WriteLine($"{indentLine}{indentAttribute}fill-rule: {svgElement.FillRule}", AttributeColor);
                }

                if (svgElement.FillOpacity != 1f)
                {
                    WriteLine($"{indentLine}{indentAttribute}fill-opacity: {Format(svgElement.FillOpacity)}", AttributeColor);
                }

                if (svgElement.StrokeWidth != 1f)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-width: {Format(svgElement.StrokeWidth)}", AttributeColor);
                }

                if (svgElement.StrokeLineCap != SvgStrokeLineCap.Butt)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-linecap: {svgElement.StrokeLineCap}", AttributeColor);
                }

                if (svgElement.StrokeLineJoin != SvgStrokeLineJoin.Miter)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-linejoin: {svgElement.StrokeLineJoin}", AttributeColor);
                }

                if (svgElement.StrokeMiterLimit != 4f)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-miterlimit: {Format(svgElement.StrokeMiterLimit)}", AttributeColor);
                }

                if (svgElement.StrokeDashArray != null && svgElement.StrokeDashArray.Count > 0)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-dasharray: {svgElement.StrokeDashArray}", AttributeColor);
                }

                if (svgElement.StrokeDashOffset != SvgUnit.Empty)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-dashoffset: {svgElement.StrokeDashOffset}", AttributeColor);
                }

                if (svgElement.StrokeOpacity != 1f)
                {
                    WriteLine($"{indentLine}{indentAttribute}stroke-opacity: {Format(svgElement.StrokeOpacity)}", AttributeColor);
                }

                if (svgElement.StopColor != null)
                {
                    PrintSvgPaintServerServer(svgElement.StopColor, "stop-color", indentLine, indentAttribute);
                }

                if (svgElement.Opacity != 1f)
                {
                    WriteLine($"{indentLine}{indentAttribute}opacity: {Format(svgElement.Opacity)}", AttributeColor);
                }

                if (svgElement.ShapeRendering != SvgShapeRendering.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}shape-rendering: {svgElement.ShapeRendering}", AttributeColor);
                }

                if (svgElement.TextAnchor != SvgTextAnchor.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}text-anchor: {svgElement.TextAnchor}", AttributeColor);
                }

                if (!string.IsNullOrEmpty(svgElement.BaselineShift))
                {
                    WriteLine($"{indentLine}{indentAttribute}baseline-shift: {svgElement.BaselineShift}", AttributeColor);
                }

                if (!string.IsNullOrEmpty(svgElement.FontFamily))
                {
                    WriteLine($"{indentLine}{indentAttribute}font-family: {svgElement.FontFamily}", AttributeColor);
                }

                if (svgElement.FontSize != SvgUnit.Empty)
                {
                    WriteLine($"{indentLine}{indentAttribute}font-size: {svgElement.FontSize}", AttributeColor);
                }

                if (svgElement.FontStyle != SvgFontStyle.All)
                {
                    WriteLine($"{indentLine}{indentAttribute}font-style: {svgElement.FontStyle}", AttributeColor);
                }

                if (svgElement.FontVariant != SvgFontVariant.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}font-variant: {svgElement.FontVariant}", AttributeColor);
                }

                if (svgElement.TextDecoration != SvgTextDecoration.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}text-decoration: {svgElement.TextDecoration}", AttributeColor);
                }

                if (svgElement.FontWeight != SvgFontWeight.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}font-weight: {svgElement.FontWeight}", AttributeColor);
                }

                if (svgElement.TextTransformation != SvgTextTransformation.Inherit)
                {
                    WriteLine($"{indentLine}{indentAttribute}text-transform: {svgElement.TextTransformation}", AttributeColor);
                }

                if (!string.IsNullOrEmpty(svgElement.Font))
                {
                    WriteLine($"{indentLine}{indentAttribute}font: {svgElement.Font}", AttributeColor);
                }
            }

            switch (svgElement)
            {
                case SvgClipPath svgClipPath:
                    PrintAttributes(svgClipPath, indentLine, indentAttribute);
                    break;
                case SvgDocument svgDocument:
                    PrintAttributes(svgDocument, indentLine, indentAttribute);
                    break;
                case SvgFragment svgFragment:
                    PrintAttributes(svgFragment, indentLine, indentAttribute);
                    break;
                case SvgMask svgMask:
                    PrintAttributes(svgMask, indentLine, indentAttribute);
                    break;
                case SvgDefinitionList svgDefinitionList:
                    PrintAttributes(svgDefinitionList, indentLine, indentAttribute);
                    break;
                case SvgDescription svgDescription:
                    PrintAttributes(svgDescription, indentLine, indentAttribute);
                    break;
                case SvgDocumentMetadata svgDocumentMetadata:
                    PrintAttributes(svgDocumentMetadata, indentLine, indentAttribute);
                    break;
                case SvgTitle svgTitle:
                    PrintAttributes(svgTitle, indentLine, indentAttribute);
                    break;
                case SvgMergeNode svgMergeNode:
                    PrintAttributes(svgMergeNode, indentLine, indentAttribute);
                    break;
                case SvgFilter svgFilter:
                    PrintAttributes(svgFilter, indentLine, indentAttribute);
                    break;
                case NonSvgElement nonSvgElement:
                    PrintAttributes(nonSvgElement, indentLine, indentAttribute);
                    break;
                case SvgGradientStop svgGradientStop:
                    PrintAttributes(svgGradientStop, indentLine, indentAttribute);
                    break;
                case SvgUnknownElement svgUnknownElement:
                    PrintAttributes(svgUnknownElement, indentLine, indentAttribute);
                    break;
                case SvgFont svgFont:
                    PrintAttributes(svgFont, indentLine, indentAttribute);
                    break;
                case SvgFontFace svgFontFace:
                    PrintAttributes(svgFontFace, indentLine, indentAttribute);
                    break;
                case SvgFontFaceSrc svgFontFaceSrc:
                    PrintAttributes(svgFontFaceSrc, indentLine, indentAttribute);
                    break;
                case SvgFontFaceUri svgFontFaceUri:
                    PrintAttributes(svgFontFaceUri, indentLine, indentAttribute);
                    break;
                case SvgImage svgImage:
                    PrintAttributes(svgImage, indentLine, indentAttribute);
                    break;
                case SvgSwitch svgSwitch:
                    PrintAttributes(svgSwitch, indentLine, indentAttribute);
                    break;
                case SvgSymbol svgSymbol:
                    PrintAttributes(svgSymbol, indentLine, indentAttribute);
                    break;
                case SvgUse svgUse:
                    PrintAttributes(svgUse, indentLine, indentAttribute);
                    break;
                case SvgForeignObject svgForeignObject:
                    PrintAttributes(svgForeignObject, indentLine, indentAttribute);
                    break;
                case SvgCircle svgCircle:
                    PrintAttributes(svgCircle, indentLine, indentAttribute);
                    break;
                case SvgEllipse svgEllipse:
                    PrintAttributes(svgEllipse, indentLine, indentAttribute);
                    break;
                case SvgRectangle svgRectangle:
                    PrintAttributes(svgRectangle, indentLine, indentAttribute);
                    break;
                case SvgMarker svgMarker:
                    PrintAttributes(svgMarker, indentLine, indentAttribute);
                    break;
                case SvgGlyph svgGlyph:
                    PrintAttributes(svgGlyph, indentLine, indentAttribute);
                    break;
                case SvgGroup svgGroup:
                    PrintAttributes(svgGroup, indentLine, indentAttribute);
                    break;
                case SvgLine svgLine:
                    PrintAttributes(svgLine, indentLine, indentAttribute);
                    break;
                case SvgPath svgPath:
                    PrintAttributes(svgPath, indentLine, indentAttribute);
                    break;
                case SvgPolyline svgPolyline:
                    PrintAttributes(svgPolyline, indentLine, indentAttribute);
                    break;
                case SvgPolygon svgPolygon:
                    PrintAttributes(svgPolygon, indentLine, indentAttribute);
                    break;
                case SvgText svgText:
                    PrintAttributes(svgText, indentLine, indentAttribute);
                    break;
                case SvgTextPath svgTextPath:
                    PrintAttributes(svgTextPath, indentLine, indentAttribute);
                    break;
                case SvgTextRef svgTextRef:
                    PrintAttributes(svgTextRef, indentLine, indentAttribute);
                    break;
                case SvgTextSpan svgTextSpan:
                    PrintAttributes(svgTextSpan, indentLine, indentAttribute);
                    break;
                case SvgColourMatrix svgColourMatrix:
                    PrintAttributes(svgColourMatrix, indentLine, indentAttribute);
                    break;
                case SvgGaussianBlur svgGaussianBlur:
                    PrintAttributes(svgGaussianBlur, indentLine, indentAttribute);
                    break;
                case SvgMerge svgMerge:
                    PrintAttributes(svgMerge, indentLine, indentAttribute);
                    break;
                case SvgOffset svgOffset:
                    PrintAttributes(svgOffset, indentLine, indentAttribute);
                    break;
                case SvgColourServer svgColourServer:
                    PrintAttributes(svgColourServer, indentLine, indentAttribute, null);
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    PrintAttributes(svgDeferredPaintServer, indentLine, indentAttribute, null);
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    PrintAttributes(svgFallbackPaintServer, indentLine, indentAttribute, null);
                    break;
                case SvgPatternServer svgPatternServer:
                    PrintAttributes(svgPatternServer, indentLine, indentAttribute);
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    PrintAttributes(svgLinearGradientServer, indentLine, indentAttribute);
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    PrintAttributes(svgRadialGradientServer, indentLine, indentAttribute);
                    break;
                case SvgVerticalKern svgVerticalKern:
                    PrintAttributes(svgVerticalKern, indentLine, indentAttribute);
                    break;
                case SvgHorizontalKern svgHorizontalKern:
                    PrintAttributes(svgHorizontalKern, indentLine, indentAttribute);
                    break;
                default:
                    WriteLine($"ERROR: Unknown elemen type: {svgElement.GetType()}", ErrorColor);
                    break;
            }

            if (PrintSvgElementCustomAttributesEnabled && svgElement.CustomAttributes.Count > 0)
            {
                WriteLine($"{indentLine}CustomAttributes:", HeaderColor);

                foreach (var attribute in svgElement.CustomAttributes)
                {
                    WriteLine($"{indentLine}{indentAttribute}{attribute.Key}: {attribute.Value}", AttributeColor);
                }
            }

            if (PrintSvgElementNodesEnabled && svgElement.Nodes.Count > 0)
            {
                WriteLine($"{indentLine}Nodes: |", HeaderColor);

                foreach (var node in svgElement.Nodes)
                {
                    WriteLine($"{indentLine}{indentAttribute}{IndentTab}{node.Content}", AttributeColor);
                }
            }

            if (PrintSvgElementChildrenEnabled && svgElement.Children.Count > 0)
            {
                WriteLine($"{indentLine}Children:", HeaderColor);

                foreach (var child in svgElement.Children)
                {
                    PrintSvgElement(child, indentLine + IndentTab, indentAttribute);
                }
            }
        }

        public void Run(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string path = args[i];
                WriteLine($"# {path}", HeaderColor);

                var svgDocument = SvgDocument.Open<SvgDocument>(path, null);
                if (svgDocument != null)
                {
                    svgDocument.FlushStyles(true);
                    PrintSvgElement(svgDocument, "", IndentTab);
                    ResetColor();
                }
            }
        }
    }

    class Program
    {
        static void Error(Exception ex)
        {
            Console.WriteLine($"{ex.Message}", ConsoleColor.Yellow);
            Console.WriteLine($"{ex.StackTrace}", ConsoleColor.Black);
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                new SvgDebug()
                {
                    ErrorColor = ConsoleColor.Yellow,
                    ElementColor = ConsoleColor.Red,
                    HeaderColor = ConsoleColor.White,
                    AttributeColor = ConsoleColor.Blue,
                    IndentTab = "  ",
                    PrintSvgElementAttributesEnabled = true,
                    PrintSvgElementCustomAttributesEnabled = true,
                    PrintSvgElementChildrenEnabled = true,
                    PrintSvgElementNodesEnabled = false
                }
                .Run(args);
            }
            catch (Exception ex)
            {
                Error(ex);
                Console.ResetColor();
            }
        }
    }
}
