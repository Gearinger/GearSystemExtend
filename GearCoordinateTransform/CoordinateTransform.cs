using MathNet.Numerics.LinearAlgebra;
using System;

namespace CoordinateTranslate
{
    class CoordinateTransform
    {
    }

    /// <summary>
    /// 三维七参转换模型
    /// </summary>
    public static class CoordinateTransformSevenPara
    {
        /// <summary>
        /// 七参数转换（原坐标系参心空间坐标==>目标坐标系参心空间坐标）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="sevenPara">七参数（位移量单位为米，角度单位为弧度</param>
        /// <returns></returns>
        public static Point3D TransformCoord(double x, double y, double z, CalculateSevenParamenter sevenPara)
        {
            //转换过程
            MatrixBuilder<double> M_Matrix = Matrix<double>.Build;

            Matrix<double> A_Matrix = M_Matrix.Dense(7, 1);//七参数
            Matrix<double> X_Matrix = M_Matrix.Dense(3, 7, 0);
            Matrix<double> Y_Matrix = M_Matrix.Dense(3, 1, 0);

            #region 初始化X矩阵
            X_Matrix[0, 0] = 1;
            X_Matrix[0, 3] = x;
            X_Matrix[0, 5] = -z;
            X_Matrix[0, 6] = y;

            X_Matrix[1, 1] = 1;
            X_Matrix[1, 3] = y;
            X_Matrix[1, 4] = z;
            X_Matrix[1, 6] = -x;

            X_Matrix[2, 2] = 1;
            X_Matrix[2, 3] = z;
            X_Matrix[2, 4] = -y;
            X_Matrix[2, 5] = x;
            #endregion

            #region 初始化A矩阵
            A_Matrix[0, 0] = sevenPara.X;
            A_Matrix[1, 0] = sevenPara.Y;
            A_Matrix[2, 0] = sevenPara.Z;
            A_Matrix[3, 0] = sevenPara.M + 1;
            A_Matrix[4, 0] = sevenPara.Wx * (sevenPara.M + 1);
            A_Matrix[5, 0] = sevenPara.Wy * (sevenPara.M + 1);
            A_Matrix[6, 0] = sevenPara.Wz * (sevenPara.M + 1);
            #endregion

            Y_Matrix = X_Matrix * A_Matrix;

            Y_Matrix[0, 0] = sevenPara.X + (sevenPara.M + 1) * x + 0 - (sevenPara.M + 1) * sevenPara.Wy * z + (sevenPara.M + 1) * sevenPara.Wz * y;
            Y_Matrix[1, 0] = sevenPara.Y + (sevenPara.M + 1) * y + (sevenPara.M + 1) * sevenPara.Wx * z + 0 - (sevenPara.M + 1) * sevenPara.Wz * x;
            Y_Matrix[2, 0] = sevenPara.Z + (sevenPara.M + 1) * z - (sevenPara.M + 1) * sevenPara.Wx * y + (sevenPara.M + 1) * sevenPara.Wy * x + 0;

            return new Point3D(Y_Matrix[0, 0], Y_Matrix[1, 0], Y_Matrix[2, 0]);
        }

        /// <summary>
        /// 七参数转换（原坐标系参心空间坐标==>目标坐标系参心空间坐标）理论精度比TransformCoord高
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="sevenPara"></param>
        /// <returns></returns>
        public static Point3D TransformCoord2(double x, double y, double z, CalculateSevenParamenter sevenPara)
        {
            //转换过程
            MatrixBuilder<double> M_Matrix = Matrix<double>.Build;

            Matrix<double> Delta_Matrix = M_Matrix.Dense(3, 1);//XYZ位移量
            Matrix<double> Alpha_Matrix = M_Matrix.Dense(3, 3, 0);
            Matrix<double> Beta_Matrix = M_Matrix.Dense(3, 3, 0);
            Matrix<double> Gama_Matrix = M_Matrix.Dense(3, 3, 0);

            Matrix<double> Coord1_Matrix = M_Matrix.Dense(3, 1, 0);
            Matrix<double> Coord2_Matrix = M_Matrix.Dense(3, 1, 0);

            Coord1_Matrix[0, 0] = x;
            Coord1_Matrix[1, 0] = y;
            Coord1_Matrix[2, 0] = z;

            Delta_Matrix[0, 0] = sevenPara.X;
            Delta_Matrix[1, 0] = sevenPara.Y;
            Delta_Matrix[2, 0] = sevenPara.Z;

            Alpha_Matrix[0, 0] = 1;
            Alpha_Matrix[1, 1] = Math.Cos(sevenPara.Wx);
            Alpha_Matrix[1, 2] = Math.Sin(sevenPara.Wx);
            Alpha_Matrix[2, 1] = -Math.Sin(sevenPara.Wx);
            Alpha_Matrix[2, 2] = Math.Cos(sevenPara.Wx);

            Beta_Matrix[0, 0] = Math.Cos(sevenPara.Wy);
            Beta_Matrix[0, 2] = -Math.Sin(sevenPara.Wy);
            Beta_Matrix[1, 1] = 1;
            Beta_Matrix[2, 0] = Math.Sin(sevenPara.Wy);
            Beta_Matrix[2, 2] = Math.Cos(sevenPara.Wy);

            Gama_Matrix[0, 0] = Math.Cos(sevenPara.Wz);
            Gama_Matrix[0, 1] = Math.Sin(sevenPara.Wz);
            Gama_Matrix[1, 0] = -Math.Sin(sevenPara.Wz);
            Gama_Matrix[1, 1] = Math.Cos(sevenPara.Wz);
            Gama_Matrix[2, 2] = 1;

            Coord2_Matrix = Delta_Matrix + (1 + sevenPara.M) * Alpha_Matrix * Beta_Matrix * Gama_Matrix * Coord1_Matrix;

            return new Point3D(Coord2_Matrix[0, 0], Coord2_Matrix[1, 0], Coord2_Matrix[2, 0]);
        }

    }

    /// <summary>
    /// 二维四参转换模型
    /// </summary>
    public static class CoordinateTransformFourPara
    {
        /// <summary>
        /// 二维四参转换（原坐标系平面坐标==>目标坐标系平面坐标, 高程不变）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fourPara">四参数，位移单位为米，角度单位为弧度</param>
        /// <returns></returns>
        public static Point2D TransformCoord(double x, double y, CalculateFourParamenter fourPara)
        {
            Point2D resultPoint = new Point2D
            {
                X = fourPara.X + (1 + fourPara.M) * (Math.Cos(fourPara.R) * x - Math.Sin(fourPara.R) * y),
                Y = fourPara.Y + (1 + fourPara.M) * (Math.Sin(fourPara.R) * x + Math.Cos(fourPara.R) * y)
            };
            return resultPoint;
        }
    }
}
