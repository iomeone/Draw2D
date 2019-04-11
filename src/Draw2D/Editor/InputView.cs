﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    public class InputView : Border
    {
        public static readonly StyledProperty<IVisual> RelativeToProperty =
            AvaloniaProperty.Register<InputView, IVisual>(nameof(RelativeTo));

        public IVisual RelativeTo
        {
            get { return GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        public InputView()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            Modifier modifier = Modifier.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(RelativeTo);
                    ctx.CurrentTool.LeftDown(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(RelativeTo);
                    ctx.CurrentTool.RightDown(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(RelativeTo);
                    if (ctx.Mode == EditMode.Mouse)
                    {
                        ctx.CurrentTool.LeftUp(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                    else if (ctx.Mode == EditMode.Touch)
                    {
                        ctx.CurrentTool.LeftDown(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(RelativeTo);
                    ctx.CurrentTool.RightUp(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (this.DataContext is IToolContext ctx)
            {
                var point = e.GetPosition(RelativeTo);
                ctx.CurrentTool.Move(ctx, point.X, point.Y, GetModifier(e.InputModifiers));
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is IToolContext ctx)
            {
                if (ctx.CurrentContainer.WorkBackground != null)
                {
                    var color = AvaloniaBrushCache.FromDrawColor(ctx.CurrentContainer.InputBackground);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
                }
            }
        }
    }
}
