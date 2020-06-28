using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateTranslate
{
    /// <summary>
    /// 三维点
    /// </summary>
    public class Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class Point2D
    {
        public Point2D()
        {

        }
        public Point2D(double x, double y, double h)
        {
            X = x;
            Y = y;
            H = h;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }
    }

    public class PointBLH
    {
        public PointBLH(double b, double l, double h)
        {
            B = b;
            L = l;
            H = h;
        }
        public double B { get; set; }
        public double L { get; set; }
        public double H { get; set; }
    }
}
