﻿using PicView.UILogic.Sizing;
using System.Windows;
using System.Windows.Input;

namespace PicView.Views.Windows
{
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public ResizeWindow()
        {
            Title = Application.Current.Resources["BatchResize"] + " - PicView";
            MaxHeight = WindowSizing.MonitorInfo.WorkArea.Height;
            Width *= WindowSizing.MonitorInfo.DpiScaling;
            if (double.IsNaN(Width)) // Fixes if user opens window when loading from startup
            {
                WindowSizing.MonitorInfo = SystemIntegration.MonitorSize.GetMonitorSize();
                MaxHeight = WindowSizing.MonitorInfo.WorkArea.Height;
                Width *= WindowSizing.MonitorInfo.DpiScaling;
            }

            InitializeComponent();

            ContentRendered += (sender, e) =>
            {
                MouseLeftButtonDown += (_, e) =>
                { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } };

                KeyDown += (_, e) => Shortcuts.GenericWindowShortcuts.KeysDown(null, e, this);

                // CloseButton
                CloseButton.TheButton.Click += delegate { Hide(); };

                // MinButton
                MinButton.TheButton.Click += delegate { SystemCommands.MinimizeWindow(this); };

                TitleBar.MouseLeftButtonDown += delegate { DragMove(); };
            };
        }
    }
}
