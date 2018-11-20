using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageScaler
{
    class Program
    {
        const string directory = "ScaledImages";

        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "-android":
                    Directory.CreateDirectory(directory);
                    CreateMipMapFiles(directory, args[1], "mipmap-mdpi", "48", "108");
                    CreateMipMapFiles(directory, args[1], "mipmap-hdpi", "72", "162");
                    CreateMipMapFiles(directory, args[1], "mipmap-xhdpi", "96", "216");
                    CreateMipMapFiles(directory, args[1], "mipmap-xxhdpi", "144", "324");
                    CreateMipMapFiles(directory, args[1], "mipmap-xxxhdpi", "192", "432");
                    return;
                case "-playstore":
                    Directory.CreateDirectory(directory);
                    ScaleImage(args[1], "512").Save(Path.Combine(directory, "hi-res icon.png"));
                    ScaleImageOnBackground(args[1], args[2], "1024x500").Save(Path.Combine(directory, "feature graphic.png"));
                    ScaleImageOnBackground(args[1], args[2], "180x120").Save(Path.Combine(directory, "promo graphic.png"));
                    return;
                default:
                    var path = args[0];
                    Directory.CreateDirectory(directory);
                    ScaleImage(path, args[1]).Save(Path.Combine(directory, Path.GetFileName(path)));
                    return;
            }
        }

        private static void CreateMipMapFiles(string baseDirectory, string filenameArg, string mipmap, string sizeArgIcon, string sizeArgLauncher)
        {
            var path = Path.Combine(baseDirectory, mipmap);
            Directory.CreateDirectory(path);
            ScaleImage(filenameArg, sizeArgIcon).Save(Path.Combine(path, "icon.png"));
            ScaleImageForLauncher(filenameArg, sizeArgIcon, sizeArgLauncher).Save(Path.Combine(path, "launcher_foreground.png"));
        }

        private static Bitmap ScaleImage(string filenameArg, string sizeArg)
        {
            var scaled = ParseSize(sizeArg);
            var bitmap = Image.FromFile(filenameArg);
            using (var g = Graphics.FromImage(scaled))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bitmap, 0, 0, scaled.Width, scaled.Height);
            }
            return scaled;
        }

        private static Bitmap ScaleImageForLauncher(string filenameArg, string sizeArg, string sizeArgForLauncher)
        {
            var scaled = ParseSize(sizeArgForLauncher);
            var inner = ParseSize(sizeArg);
            var bitmap = Image.FromFile(filenameArg);
            using (var g = Graphics.FromImage(scaled))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bitmap, (float)Math.Round(scaled.Width / 2.0 - inner.Width / 2.0), (float)Math.Round(scaled.Height / 2.0 - inner.Height / 2.0), inner.Width, inner.Height);
            }
            return scaled;
        }

        private static Bitmap ScaleImageOnBackground(string filenameArg, string backgroundFilenameArg, string sizeArg)
        {
            var scaled = ParseSize(sizeArg, PixelFormat.Format24bppRgb);
            var foreground = Image.FromFile(filenameArg);
            var background = Image.FromFile(backgroundFilenameArg);
            var squareW = (float)Math.Round(Math.Min(scaled.Width, scaled.Height) * 0.85);
            using (var g = Graphics.FromImage(scaled))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(background, 0, 0, scaled.Width, scaled.Height);
                g.DrawImage(foreground, (float)Math.Round(scaled.Width / 2 - squareW / 2), (float)Math.Round(scaled.Height / 2 - squareW / 2), squareW, squareW);
            }
            return scaled;
        }

        private static Bitmap ParseSize(string arg, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var x = arg.IndexOf("x");
            if (x != -1)
            {
                var w = int.Parse(arg.Substring(0, x));
                var h = int.Parse(arg.Substring(x + 1, arg.Length - x - 1));
                return new Bitmap(w, h, format);
            }
            else
            {
                var w = int.Parse(arg);
                return new Bitmap(w, w, format);
            }
        }
    }
}
