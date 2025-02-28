﻿using PicView.PicGallery;
using PicView.Shortcuts;
using PicView.UILogic;
using PicView.UILogic.Sizing;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using static PicView.SystemIntegration.NativeMethods;

namespace PicView.Views.Windows
{
    public partial class FakeWindow : Window
    {
        internal bool ActuallyVisible { get; set; }
        public FakeWindow()
        {
            ShowActivated = false;
            InitializeComponent();
            Width = WindowSizing.MonitorInfo.Width;
            Height = WindowSizing.MonitorInfo.Height;
            Width = WindowSizing.MonitorInfo.Width;
            Height = WindowSizing.MonitorInfo.Height;
            ContentRendered += FakeWindow_ContentRendered;
        }

        private void FakeWindow_ContentRendered(object sender, EventArgs e)
        {
            EnableBlur(this);

            // Hide from alt tab
            var helper = new WindowInteropHelper(this);
            _ = SetWindowLong(helper.Handle, GWL_EX_STYLE, (GetWindowLong(helper.Handle, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
            helper.EnsureHandle();

            var timer = new System.Timers.Timer(20)
            {
                AutoReset = false,
                Enabled = true
            };
            timer.Elapsed += delegate
            {
                ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Render, () =>
                {
                    ConfigureWindows.GetMainWindow.Topmost = true;
                    ConfigureWindows.GetMainWindow.Topmost = Properties.Settings.Default.TopMost;
                    ConfigureWindows.GetMainWindow.BringIntoView();
                    Keyboard.Focus(ConfigureWindows.GetMainWindow);
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(ConfigureWindows.GetMainWindow), null);
                });
            };
            timer.Start();

            PreviewMouseLeftButtonDown += FakeWindow_MouseLeftButtonDown;
            PreviewMouseRightButtonDown += FakeWindow_MouseLeftButtonDown;
            PreviewMouseWheel += async (sender, e) => await MainMouseKeys.MainImage_MouseWheelAsync(sender, e).ConfigureAwait(false);
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
            StateChanged += FakeWindow_StateChanged;
            ConfigureWindows.GetMainWindow.Activated += GetMainWindow_Activated;
        }

        private void GetMainWindow_Activated(object? sender, EventArgs e)
        {
            if (ConfigureWindows.GetMainWindow.WindowState == WindowState.Normal && GalleryFunctions.IsVerticalFullscreenOpen
                || ConfigureWindows.GetMainWindow.WindowState == WindowState.Normal && GalleryFunctions.IsHorizontalFullscreenOpen)
            {
                if (ActuallyVisible)
                {
                    return;
                }

                Show();
                Activate();
                ActuallyVisible = true;
                ConfigureWindows.GetMainWindow.Topmost = true;
                ConfigureWindows.GetMainWindow.Topmost = Properties.Settings.Default.TopMost;
                ConfigureWindows.GetMainWindow.BringIntoView();
                Keyboard.Focus(ConfigureWindows.GetMainWindow);
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(ConfigureWindows.GetMainWindow), null);
            }
        }

        private void FakeWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                ConfigureWindows.GetMainWindow.Focus();
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (ConfigureWindows.GetMainWindow.WindowState == WindowState.Normal && GalleryFunctions.IsVerticalFullscreenOpen
                || ConfigureWindows.GetMainWindow.WindowState == WindowState.Normal && GalleryFunctions.IsHorizontalFullscreenOpen)
            {
                if (IsVisible is false)
                {
                    Show();
                    Activate();
                }
            }
            else if (ConfigureWindows.GetMainWindow.WindowState == WindowState.Minimized)
            {
                Hide();
                ActuallyVisible = false;
            }
        }

        private void FakeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConfigureWindows.GetMainWindow.Focus();
        }
    }
}