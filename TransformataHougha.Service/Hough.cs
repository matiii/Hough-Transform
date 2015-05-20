using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class Hough
    {
        /// <summary>
        /// Handle event to get picutres list in Transform method
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Get draw point
        /// </summary>
        public event EventHandler GetPoint;

        private Bitmap originalPicture, accumulator;
        private PixelFormat pixelFormat;
        private int diagonal;
        private bool allLine;

        public Hough(Bitmap loadPicture, PixelFormat pixelFormat = PixelFormat.Format24bppRgb, bool allLine = true)
        {
            this.originalPicture = new Bitmap( loadPicture);
            this.pixelFormat = pixelFormat;
            this.diagonal = (int)(Math.Sqrt(originalPicture.Width * originalPicture.Width + originalPicture.Height * originalPicture.Height)) + 1;
            this.accumulator = new Bitmap(271, diagonal, pixelFormat); //270 angles
            this.allLine = !allLine;
        }

        /// <summary>
        /// Find lines, handle completed to get picture list
        /// </summary>
        /// <param name="amount">Amount lines</param>
        /// <returns>Task</returns>
        public unsafe void Transform(int amount = 1)
        {
            var dataImage = new DataPicture(originalPicture, pixelFormat);
            var dataAccumulator = new DataPicture(accumulator, pixelFormat);
            
            int R;
            int[,] taransf = new int[diagonal, 271];     // from -90 to 180 (270 angels)

            int colorLine = 0;                         // 0 - edges color black

            int[] max = new int[3];

            byte* wsk = (byte*)dataImage.Picture.Scan0;
            int nOffSet = dataImage.Picture.Stride - dataImage.Picture.Width * 3;

            for (int y = 0; y < originalPicture.Height; y++)       // transformata Hough
            {
                for (int x = 0; x < originalPicture.Width; x++)
                {
                    if (wsk[2] == colorLine)
                    {
                        for (int m = -90; m <= 180; m++)
                        {
                            R = (int)(y * Math.Sin(Math.PI * m / 180) + x * Math.Cos(Math.PI * m / 180));
                            if (R < 0)
                                R = -R;
                            taransf[R, m + 90] += 1;

                            if (taransf[R, m + 90] > max[0])
                            {
                                max[0] = taransf[R, m + 90];
                                max[1] = R;
                                max[2] = m;
                            }
                        }
                    }

                    wsk += 3;
                }

                wsk += nOffSet;
            }

            wsk = (byte*)dataAccumulator.Picture.Scan0;
            nOffSet = dataAccumulator.Picture.Stride - dataAccumulator.Picture.Width * 3;

            for (int j = 0; j < accumulator.Height; j++)        // poziom jasności
            {
                for (int i = 0; i < accumulator.Width; i++)
                {
                    double jas = (double)(255.0 * (double)(Convert.ToDouble(taransf[j, i]) / Convert.ToDouble(max[0])));
                    byte jasInt = Convert.ToByte(jas);
                    wsk[0] = wsk[1] = wsk[2] = jasInt;

                    wsk += 3;
                }

                wsk += nOffSet;
            }

            int[,] naj = new int[amount, 3];                   // analiza maximum

            wsk = (byte*)dataAccumulator.Picture.Scan0;

            for (int j = 0; j < accumulator.Height; j++)
            {
                for (int i = 0; i < accumulator.Width; i++)
                {
                    int tmp = GetMinimumCoordinate(naj, amount);
                    if (wsk[2] > naj[tmp, 0])
                    {
                        naj[tmp, 0] = wsk[2];
                        naj[tmp, 1] = i;
                        naj[tmp, 2] = j;
                    }

                    wsk += 3;
                }

                wsk += nOffSet;
            }

            dataAccumulator.LoadBitmapFromBitmapData();
            accumulator = dataAccumulator.GetOriginalPicture;

            for (int i = 0; i < amount; i++)
            {
                accumulator.SetPixel(naj[i, 1], naj[i, 2], Color.Red);
                naj[i, 1] -= 90;
            }

            for (int i = 0; i < amount; i++)
            {
                wsk = (byte*)dataImage.Picture.Scan0;
                nOffSet = dataImage.Picture.Stride - dataImage.Picture.Width * 3;

                for (int y = 0; y < originalPicture.Height; y++) // rysowanie
                {
                    for (int x = 0; x < originalPicture.Width; x++)
                    {
                        if ((int)(y * Math.Sin(Math.PI * naj[i, 1] / 180) + x * Math.Cos(Math.PI * naj[i, 1] / 180)) == naj[i, 2] || (y * Math.Sin(Math.PI * naj[i, 1] / 180) + x * Math.Cos(Math.PI * naj[i, 1] / 180)) == -naj[i, 2])
                        {
                            if (wsk[2] == colorLine || allLine)
                            {
                                if (GetPoint != null)
                                {
                                    GetPoint(new Point(x, y), null);
                                }

                                wsk[0] = wsk[1] = 0;
                                wsk[2] = 255;
                            }
                        }

                        wsk += 3;
                    }

                    wsk += nOffSet;
                }
            }

            dataImage.LoadBitmapFromBitmapData();

            originalPicture = dataImage.GetOriginalPicture;

            if (Completed != null)
            {
                var lista = new List<Bitmap>() { originalPicture, accumulator };
                Completed(lista, null);
            }
        }

        /// <summary>
        /// Get the minimum value coordinate x in array
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private int GetMinimumCoordinate(int[,] collection, int amount)
        {
            int tmp = collection[0, 0];
            int x = 0;

            for (int i = 0; i < amount; i++)
            {
                if (collection[i, 0] < tmp)
                {
                    tmp = collection[i, 0];
                    x = i;
                }
            }

            return x;
        }
    }
}
