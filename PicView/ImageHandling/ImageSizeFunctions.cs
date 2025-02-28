﻿using ImageMagick;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PicView.ImageHandling
{
    internal static class ImageSizeFunctions
    {
        internal static async Task<Size?> GetImageSizeAsync(FileInfo fileInfo)
        {
            using var magick = new MagickImage();
            if (fileInfo.Length > 2e+9)
            {
#if DEBUG
                Trace.WriteLine("File size bigger than 2gb");
#endif
                return null;
            }
            try
            {
                await magick.ReadAsync(fileInfo).ConfigureAwait(false);
            }
#if DEBUG
            catch (MagickException e)
            {
                Trace.WriteLine("ImageSize returned " + fileInfo.FullName + " null, \n" + e.Message);
                return null;
            }
#else
                catch (MagickException) { return null; }
#endif

            return new Size(magick.Width, magick.Height);
        }

        internal static async Task<Size?> GetImageSizeAsync(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            return await GetImageSizeAsync(fileInfo).ConfigureAwait(false);
        }

        internal static async Task<bool> ResizeImageAsync(string file, int width, int height, int quality = 100, Percentage? percentage = null)
        {
            if (string.IsNullOrWhiteSpace(file)) { return false; }
            if (File.Exists(file) == false) { return false; }
            if (width < 0 && percentage is not null || height < 0 && percentage is not null) { return false; }

            var magick = new MagickImage()
            {
                ColorSpace = ColorSpace.Transparent
            };

            if (quality > 0) // not inputting quality results in lower file size
            {
                magick.Quality = quality;
            }

            try
            {
                await magick.ReadAsync(file).ConfigureAwait(false);
            }
            catch (MagickException e)
            {
#if DEBUG
                Trace.WriteLine($"{nameof(ResizeImageAsync)} magic read exception caught \n {e.Message}");
#endif
                return false;
            }

            try
            {
                if (percentage is not null)
                {
                    magick.Resize(percentage.Value);
                }
                else
                {
                    magick.Resize(width, height);
                }

                await magick.WriteAsync(file).ConfigureAwait(false);
            }
            catch (MagickException e)
            {
#if DEBUG
                Trace.WriteLine($"{nameof(ResizeImageAsync)} exception caught \n {e.Message}");
#endif
                return false;
            }

            magick.Dispose();
            return true;
        }
    }
}
