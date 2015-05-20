using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformataHougha.Service
{
    static class DoubleExtension
    {
        public static int ToInt(this double number)
        {
            return Convert.ToInt32(number);
        }
    }
}
