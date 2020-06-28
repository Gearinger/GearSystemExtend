using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoordinateTranslate
{
    public class CalculateFourParamenter
    {
        public CalculateFourParamenter(double x, double y, double r, double m)
        {
            X = x;
            Y = y;
            R = r;
            M = m;
        }
        public CalculateFourParamenter(List<Point2D> pointList, List<Point2D> resultPointList)
        {
            PointList = pointList;
            ResultPointList = resultPointList;
            Calculate();
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double M { get; set; }
        public double R { get; set; }
        public List<Point2D> PointList { get; set; }
        public List<Point2D> ResultPointList { get; set; }
        private void Calculate()
        {
            MatrixBuilder<double> M_Matrix = Matrix<double>.Build;

            //B矩阵
            Matrix<double> B = M_Matrix.Dense(PointList.Count * 2, 4, 0);
            //L矩阵
            Matrix<double> L = M_Matrix.Dense(PointList.Count * 2, 2, 0);
            //X矩阵
            Matrix<double> X = M_Matrix.Dense(4, 1);
            //P矩阵
            double P = M_Matrix.One;

            for (int i = 0; i < PointList.Count; i++)
            {
                B[i * 2, 0] = 1;
                B[i * 2, 1] = 0;
                B[i * 2, 2] = -PointList[i].Y;
                B[i * 2, 3] = PointList[i].X;

                B[i * 2 + 1, 0] = 0;
                B[i * 2 + 1, 1] = 1;
                B[i * 2 + 1, 2] = PointList[i].X;
                B[i * 2 + 1, 3] = PointList[i].Y;


                L[i * 2, 0] = ResultPointList[i].X;
                L[i * 2, 1] = -PointList[i].X;
                L[i * 2 + 1, 0] = ResultPointList[i].Y;
                L[i * 2 + 1, 1] = -PointList[i].Y;
            }

            X = (B.Transpose() * P * B).Inverse() * (B.Transpose()) * P * L;

            this.X = X[0, 0];
            Y = X[1, 0];
            R = Math.Atan(X[2, 0] / X[3, 0]);
            M = Math.Sqrt(X[2, 0] * X[2, 0] + X[3, 0] * X[3, 0]) - 1;
        }

        public void SwapXYCalculate()
        {
            PointList = PointList.Select(p => new Point2D(p.Y, p.X, p.H)).ToList();
            ResultPointList = ResultPointList.Select(p => new Point2D(p.Y, p.X, p.H)).ToList();
            Calculate();
        }
    }
}
