using System;

namespace CoordinateTranslate
{
    /// <summary>
    /// 椭球
    /// </summary>
    class Ellipsoid
    {
        public Ellipsoid()
        {

        }
        /// <summary>
        /// 椭球
        /// </summary>
        /// <param name="a">长半轴</param>
        /// <param name="b">短半轴</param>
        public Ellipsoid(double a, double b)
        {
            this.a = a;
            this.b = b;
            e = Math.Sqrt(a * a - b * b) / a;
            e2 = e / (1 - e);
            f = (a - b) / a;
        }
        /// <summary>
        /// 长半轴
        /// </summary>
        public double a { get; set; }
        /// <summary>
        /// 短半轴
        /// </summary>
        public double b { get; set; }
        /// <summary>
        /// 第一偏心率
        /// </summary>
        public double e { get; set; }
        /// <summary>
        /// 第二偏心率
        /// </summary>
        public double e2 { get; set; }

        /// <summary>
        /// 扁率
        /// </summary>
        public double f { get; set; }

        /// <summary>
        /// 创建常用的标准椭球
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Ellipsoid Instance(Ellipsoids ellipsoidstype)
        {
            switch (ellipsoidstype)
            {
                case Ellipsoids.XiAn1980:
                    return new Ellipsoid(6378140.0, 6356755.288157528);
                case Ellipsoids.CGCS2000:
                    return new Ellipsoid(6378137, 6356752.31414035615);
                case Ellipsoids.BeiJing1954:
                    return new Ellipsoid(6378245.0, 6356863.018773047);
                case Ellipsoids.WGS1984:
                    return new Ellipsoid(6378137, 6356752.3142451793);
                default:
                    return new Ellipsoid(6378137, 6356752.31414035615);
            }
        }
    }

    /// <summary>
    /// 常用标准椭球枚举
    /// </summary>
    enum Ellipsoids
    {
        XiAn1980, CGCS2000, BeiJing1954, WGS1984
    }

    /// <summary>
    /// 平面坐标，大地坐标，参心空间坐标相互转换
    /// </summary>
    static class Common
    {
        /// <summary>
        /// 大地坐标转空间坐标
        /// </summary>
        /// <param name="B"></param>
        /// <param name="L"></param>
        /// <param name="H"></param>
        /// <param name="ellipsoid"></param>
        /// <returns></returns>
        public static Point3D TranslateBLHToXYZ(double B, double L, double H, Ellipsoids ellipsoid)
        {
            double p = 180 / Math.PI;
            B = B / p;
            L = L / p;
            double x = 0, y = 0, z = 0;
            Ellipsoid ellipsoidTemp = Ellipsoid.Instance(ellipsoid);
            double ee = ellipsoidTemp.e * ellipsoidTemp.e;
            double N = ellipsoidTemp.a / (Math.Sqrt(1 - (ee * Math.Sin(B) * Math.Sin(B))));
            x = (N + H) * Math.Cos(B) * Math.Cos(L);
            y = (N + H) * Math.Cos(B) * Math.Sin(L);
            z = (N * (1 - ee) + H) * Math.Sin(B);

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// 空间坐标转大地坐标
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="ellipsoid"></param>
        /// <returns></returns>
        public static PointBLH TranslateXYZToBLH(double X, double Y, double Z, Ellipsoids ellipsoid)
        {
            Ellipsoid ellipsoidTemp = Ellipsoid.Instance(ellipsoid);
            double ee = ellipsoidTemp.e * ellipsoidTemp.e;
            double p = 180 / Math.PI;
            double B, L, H;
            L = Math.Atan(Y / X);
            if (X < 0)
                L += Math.PI;
            double r = Math.Sqrt(X * X + Y * Y);
            double B1 = Math.Atan(Z / r);
            double B2;
            while (true)
            {
                double W1 = Math.Sqrt(1 - ee * (Math.Sin(B1) * Math.Sin(B1)));
                double N1 = ellipsoidTemp.a / W1;
                B2 = Math.Atan((Z + N1 * ee * Math.Sin(B1)) / r);
                if (Math.Abs(B2 - B1) <= 0.0000000001)
                    break;
                B1 = B2;
            }
            B = B2;
            double W = Math.Sqrt(1 - ee * (Math.Sin(B2) * Math.Sin(B2)));
            double N = ellipsoidTemp.a / W;
            H = r / Math.Cos(B2) - N;
            return new PointBLH(B * p, L * p, H);
        }


        /// <summary>
        /// 大地坐标转平面坐标，高斯正算(输出X值为横坐标，且不带带号)
        /// </summary>
        /// <param name="B">纬度</param>
        /// <param name="L">经度</param>
        /// <param name="ZoneWidth">带宽</param>
        /// <param name="longitude0">中央经度</param>
        /// <returns></returns>
        public static Point2D ProjectToXY(double B, double L, double H, int ZoneWidth, double longitude0, Ellipsoids ellipsoid)
        {
            Ellipsoid ellipsoidTemp = Ellipsoid.Instance(ellipsoid);//椭球

            double longitude1, latitude1;
            double p = 180.0 / Math.PI;
            longitude0 = longitude0 / p;

            // 转换为弧度  
            longitude1 = L / p;
            latitude1 = B / p;

            double e2 = 2 * ellipsoidTemp.f - ellipsoidTemp.f * ellipsoidTemp.f;
            double ee = e2 * (1.0 - e2);
            double NN = ellipsoidTemp.a / Math.Sqrt(1.0 - e2 * Math.Sin(latitude1) * Math.Sin(latitude1));
            double T = Math.Tan(latitude1) * Math.Tan(latitude1);
            double C = ee * Math.Cos(latitude1) * Math.Cos(latitude1);
            double A = (longitude1 - longitude0) * Math.Cos(latitude1);
            double M = ellipsoidTemp.a * ((1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256) * latitude1 - (3 * e2 / 8 + 3 * e2 * e2 / 32 + 45 * e2 * e2 * e2 / 1024) * Math.Sin(2 * latitude1) + (15 * e2 * e2 / 256 + 45 * e2 * e2 * e2 / 1024) * Math.Sin(4 * latitude1) - (35 * e2 * e2 * e2 / 3072) * Math.Sin(6 * latitude1));

            //计算x y坐标值
            double xval = NN * (A + (1 - T + C) * A * A * A / 6 + (5 - 18 * T + T * T + 72 * C - 58 * ee) * A * A * A * A * A / 120);
            double yval = M + NN * Math.Tan(latitude1) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24 + (61 - 58 * T + T * T + 600 * C - 330 * ee) * A * A * A * A * A * A / 720);
            //起算坐标值
            double X0 = 500000;
            double Y0 = 0;

            xval = xval + X0;
            yval = yval + Y0;
            return new Point2D(xval, yval, H);
        }

