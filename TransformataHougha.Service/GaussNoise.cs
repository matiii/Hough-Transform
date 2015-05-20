using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class GaussNoise
    {
        private Bitmap image;
        private PixelFormat pixelFormat;

        public event EventHandler Completed;

        public GaussNoise(Image image, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            this.image = new Bitmap(image);
            this.pixelFormat = pixelFormat;
        }

        public unsafe void MakeNoise(int mean = 0, int stdDev = 1)
        {
            var dataImage = new DataPicture(image, pixelFormat);

            byte* wsk = (byte*)dataImage.Picture.Scan0;
            int nOffSet = dataImage.Picture.Stride - dataImage.Picture.Width * 3;

            var gauss = new GaussRandom();
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    int colorB = wsk[0] + gauss.NextRandom(mean, stdDev).ToInt();
                    int colorG = wsk[1] + gauss.NextRandom(mean, stdDev).ToInt();
                    int colorR = wsk[2] + gauss.NextRandom(mean, stdDev).ToInt();

                    colorR = colorR > 255 ? 255 : colorR;
                    colorR = colorR < 0 ? 0 : colorR;

                    colorG = colorG > 255 ? 255 : colorG;
                    colorG = colorG < 0 ? 0 : colorG;

                    colorB = colorB > 255 ? 255 : colorB;
                    colorB = colorB < 0 ? 0 : colorB;

                    wsk[0] = colorB.ToByte(); wsk[1] = colorG.ToByte(); wsk[2] = colorR.ToByte();

                    wsk += 3;
                }

                wsk += nOffSet;
            }

            dataImage.LoadBitmapFromBitmapData();
            image = dataImage.GetOriginalPicture;

            if (Completed != null)
            {
                Completed(image, null);
            }
        }
    }
}
