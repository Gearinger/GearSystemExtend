using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace CoordinateTranslate
{
    /// <summary>
    /// 传入控制点对，获取七参数
    /// </summary>
    public class CalculateSevenParamenter
    {
        public CalculateSevenParamenter(double x, double y, double z, double wx, double wy, double wz, double m)
        {
            X = x;
            Y = y;
            Z = z;
            Wx = wx;
            Wy = wy;
            Wz = wz;
            M = m;
        }
        public CalculateSevenParamenter(List<Point3D> pointList, List<Point3D> resultPointList)
        {
            PointList = pointList;
            ResultPointList = resultPointList;
            Calculate();
        }

        /// <summary>
        /// 米
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 米
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// 米
        /// </summary>
        public double Z { get; set; }
        /// <summary>
        /// 度
        /// </summary>
        public double Wx { get; set; }
        /// <summary>
        /// 度
        /// </summary>
        public double Wy { get; set; }
        /// <summary>
        /// 度
        /// </summary>
        public double Wz { get; set; }
        /// <summary>
        /// 尺度
        /// </summary>
        public double M { get; set; }
        public List<Point3D> PointList { get; set; }
        public List<Point3D> ResultPointList { get; set; }

        /// <summary>
        /// 根据控制点对PointList、ResultPointList计算七参数X、Y、Z、M、Wx、Wy、Wz
        /// </summary>
        private void Calculate()
        {
            MatrixBuilder<double> M_Matrix = Matrix<double>.Build;

            Matrix<double> A_Matrix = M_Matrix.Dense(7, 1);//七参数
            Matrix<double> X_Matrix = M_Matrix.Dense(PointList.Count * 3, 7, 0);
            Matrix<double> Y_Matrix = M_Matrix.Dense(PointList.Count * 3, 1, 0);

            for (int i = 0; i < PointList.Count; i++)
            {
                X_Matrix[i * 3, 0] = 1;
                X_Matrix[i * 3, 3] = PointList[i].X;
                X_Matrix[i * 3, 5] = -PointList[i].Z;
                X_Matrix[i * 3, 6] = PointList[i].Y;

                X_Matrix[i * 3 + 1, 1] = 1;
                X_Matrix[i * 3 + 1, 3] = PointList[i].Y;
                X_Matrix[i * 3 + 1, 4] = PointList[i].Z;
                X_Matrix[i * 3 + 1, 6] = -PointList[i].X;

                X_Matrix[i * 3 + 2, 2] = 1;
                X_Matrix[i * 3 + 2, 3] = PointList[i].Z;
                X_Matrix[i * 3 + 2, 4] = -PointList[i].Y;
                X_Matrix[i * 3 + 2, 5] = PointList[i].X;

            }

            for (int i = 0; i < ResultPointList.Count; i++)
            {
                Y_Matrix[i * 3, 0] = ResultPointList[i].X;
                Y_Matrix[i * 3 + 1, 0] = ResultPointList[i].Y;
                Y_Matrix[i * 3 + 2, 0] = ResultPointList[i].Z;
            }

            A_Matrix = (X_Matrix.Transpose() * X_Matrix).Inverse() * (X_Matrix.Transpose()) * Y_Matrix;

            X = A_Matrix[0, 0];
            Y = A_Matrix[1, 0];
            Z = A_Matrix[2, 0];
            M = A_Matrix[3, 0] - 1;
            Wx = A_Matrix[4, 0] / A_Matrix[3, 0];
            Wy = A_Matrix[5, 0] / A_Matrix[3, 0];
            Wz = A_Matrix[6, 0] / A_Matrix[3, 0];
            PointList.Clear();
            ResultPointList.Clear();
        }
    }

}
