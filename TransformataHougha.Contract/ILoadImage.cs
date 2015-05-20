using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Contract
{
    /// <summary>
    /// Load image from path
    /// </summary>
    public interface ILoadImage
    {
        void LoadImageFromPath(string path);
        Bitmap GetPicture { get; }
        PixelFormat GetPixelFormat { get; }
    }
}
