﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using System.Collections.Generic;
using PanAndZoom;

namespace Draw2D.ViewModels
{
    public interface IToolContext
    {
        IShapeRenderer Renderer { get; set; }
        IHitTest HitTest { get; set; }
        CanvasContainer CurrentContainer { get; set; }
        CanvasContainer WorkingContainer { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        Action Capture { get; set; }
        Action Release { get; set; }
        Action Invalidate { get; set; }
        IList<ToolBase> Tools { get; set; }
        ToolBase CurrentTool { get; set; }
        EditMode Mode { get; set; }
        ICanvasPresenter Presenter { get; set; }
        ISelection Selection { get; set; }
        IPanAndZoom Zoom { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
        void SetTool(string name);
    }
}
