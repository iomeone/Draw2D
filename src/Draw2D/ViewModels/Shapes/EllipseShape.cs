﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Spatial;

namespace Draw2D.ViewModels.Shapes
{
    public static class EllipseShapeExtensions
    {
        public static Rect2 ToRect2(this EllipseShape ellipse, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                ellipse.TopLeft.X, ellipse.TopLeft.Y,
                ellipse.BottomRight.X, ellipse.BottomRight.Y,
                dx, dy);
        }

        public static EllipseShape FromRect2(this Rect2 rect)
        {
            return new EllipseShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2());
        }
    }

    public class EllipseShape : BoxShape, ICopyable
    {
        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            if (this.IsDirty || result == true)
            {
                renderer.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawEllipse(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.Selection.Selected.Contains(TopLeft))
                {
                    TopLeft.Draw(dc, renderer, dx, dy, mode, db, r);
                }
    
                if (renderer.Selection.Selected.Contains(BottomRight))
                {
                    BottomRight.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
            base.EndTransform(dc, renderer, state);
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new EllipseShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}
