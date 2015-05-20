using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class RandomNoise
    {
        private double probability;
        private Bitmap picture;
        private PixelFormat pixelFormat;


        public event EventHandler Completed;

        public RandomNoise(Image image, decimal p, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            this.picture = new Bitmap(image);
            this.pixelFormat = pixelFormat;
            this.probability = (double) p / 100;
        }

        public unsafe void MakeNoise()
        {
            var dataImage = new DataPicture(picture, pixelFormat);

            byte* wsk = (byte*)dataImage.Picture.Scan0;
            int nOffSet = dataImage.Picture.Stride - dataImage.Picture.Width * 3;

            var rand = new Random();

            for (int i = 0; i < picture.Height; i++)
            {
                for (int j = 0; j < picture.Width; j++)
                {
                    var next = rand.NextDouble();

                    if (next <= probability)
                    {
                        next = rand.NextDouble();

                        if (next > 0.1)
                        {
                            wsk[0] = wsk[1] = wsk[2] = 255;
                        }
                        else
                        {
                            wsk[0] = wsk[1] = wsk[2] = 0;
                        }

                        //if (wsk[0] == 0)
                        //{
                        //    wsk[0] = wsk[1] = wsk[2] = 255;
                        //}
                        //else
                        //{
                        //    wsk[0] = wsk[1] = wsk[2] = 0;
                        //}
                        //wsk[0] = wsk[1] = wsk[2] = ChangePixel(ref dataImage, picture.Height, picture.Width, wsk[0]).ToByte();
                    }

                    wsk += 3;
                }

                wsk += nOffSet;
            }

            dataImage.LoadBitmapFromBitmapData();
            picture = dataImage.GetOriginalPicture;

            if (Completed != null)
            {
                Completed(picture, null);
            }
        }

        private unsafe int ChangePixel(ref DataPicture dataImage, int pictureHeight, int pictureWidth, int color)
        {
            byte* wsk = (byte*)dataImage.Picture.Scan0;
            int nOffSet = dataImage.Picture.Stride - dataImage.Picture.Width * 3;

            var rand = new Random();

            int x = rand.Next(0, pictureHeight);
            int y = rand.Next(0, pictureWidth);

            int was = (3 * pictureWidth + nOffSet) * x + 3*y;

            wsk += was;

            var item = wsk;

            int c = wsk[0];

            wsk[0] = wsk[1] = wsk[2] = color.ToByte();

            return c;
        }
    }


    public static class ClassRandom
    {
        private static Random _randomGen = new Random();
        public static int iset = 0;
        public static double gset = 0;

        private static double GenerateGaussNoiseNorm()
        {
            double fac, r, v1, v2;

            if (iset == 0)
            {
                do
                {
                    v1 = 2.0 * _randomGen.NextDouble() - 1.0;
                    v2 = 2.0 * _randomGen.NextDouble() - 1.0;
                    r = v1 * v1 + v2 * v2;
                } while (r >= 1.0);
                fac = Math.Sqrt(-2.0 * Math.Log(r) / r);
                gset = v1 * fac;
                iset = 1;
                return v2 * fac;
            }
            else
            {
                iset = 0;
                return gset;
            }
        }

        private static double GenerateGaussNoise(double sigma)
        {
            double m = _randomGen.NextDouble();
            return GenerateGaussNoiseNorm() * sigma + m;
        } // GenerateGaussNoise

        public static Bitmap GaussNoiseImage(Image image, double sigma = 1.0)
        {
            int x, y;
            double A, R, G, B;
            Color pixelColor;
            var pic = new Bitmap(image);

            for (x = 0; x < pic.Width; x++)
                for (y = 0; y < pic.Height; y++)
                {
                    pixelColor = pic.GetPixel(x, y);

                    //Generating noise
                    double noise = GenerateGaussNoise(sigma);
                    double gre = pic.GetPixel(x, y).GetBrightness();
                    gre += Math.Round(noise);

                    A = pixelColor.A;
                    R = pixelColor.R + gre;
                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G + gre;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B + gre;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    pic.SetPixel(x, y, Color.FromArgb(255, (int)R, (int)G, (int)B));
                } // for (x,y)

            return pic;
        }
    }
}
