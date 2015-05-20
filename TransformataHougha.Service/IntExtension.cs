using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    public static class IntExtension
    {
        public static byte ToByte(this int number)
        {
            return (byte)number;
        }

        public static double ToDouble(this int number)
        {
            return (double)number;
        }
    }
}
