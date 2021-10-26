﻿using PicView.FileHandling;
using PicView.ImageHandling;
using PicView.Views.UserControls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PicView.UILogic
{
    internal static class ImageInfo
    {
        static object? rating;

        internal static async Task RenameTask(KeyEventArgs e, TextBox textBox, string file)
        {
            if (e.Key != System.Windows.Input.Key.Enter) { return; }

            e.Handled = true;
            var rename = await FileFunctions.RenameFileWithErrorChecking(file).ConfigureAwait(false);
            if (rename.HasValue == false)
            {
                Tooltip.ShowTooltipMessage(Application.Current.Resources["AnErrorOccuredMovingFile"]);
                return;
            }
            if (rename.Value)
            {
                await ConfigureWindows.GetImageInfoWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                {
                    textBox.CaretIndex = textBox.Text.Length;
                });
            }
        }

        internal static async Task UpdateValuesAsync(FileInfo? fileInfo)
        {
            if (ConfigureWindows.GetImageInfoWindow == null || ConfigureWindows.GetImageInfoWindow != null && ConfigureWindows.GetImageInfoWindow.IsVisible == false)
            {
                return;
            }

            var data = await GetImageData.RetrieveData(fileInfo).ConfigureAwait(false);

            await ConfigureWindows.GetImageInfoWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                if (data != null)
                {
                    ConfigureWindows.GetImageInfoWindow.FilenameBox.Text = data[0];

                    ConfigureWindows.GetImageInfoWindow.FolderBox.Text = data[1];

                    ConfigureWindows.GetImageInfoWindow.FullPathBox.Text = data[2];

                    ConfigureWindows.GetImageInfoWindow.CreatedBox.Text = data[3];

                    ConfigureWindows.GetImageInfoWindow.ModifiedBox.Text = data[4];

                    ConfigureWindows.GetImageInfoWindow.AccessedBox.Text = data[5];

                    ConfigureWindows.GetImageInfoWindow.BitDepthBox.Text = data[6];

                    ConfigureWindows.GetImageInfoWindow.WidthBox.Text = data[7];

                    ConfigureWindows.GetImageInfoWindow.HeightBox.Text = data[8];

                    ConfigureWindows.GetImageInfoWindow.ResolutionBox.Text = data[9];

                    ConfigureWindows.GetImageInfoWindow.SizeMpBox.Text = data[10];

                    ConfigureWindows.GetImageInfoWindow.PrintSizeCmBox.Text = data[11];

                    ConfigureWindows.GetImageInfoWindow.PrintSizeInBox.Text = data[12];

                    ConfigureWindows.GetImageInfoWindow.AspectRatioBox.Text = data[13];

                    rating = data[14];

                    if (data.Length > 15)
                    {
                        if (ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Count > 0)
                        {
                            // 0 == GPS
                            var latitudeBox = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[1];
                            latitudeBox.SetValues(data[15], data[16], true);
                            var longitudeBox = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[2];
                            longitudeBox.SetValues(data[17], data[18], true);

                            var linkX = (LinkTextBox)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[3];
                            linkX.SetURL(data[19], "Bing");
                            var linkY = (LinkTextBox)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[4];
                            linkY.SetURL(data[20], "Google");

                            // 5 == Origin
                            var title = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[6];
                            title.SetValues(data[21], data[22], true);
                            var dateTakenBox = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[7];
                            dateTakenBox.SetValues(data[23], data[24], true);

                            var authorBox = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[8];
                            authorBox.SetValues(data[25], data[26], true);
                            var subject = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[9];
                            subject.SetValues(data[27], data[28], true);

                            var program = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[10];
                            program.SetValues(data[29], data[30], true);
                            var copyright = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[11];
                            copyright.SetValues(data[31], data[32], true);

                            // 12 == Image
                            var resolutionUnit = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[13];
                            resolutionUnit.SetValues(data[33], data[34], true);
                            var colorRepresentation = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[14];
                            colorRepresentation.SetValues(data[35], data[36], true);

                            var compression = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[15];
                            compression.SetValues(data[37], data[38], true);
                            var compressionBits = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[16];
                            compressionBits.SetValues(data[39], data[40], true);

                            // 17 == Camera
                            var cameraMaker = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[18];
                            cameraMaker.SetValues(data[41], data[42], true);
                            var cameraModel = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[19];
                            cameraModel.SetValues(data[43], data[44], true);

                            var fstop = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[20];
                            fstop.SetValues(data[45], data[46], true);
                            var exposure = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[21];
                            exposure.SetValues(data[47], data[48], true);

                            var isoSpeed = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[22];
                            isoSpeed.SetValues(data[49], data[50], true);
                            var exposureBias = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[23];
                            exposureBias.SetValues(data[51], data[52], true);

                            var maxAperture = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[24];
                            maxAperture.SetValues(data[53], data[54], true);
                            var focal = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[25];
                            focal.SetValues(data[55], data[56], true);

                            var flashMode = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[26];
                            flashMode.SetValues(data[57], data[58], true);
                            var flashEnergy = (TextboxInfo)ConfigureWindows.GetImageInfoWindow.ExifParent.Children[27];
                            flashEnergy.SetValues(data[59], data[60], true);
                        }
                        else
                        {
                            var latitudeBox = new TextboxInfo(data[15], data[16], true);
                            var longitudeBox = new TextboxInfo(data[17], data[18], true);

                            var linkX = new LinkTextBox(data[19], "Bing");
                            var linkY = new LinkTextBox(data[20], "Google");

                            var title = new TextboxInfo(data[21], data[22], true);
                            var subject = new TextboxInfo(data[23], data[24], true);

                            var authorBox = new TextboxInfo(data[25], data[26], true);
                            var dateTakenBox = new TextboxInfo(data[27], data[28], true);

                            var program = new TextboxInfo(data[29], data[30], true);
                            var copyright = new TextboxInfo(data[31], data[32], true);

                            var resolutionUnit = new TextboxInfo(data[33], data[34], true);
                            var colorRepresentation = new TextboxInfo(data[35], data[36], true);

                            var compression = new TextboxInfo(data[37], data[38], true);
                            var compressionBits = new TextboxInfo(data[39], data[40], true);

                            var cameraMaker = new TextboxInfo(data[41], data[42], true);
                            var cameraModel = new TextboxInfo(data[43], data[44], true);

                            var fstop = new TextboxInfo(data[45], data[46], true);
                            var exposure = new TextboxInfo(data[47], data[48], true);

                            var isoSpeed = new TextboxInfo(data[49], data[50], true);
                            var exposureBias = new TextboxInfo(data[51], data[52], true);

                            var maxAperture = new TextboxInfo(data[53], data[54], true);
                            var focal = new TextboxInfo(data[55], data[56], true);

                            var flashMode = new TextboxInfo(data[57], data[58], true);
                            var flashEnergy = new TextboxInfo(data[59], data[60], true);

                            var gps = (StackPanel)ConfigureWindows.GetImageInfoWindow.Resources["GPS"];
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(gps);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(latitudeBox);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(longitudeBox);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(linkX);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(linkY);

                            var origin = (StackPanel)ConfigureWindows.GetImageInfoWindow.Resources["Origin"];
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(origin);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(title);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(subject);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(authorBox);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(dateTakenBox);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(program);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(copyright);

                            var image = (StackPanel)ConfigureWindows.GetImageInfoWindow.Resources["Image"];
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(image);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(resolutionUnit);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(colorRepresentation);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(compression);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(compressionBits);

                            var camera = (StackPanel)ConfigureWindows.GetImageInfoWindow.Resources["Camera"];
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(camera);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(cameraMaker);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(cameraModel);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(fstop);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(exposure);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(isoSpeed);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(exposureBias);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(maxAperture);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(focal);

                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(flashMode);
                            ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Add(flashEnergy);
                        }
                    }
                }
                else
                {
                    ConfigureWindows.GetImageInfoWindow.FilenameBox.Text =

                    ConfigureWindows.GetImageInfoWindow.FolderBox.Text =

                    ConfigureWindows.GetImageInfoWindow.FullPathBox.Text =

                    ConfigureWindows.GetImageInfoWindow.CreatedBox.Text =

                    ConfigureWindows.GetImageInfoWindow.ModifiedBox.Text =

                    ConfigureWindows.GetImageInfoWindow.WidthBox.Text =

                    ConfigureWindows.GetImageInfoWindow.HeightBox.Text =

                    ConfigureWindows.GetImageInfoWindow.ResolutionBox.Text =

                    ConfigureWindows.GetImageInfoWindow.BitDepthBox.Text =

                    ConfigureWindows.GetImageInfoWindow.SizeMpBox.Text =

                    ConfigureWindows.GetImageInfoWindow.PrintSizeCmBox.Text =

                    ConfigureWindows.GetImageInfoWindow.PrintSizeInBox.Text =

                    ConfigureWindows.GetImageInfoWindow.AspectRatioBox.Text = string.Empty;

                    rating = 0;

                    ConfigureWindows.GetImageInfoWindow.ExifParent.Children.Clear();
                }

                UpdateStars();
            });
        }

        internal static void UpdateStars()
        {
            if (rating is null)
            {
                UpdateStars(0);
                return;
            }

            var castRating = rating.GetType();
            if (castRating.Equals(typeof(int))) // Try and convert to int to avoid exception
            {
                int intRating = (int)rating;
                intRating = intRating >= 0 && intRating <= 5 ? intRating : 0;
                UpdateStars((intRating));
                return;
            }

            if ((string)rating == string.Empty || (string)rating == "0")
            {
                UpdateStars(0);
                return;
            }

            int percent = Convert.ToInt32(rating.ToString());
            var stars = Math.Ceiling(percent / 20d);

            UpdateStars((int)stars);
        }

        internal static void UpdateStars(int stars)
        {
            switch (stars)
            {
                default:
                case 0:
                    ConfigureWindows.GetImageInfoWindow.Star1.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.OutlineStar();
                    return;
                case 1:
                    ConfigureWindows.GetImageInfoWindow.Star1.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.OutlineStar();
                    return;
                case 2:
                    ConfigureWindows.GetImageInfoWindow.Star1.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.OutlineStar();
                    return;
                case 3:
                    ConfigureWindows.GetImageInfoWindow.Star1.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.OutlineStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.OutlineStar();
                    return;
                case 4:
                    ConfigureWindows.GetImageInfoWindow.Star1.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.OutlineStar();
                    return;
                case 5:
                    ConfigureWindows.GetImageInfoWindow.Star1.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star2.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star3.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star4.FillStar();
                    ConfigureWindows.GetImageInfoWindow.Star5.FillStar();
                    return;
            }
        }
    }
}
