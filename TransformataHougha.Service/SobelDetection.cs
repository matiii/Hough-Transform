using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class SobelDetection
    {
        private Bitmap image;
        private PixelFormat pixelFormat;

        public SobelDetection(Image loadPicture, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            this.image = new Bitmap(loadPicture);
            this.pixelFormat = pixelFormat;
        }


        /// <summary>
        /// Is rised when SobelEdgeDetection completed
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Method wchich edge detection, in the event return picture
        /// </summary>
        unsafe public void SobelEdgeDetection()
        {
            var dataImage = new DataPicture(image, pixelFormat);
            var copy = dataImage.Picture;

            byte* wsk = (byte*)copy.Scan0;
            byte* wskCopy = (byte*)copy.Scan0;

            int nOffSet = copy.Stride - copy.Width * 3;

            int width = dataImage.GetWidth;
            int height = dataImage.GetHeight;

            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            int[,] allPixR = new int[width, height];
            int[,] allPixG = new int[width, height];
            int[,] allPixB = new int[width, height];

            int limit = 128 * 128;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    allPixR[i, j] = wsk[2]; //red
                    allPixG[i, j] = wsk[1]; //green
                    allPixB[i, j] = wsk[0]; //blue

                    wsk += 3*width+nOffSet;
                }
                wsk -= (3*width+nOffSet)*height - 3;
            }


            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;

            wskCopy += 3 * width + nOffSet + 3;

            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {

                    new_rx = 0;
                    new_ry = 0;
                    new_gx = 0;
                    new_gy = 0;
                    new_bx = 0;
                    new_by = 0;
                    rc = 0;
                    gc = 0;
                    bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = allPixR[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = allPixG[i + hw, j + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = allPixB[i + hw, j + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                    {
                        wskCopy[0] = 0; wskCopy[1] = 0; wskCopy[2] = 0;
                    }
                    else
                    {
                        wskCopy[0] = 255; wskCopy[1] = 255; wskCopy[2] = 255;
                    }

                    wskCopy += 3 * width + nOffSet;
                }

                wskCopy -= (3 * width + nOffSet) * (height-2) - 3;
            }

            dataImage.LoadBitmapFromBitmapData();


            if (Completed != null)
            {
                Completed(dataImage.GetOriginalPicture, new SobelArgs() { IsCompleted = true });
            }

        }
    }
}
