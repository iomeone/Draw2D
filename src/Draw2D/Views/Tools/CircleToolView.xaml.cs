﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Tools
{
    public class CircleToolView : UserControl
    {
        public CircleToolView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
