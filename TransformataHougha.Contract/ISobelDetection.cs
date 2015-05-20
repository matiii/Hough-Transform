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
    /// Make Sobel Edge Detection
    /// </summary>
    public interface ISobelDetection
    {
        void SobelEdgeDetection(BitmapData image);
        Bitmap GetPicture { get; }
        event EventHandler Completed;
    }
}
