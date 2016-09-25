﻿using System;
using System.IO;
using System.Windows;
using Draw2D.Models.Containers;
using Draw2D.ViewModels.Containers;
using Microsoft.Win32;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Draw2D.Wpf.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                New(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                var vm = this.DataContext as ShapesContainerViewModel;
                if (vm != null)
                {
                    Open(path, vm);
                    RendererView.InvalidateVisual();
                }
            }
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                var vm = this.DataContext as ShapesContainerViewModel;
                if (vm != null)
                {
                    Save(path, vm);
                }
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[0].Close();
        }

        private void New(ShapesContainerViewModel vm)
        {
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            var container = new ShapesContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new ShapesContainer();
            vm.Container = container;
            vm.WorkingContainer = new ShapesContainer();
        }

        private void Open(string path, ShapesContainerViewModel vm)
        {
            var yaml = File.ReadAllText(path);
            using (var reader = new StringReader(yaml))
            {
                var builder = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithNamingConvention(new NullNamingConvention());
                var deserializer = builder.Build();
                var container = deserializer.Deserialize<ShapesContainer>(reader);
                var workingContainer = new ShapesContainer();
                vm.CurrentTool.Clean(vm);
                vm.Renderer.Selected.Clear();
                vm.Container = container;
                vm.WorkingContainer = workingContainer;
            }
        }

        private void Save(string path, ShapesContainerViewModel vm)
        {
            using (var writer = new StringWriter())
            {
                var builder = new SerializerBuilder()
                    .EmitDefaults()
                    .EnsureRoundtrip()
                    .WithNamingConvention(new NullNamingConvention());
                var serializer = builder.Build();
                serializer.Serialize(writer, vm.Container);
                File.WriteAllText(path, writer.ToString());
            }
        }
    }
}
