﻿using Microsoft.Win32;
using PicView.ChangeImage;
using PicView.ImageHandling;
using PicView.UILogic;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static PicView.ChangeImage.Error_Handling;
using static PicView.ChangeImage.Navigation;
using static PicView.UILogic.Tooltip;
using static PicView.UILogic.TransformImage.Rotation;
using static PicView.UILogic.UC;

namespace PicView.FileHandling
{
    internal static class Open_Save
    {
        internal static bool IsDialogOpen { get; set; }

        /// <summary>
        ///  Files filterering string used for file/save dialog
        ///  TODO update for and check file support
        /// </summary>
        internal const string FilterFiles =
            "Pictures|*.bmp;*.jpg;*.png;.tif;*.gif;*.ico;*.jpeg;*.webp;"                                    // Common pics
            + "|jpg| *.jpg;*.jpeg"                                                                          // JPG
            + "|PNG|*.png;"                                                                                 // PNG
            + "|gif|*.gif;"                                                                                 // GIF
            + "|ico|*.ico;"                                                                                 // ICO
            + "|svg|*.svg;"                                                                                 // SVG
            + "|webp|*.webp;"                                                                               // WEBP
            + "|tga|*.tga;"                                                                                 // TGA
            + "|dds|*.dds;"                                                                                 // DDS
            + "|ico|*.ico;"                                                                                 // ICO
            + "|wdp|*.wdp;"                                                                                 // WDP
            + "|svg|*.svg;"                                                                                 // SVG
            + "|Photoshop|*.psd;*.psb"                                                                      // PSD
            + "|GIMP|*.xcf"                                                                                 // GIMP
            + "|Archives|*.zip;*.7zip;*.7z;*.rar;*.bzip2;*.tar;*.wim;*.iso;*.cab"                           // Archives
            + "|Comics|*.cbr;*.cb7;*.cbt;*.cbz;*.xz"                                                        // Comics
            + "|Camera files|*.orf;*.cr2;*.crw;*.dng;*.raf;*.ppm;*.raw;*.mrw;*.nef;*.pef;*.3xf;*.arw";      // Camera files

        /// <summary>
        /// Opens image in File Explorer
        /// </summary>
        internal static void Open_In_Explorer()
        {
            if (Pics?.Count > 0)
            {
                if (Pics.Count < FolderIndex)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            if (!File.Exists(Pics[FolderIndex]) || ConfigureWindows.GetMainWindow.MainImage.Source == null)
            {
                return;
            }
            try
            {
                Close_UserControls();
                FileFunctions.OpenFolderAndSelectItem(Path.GetDirectoryName(Pics?[FolderIndex]), Pics?[FolderIndex]); // https://stackoverflow.com/a/39427395
            }
#if DEBUG
            catch (InvalidCastException e)
            {
                Trace.WriteLine("Open_In_Explorer exception \n" + e.Message);
            }
#else
            catch (InvalidCastException) { }
#endif
        }

        /// <summary>
        /// Open a file dialog where user can select a supported file
        /// </summary>
        internal static async Task OpenAsync()
        {
            IsDialogOpen = true;

            var dlg = new OpenFileDialog()
            {
                Filter = FilterFiles,
                Title = $"{Application.Current.Resources["OpenFileDialog"]} - {SetTitle.AppName}"
            };
            if (dlg.ShowDialog() == true)
            {
                await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
                {
                    ToggleStartUpUC(true);
                });
                await LoadPic.LoadPiFromFileAsync(dlg.FileName).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Start Windows "Open With" function
        /// </summary>
        /// <param name="file">The absolute path to the file</param>
        internal static void OpenWith(string file)
        {
            try
            {
                using var process = new Process();
                process.StartInfo.FileName = "openwith";
                process.StartInfo.Arguments = $"\"{file}\"";
                process.StartInfo.ErrorDialog = true;

                process.Start();
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine("OpenWith exception \n" + e.Message);

#endif
                ShowTooltipMessage(e.Message, true);
            }
        }

        /// <summary>
        /// Open a File Dialog, where the user can save a supported file type.
        /// </summary>
        internal static async Task SaveFilesAsync()
        {
            if (ConfigureWindows.GetMainWindow.MainImage.Source == null)
            {
                return;
            }

            string fileName;
            bool randomized = false;

            if (Pics?.Count > FolderIndex)
            {
                if (string.IsNullOrEmpty(Pics[FolderIndex]))
                {
                    return;
                }
                fileName = Path.GetFileName(Pics[FolderIndex]);
            }
            else
            {
                fileName = Path.GetRandomFileName();
                randomized = true;
            }

            var Savedlg = new SaveFileDialog()
            {
                Filter = FilterFiles,
                Title = Application.Current.Resources["Save"] + $" - {SetTitle.AppName}",
                FileName = fileName,
            };

            if (randomized is false)
            {
                Savedlg.InitialDirectory = Path.GetDirectoryName(Pics[FolderIndex]);
            }

            if (!Savedlg.ShowDialog().HasValue)
            {
                return;
            }

            IsDialogOpen = true;

            var success = false;
            var source = ConfigureWindows.GetMainWindow.MainImage.Source as BitmapSource;
            var effectApplied = ConfigureWindows.GetMainWindow.MainImage.Effect != null;

            if (Pics?.Count > FolderIndex)
            {
                success = await SaveImages.SaveImageAsync(Rotateint, Flipped, null, Pics[FolderIndex], Savedlg.FileName, null, effectApplied).ConfigureAwait(false);
            }
            else if (source != null)
            {
                success = await SaveImages.SaveImageAsync(Rotateint, Flipped, source, null, Savedlg.FileName, null, effectApplied).ConfigureAwait(false);
            }

            if (success == false)
            {
                ShowTooltipMessage(Application.Current.Resources["SavingFileFailed"]);
            }

            //Reload if same pic to show changes
            else if (Savedlg.FileName == fileName)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
            {
                Close_UserControls();
            });

            IsDialogOpen = false;
        }

        /// <summary>
        /// Sends the file to Windows print system
        /// </summary>
        /// <param name="path">The file path</param>
        internal static bool Print(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = path;
                p.StartInfo.Verb = "print";
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            return true;
        }
    }
}