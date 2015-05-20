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
    /// Load picture from Bitmap to BitmapData
    /// </summary>
    public class DataPicture
    {
        private BitmapData picture;
        private Bitmap originalPicture;
        private Bitmap copy;

        public DataPicture()
        { }

        public DataPicture(Bitmap picture, PixelFormat format)
        {
            LoadPicture(picture, format);
        }

        public Bitmap GetCopy
        {
            get { return copy; }
        }

        /// <summary>
        /// Get original load picture
        /// </summary>
        public Bitmap GetOriginalPicture
        {
            get { return originalPicture; }
        }

        /// <summary>
        /// Get current picture
        /// </summary>
        public BitmapData Picture
        {
            get
            {
                return picture;
            }
        }

        /// <summary>
        /// Get Height picture
        /// </summary>
        public int GetHeight
        {
            get { return originalPicture.Height; }
        }

        /// <summary>
        /// Get width picture
        /// </summary>
        public int GetWidth
        {
            get { return originalPicture.Width; }
        }

        /// <summary>
        /// Load picture to class
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="format"></param>
        public void LoadPicture(Bitmap picture, PixelFormat format)
        {
            this.originalPicture = new Bitmap(picture);
            this.copy = new Bitmap(picture);
            this.picture = originalPicture.LockBits(new Rectangle(0, 0, originalPicture.Width, originalPicture.Height), ImageLockMode.ReadWrite, format);
        
        }

        /// <summary>
        /// Convert BitmapData to original picture
        /// </summary>
        /// <param name="data"></param>
        public void LoadBitmapFromBitmapData(BitmapData data)
        {
            originalPicture.UnlockBits(data);
        }

        /// <summary>
        /// Convert BitmapData to original picture
        /// </summary>
        /// <param name="data"></param>
        public void LoadBitmapFromBitmapData()
        {
            originalPicture.UnlockBits(picture);
        }
    }
}
