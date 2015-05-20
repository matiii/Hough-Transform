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
    /// Container for original picture and modyfication picture
    /// </summary>
    public interface IDataPicture
    {
        void LoadPicture(Bitmap picture, PixelFormat format);
        Bitmap GetOriginalPicture { get; }
        BitmapData Picture { get; set; }
    }
}