        /// <summary>
        /// 平面坐标转大地坐标，高斯反算(X值为横坐标，且不带带号)
        /// </summary>
        /// <param name="X">横坐标值（不带带号）</param>
        /// <param name="Y">纵坐标</param>
        /// <param name="ZoneWidth">带宽</param>
        /// <param name="longitude0">中央经度</param>
        /// <param name="ellipsoid">椭球</param>
        /// <returns></returns>
        public static PointBLH ProjectToBL(double X, double Y, double H, int ZoneWidth, double longitude0, Ellipsoids ellipsoid)
        {
            //对于有带号的坐标，去除带号(超过六位数，只取六位)
            if (X > 10000000)
            {
                X = X - X / 1000000 * 1000000;
            }

            Ellipsoid ellipsoidTemp = Ellipsoid.Instance(ellipsoid);//椭球
            double longitude1, latitude1, X0, Y0;
            double p = 180.0 / Math.PI; //弧度常数
            longitude0 = longitude0 / p; // 中央经线  

            //带内起算坐标
            X0 = 500000;
            Y0 = 0;
            //带内坐标值
            double xval = X - X0;
            double yval = Y - Y0;

            //相关参数
            double e2 = 2 * ellipsoidTemp.f - ellipsoidTemp.f * ellipsoidTemp.f;
            double e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
            double ee = e2 / (1 - e2);
            double M = yval;
            double u = M / (ellipsoidTemp.a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
            double bf = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * u) + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
            double C = ee * Math.Cos(bf) * Math.Cos(bf);
            double T = Math.Tan(bf) * Math.Tan(bf);
            double NN = ellipsoidTemp.a / Math.Sqrt(1.0 - e2 * Math.Sin(bf) * Math.Sin(bf));
            double R = ellipsoidTemp.a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(bf) * Math.Sin(bf)) * (1 - e2 * Math.Sin(bf) * Math.Sin(bf)) * (1 - e2 * Math.Sin(bf) * Math.Sin(bf)));
            double D = xval / NN;
            // 计算经度(Longitude) 纬度(Latitude)  
            longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D * D * D * D * D / 120) / Math.Cos(bf);
            latitude1 = bf - (NN * Math.Tan(bf) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24 + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);

            // 弧度转换为度 
            double L = longitude1 * p;
            double B = latitude1 * p;

            return new PointBLH(B, L, H);
        }
    }

}
