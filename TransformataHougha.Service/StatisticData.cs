using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class StatisticData
    {
        private ICollection<Point> cleanPoint;
        private ICollection<Point> noisePoint;
        private readonly int sumPoints;

        private int tp = 0;
        private int fp = 0;
        private int fn = 0;

        public StatisticData(int width, int height)
        {
            this.cleanPoint = new List<Point>();
            this.noisePoint = new List<Point>();

            this.sumPoints = width * height;
        }

        public StatisticData(IEnumerable<Point> cleanPoint, IEnumerable<Point> noisePoint, int width, int height)
        {
            this.cleanPoint = cleanPoint as ICollection<Point>;
            this.noisePoint = noisePoint as ICollection<Point>;

            this.sumPoints = width * height;
        }

        public void AddCleanPoint(Point point)
        {
            cleanPoint.Add(point);
        }

        public void AddNoisePoint(Point point)
        {
            noisePoint.Add(point);
        }

        public int GetTP
        {
            get 
            {
                if (tp == 0)
                {
                    int counter = 0;

                    Expected(ref counter,
                        (x, y) => 
                        {
                            return cleanPoint.Any( p => p.X == x && p.Y == y);
                        });

                    tp = counter;

                    return counter;
                }
                else
                {
                    return tp;
                }
            }
        }

        public int GetFP
        {
            get
            {
                if (fp == 0)
                {
                    int counter = 0;

                    Expected(ref counter,
                        (x, y) =>
                        {
                            return !cleanPoint.Any(p => p.X == x && p.Y == y);
                        });

                    fp = counter;

                    return counter;
                }
                else
                {
                    return fp;
                }
            }
        }

        public int GetFN
        {
            get
            {
                if (fn == 0)
                {
                    int counter = 0;
                    NotExpected(ref counter,
                        (x, y) =>
                        {
                            return !noisePoint.Any(p => p.X == x && p.Y == y);
                        });

                    fn = counter;

                    return counter;
                }
                else
                {
                    return fn;
                }
            }
        }

        public int GetTN
        {
            get
            {
                return sumPoints - (GetTP + GetFP + GetFN);
            }
        }

        private void Expected(ref int counter, Func<int,int,bool> func)
        {
            counter = 0;

            foreach (var point in noisePoint)
            {
                if (func(point.X, point.Y))
                {
                    counter++;
                }
            }
        }

        private void NotExpected(ref int counter, Func<int, int, bool> func)
        {
            counter = 0;

            foreach (var point in cleanPoint)
            {
                if (func(point.X, point.Y))
                {
                    counter++;
                }
            }
        }
    }
}
