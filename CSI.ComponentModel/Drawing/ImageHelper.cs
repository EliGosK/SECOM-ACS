using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CSI.Drawing
{
    public static class ImageHelper
    {
        public static byte[] GetImageData(Stream stream, ImageFormat format)
        {
            byte[] buffer;
            using (Image image = Bitmap.FromStream(stream))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);

                    //return the current position in the stream at the beginning
                    ms.Position = 0;

                    buffer = new byte[ms.Length];
                    ms.Read(buffer, 0, (int)ms.Length);
                    return buffer;
                }
            }
        }


        public static byte[] GetImageData(string filePath)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(filePath))
            {
                //System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] GetImageData(string filePath, Size boundedSize)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(filePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    System.Drawing.Image targetImg = ResizeImage(ms, boundedSize);

                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        targetImg.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);

                        return ms2.ToArray();
                    }
                }
            }
        }

        public static Bitmap ResizeImage(Image image, Size boundedSize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ResizeImage(ms, boundedSize);
            }
        }
        
        public static Bitmap ResizeImage(Stream stream,double ratio)
        {
            Image sourceImage = Bitmap.FromStream(stream);
            int width = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(sourceImage.Width * ratio)));
            int height = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(sourceImage.Height * ratio)));

            Bitmap scaledImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(sourceImage, 0, 0, width, height);
                return scaledImage;
            }
        }

        public static Bitmap ResizeImage(Stream stream, Size boundedSize)
        {
            return ResizeImage(stream, boundedSize, ResizeImageQuality.Default);
        }

        public static Bitmap ResizeImage(Stream stream, Size boundedSize,ResizeImageQuality quality)
        {
            Image sourceImage = Bitmap.FromStream(stream);
            // 1200, 900
            int height = sourceImage.Height;
            int width = sourceImage.Width;
            // 1024 x 768
            if (sourceImage.Width > sourceImage.Height)
            {
                if (sourceImage.Width > boundedSize.Width)
                {
                    // Landscape image 
                    double ratio = Convert.ToDouble(boundedSize.Width) / Convert.ToDouble(sourceImage.Width);
                    height = Convert.ToInt32(sourceImage.Height * ratio);
                    width = Convert.ToInt32(sourceImage.Width * ratio);
                }
                else if (sourceImage.Height > boundedSize.Height)
                {
                    double ratio = Convert.ToDouble(boundedSize.Height) / Convert.ToDouble(sourceImage.Height);
                    height = Convert.ToInt32(sourceImage.Height * ratio);
                    width = Convert.ToInt32(sourceImage.Width * ratio);
                }
            }
            else if (sourceImage.Height > sourceImage.Width)
            {
                if (sourceImage.Height> boundedSize.Height)
                {
                    double ratio = Convert.ToDouble(boundedSize.Height) / Convert.ToDouble(sourceImage.Height);
                    height = Convert.ToInt32(sourceImage.Height * ratio);
                    width = Convert.ToInt32(sourceImage.Width * ratio);
                }
                else if (sourceImage.Width> boundedSize.Width)
                {
                    // Landscape image 
                    double ratio = Convert.ToDouble(boundedSize.Width) / Convert.ToDouble(sourceImage.Width);
                    height = Convert.ToInt32(sourceImage.Height * ratio);
                    width = Convert.ToInt32(sourceImage.Width * ratio);
                }
            }

            Bitmap scaledImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                switch (quality)
                {
                    case ResizeImageQuality.Default:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        g.CompositingQuality = CompositingQuality.Default;
                        g.PixelOffsetMode = PixelOffsetMode.Default;
                        break;
                    case ResizeImageQuality.HighSpeed:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        g.CompositingQuality = CompositingQuality.HighSpeed;
                        g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                        break;
                    case ResizeImageQuality.HighQuality:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        break;
                    default:
                        break;
                }                
                g.DrawImage(sourceImage, 0, 0, width, height);
                return scaledImage;
            }

        }
    }
}
