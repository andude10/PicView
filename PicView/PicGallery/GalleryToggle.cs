﻿using PicView.Animations;
using PicView.UILogic;
using PicView.Views.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using static PicView.ChangeImage.Navigation;
using static PicView.PicGallery.GalleryFunctions;
using static PicView.UILogic.ConfigureWindows;
using static PicView.UILogic.UC;

namespace PicView.PicGallery
{
    internal static class GalleryToggle
    {
        #region Open

        internal static async Task OpenHorizontalGalleryAsync()
        {
            if (Pics?.Count < 1)
            {
                return;
            }

            IsHorizontalOpen = true;
            IsHorizontalFullscreenOpen = IsHorizontalOpen = false;

            await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, () =>
            {
                GalleryLoad.LoadLayout(false);
                GetPicGallery.Visibility = Visibility.Visible;

                bool fade = AnimationHelper.Fade(GetPicGallery, TimeSpan.FromSeconds(.3), TimeSpan.Zero, 0, 1);
                if (fade == false)
                {
                    GetPicGallery.Opacity = 1;
                }

                GetClickArrowLeft.Visibility =
                GetClickArrowRight.Visibility =
                Getx2.Visibility =
                GetMinus.Visibility =
                GetRestorebutton.Visibility =
                GetGalleryShortcut.Visibility = Visibility.Hidden;

                if (GetFakeWindow != null)
                {
                    if (GetFakeWindow.grid.Children.Contains(GetPicGallery))
                    {
                        GetFakeWindow.grid.Children.Remove(GetPicGallery);
                        GetMainWindow.ParentContainer.Children.Add(GetPicGallery);
                    }
                }
            });

            await LoadAndScrollToAsync().ConfigureAwait(false);
        }

        internal static async Task OpenFullscreenGalleryAsync(bool startup)
        {
            if (Pics?.Count < 1 && !startup)
            {
                return;
            }

            await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
            {
                GalleryLoad.LoadLayout(true);
                GetPicGallery.Visibility = Visibility.Visible;

                if (Properties.Settings.Default.FullscreenGalleryHorizontal)
                {
                    IsHorizontalFullscreenOpen = true;
                    IsVerticalFullscreenOpen = IsHorizontalOpen = false;
                }
                else
                {
                    IsVerticalFullscreenOpen = true;
                    IsHorizontalFullscreenOpen = IsHorizontalOpen = false;
                }

                if (GetFakeWindow == null)
                {
                    GetFakeWindow = new FakeWindow()
                    {
                        WindowStyle = WindowStyle.None,
                        ShowInTaskbar = false,
                        ShowActivated = false
                    };

                    if (Properties.Settings.Default.FullscreenGalleryHorizontal)
                    {
                        GetFakeWindow.grid.Children.Add(new Views.UserControls.Gallery.PicGalleryTopButtonsV2
                        {
                            Margin = new Thickness(1, 12, 0, 0),
                        });
                    }
                    else
                    {
                        GetFakeWindow.grid.Children.Add(new Views.UserControls.Gallery.PicGalleryTopButtons
                        {
                            Margin = new Thickness(1, 12, 0, 0),
                        });
                    }
                }
                else
                {
                    GetFakeWindow.grid.Children.RemoveAt(0);
                    if (Properties.Settings.Default.FullscreenGalleryHorizontal)
                    {
                        GetFakeWindow.grid.Children.Add(new Views.UserControls.Gallery.PicGalleryTopButtonsV2
                        {
                            Margin = new Thickness(1, 12, 0, 0),
                        });
                    }
                    else
                    {
                        GetFakeWindow.grid.Children.Add(new Views.UserControls.Gallery.PicGalleryTopButtons
                        {
                            Margin = new Thickness(1, 12, 0, 0),
                        });
                    }
                }

                // Switch gallery container to the correct window
                if (GetMainWindow.ParentContainer.Children.Contains(GetPicGallery))
                {
                    GetMainWindow.ParentContainer.Children.Remove(GetPicGallery);
                    GetFakeWindow.grid.Children.Add(GetPicGallery);
                }
                else if (!GetFakeWindow.grid.Children.Contains(GetPicGallery))
                {
                    GetFakeWindow.grid.Children.Add(GetPicGallery);
                }

                GetFakeWindow.Show();
                GetMainWindow.Focus();

                // Fix not showing up opacity bug..
                VisualStateManager.GoToElementState(GetPicGallery, "Opacity", false);
                VisualStateManager.GoToElementState(GetPicGallery.Container, "Opacity", false);
                GetPicGallery.Opacity = GetPicGallery.Container.Opacity = 1;
            });

