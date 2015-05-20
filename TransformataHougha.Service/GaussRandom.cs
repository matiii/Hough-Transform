using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public class GaussRandom
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mean">distribution</param>
        /// <param name="stdDev">deviation</param>
        /// <returns></returns>
        public double NextRandom(int mean = 0, int stdDev = 1)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}
