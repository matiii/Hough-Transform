using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    /// <summary>
    /// Load image from path to BitMap
    /// </summary>
    public class LoadImage
    {
        private Bitmap picture;
        private PixelFormat pixelFormat;

        public void LoadImageFromPath(string path)
        {
            picture = new Bitmap(path);

            CheckFormat();
        }

        /// <summary>
        /// Get Load picture
        /// </summary>
        public Bitmap GetPicture
        {
            get { return picture; }
        }

        /// <summary>
        /// Get pixel format
        /// </summary>
        public PixelFormat GetPixelFormat
        {
            get { return pixelFormat; }
        }


        /// <summary>
        /// Check format picture
        /// </summary>
        private void CheckFormat()
        {
            if (picture == null)
            {
                throw new Exception("You need load image first.", new Exception("Use LoadImageFromPath method."));
            }

            if (picture.PixelFormat != PixelFormat.Format8bppIndexed && picture.PixelFormat != PixelFormat.Format24bppRgb)
            {
                Bitmap bmp = new Bitmap(picture.Width, picture.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmp);

                using (g)
                {
                    g.DrawImage(picture, 0, 0, picture.Width, picture.Height);
                }

                picture = bmp;
            }

            pixelFormat = picture.PixelFormat;
        }
    }
}