            if (startup == false)
            {
                await UILogic.Sizing.ScaleImage.TryFitImageAsync().ConfigureAwait(false);
            }

            await LoadAndScrollToAsync().ConfigureAwait(false);
        }

        #endregion Open

        #region Close

        internal static void CloseCurrentGallery()
        {
            if (GalleryFunctions.IsVerticalFullscreenOpen || GalleryFunctions.IsHorizontalFullscreenOpen)
            {
                CloseFullscreenGallery();
            }
            else if (GalleryFunctions.IsHorizontalOpen)
            {
                CloseHorizontalGallery();
            }
        }

        internal static void CloseHorizontalGallery()
        {
            if (UC.GetPicGallery is null) { return; }

            IsVerticalFullscreenOpen = IsHorizontalFullscreenOpen = IsHorizontalOpen = false;

            // Restore interface elements if needed
            if (!Properties.Settings.Default.ShowInterface || Properties.Settings.Default.Fullscreen)
            {
                HideInterfaceLogic.ShowNavigation(true);
                HideInterfaceLogic.ShowShortcuts(true);
            }

            var da = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.5),
                From = 1,
                To = 0,
                FillBehavior = FillBehavior.Stop
            };
            da.Completed += delegate
            {
                GetPicGallery.Visibility = Visibility.Collapsed;
            };

            GetPicGallery.BeginAnimation(UIElement.OpacityProperty, da);
        }

        internal static void CloseFullscreenGallery()
        {
            if (ConfigureWindows.GetFakeWindow is null) { return; }

            IsVerticalFullscreenOpen = IsHorizontalFullscreenOpen = IsHorizontalOpen = false;
            GetFakeWindow.Hide();

            ConfigureSettings.ConfigColors.UpdateColor();

            HideInterfaceLogic.ShowStandardInterface();
            GetPicGallery.x2.Visibility = Visibility.Collapsed;

            // Restore settings
            UILogic.Sizing.WindowSizing.SetWindowBehavior();

            if (Properties.Settings.Default.AutoFitWindow)
            {
                UILogic.Sizing.WindowSizing.CenterWindowOnScreen();
            }
            else
            {
                UILogic.Sizing.WindowSizing.SetLastWindowSize();
            }

            if (GetMainWindow.MainImage.Source is not null)
            {
                UILogic.Sizing.ScaleImage.FitImage(GetMainWindow.MainImage.Source.Width, GetMainWindow.MainImage.Source.Height);
            }
        }

        #endregion Close

        private static async Task LoadAndScrollToAsync()
        {
            if (GalleryLoad.IsLoading == false)
            {
                bool checkLoad = false;
                await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
                {
                    if (GetPicGallery.Container.Children.Count == ChangeImage.Navigation.Pics.Count)
                    {
                        checkLoad = true;
                    }
                });
                if (checkLoad == false)
                {
                    await GalleryLoad.Load().ConfigureAwait(false);
                }
            }

            try
            {
                await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, () =>
                {
                    GalleryNavigation.ScrollTo();
                });
            }
            catch (TaskCanceledException)
            {
                // Suppress TaskCanceledException
            }
        }
    }
}