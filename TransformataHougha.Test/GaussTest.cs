using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformataHougha.Service;

namespace TransformataHougha.Test
{
    [TestClass]
    public class GaussTest
    {
        [TestMethod]
        public void Generate_Random_Values()
        {
            var g = new GaussRandom();

            double t1 = g.NextRandom(0, 255);
            double t2 = g.NextRandom(0, 255);
            double t3 = g.NextRandom(0, 255);
            double t4 = g.NextRandom(0, 255);
            double t5 = g.NextRandom(0, 255);

            Assert.AreNotEqual<double>(t1, t2);
            Assert.AreNotEqual<double>(t1, t4);
            Assert.AreNotEqual<double>(t2, t3);
            Assert.AreNotEqual<double>(t2, t5);
            Assert.AreNotEqual<double>(t3, t5);
        }
    }
}
