using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearMartrix
{
    public class Martrix
    {
        private double[,] _instance;

        /// <summary>
        /// 获取矩阵的行数
        /// </summary>
        public int RowCount
        {
            get => _instance.GetLength(0);
        }

        /// <summary>
        /// 获取矩阵的列数
        /// </summary>
        public int ColCount
        {
            get => _instance.GetLength(1);
        }

        public Martrix(double[,] xy)
        {
            _instance = xy;
        }
        public Martrix(int row, int col)
        {
            _instance = new double[row, col];
        }

        public double this[int row, int col]
        {
            get
            {
                if (row < RowCount && row >= 0
                    && col < ColCount && col >= 0)
                {
                    return _instance[row, col];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if (row < RowCount && row >= 0
                    && col < ColCount && col >= 0)
                {
                    _instance[row, col] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public static Martrix operator +(Martrix m1, Martrix m2)
        {
            int maxRow = m1.RowCount > m2.RowCount ? m1.RowCount : m2.RowCount;
            int maxCol = m1.ColCount > m2.ColCount ? m1.ColCount : m2.ColCount;

            Martrix newM = new Martrix(maxRow, maxCol);
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    newM[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return newM;
        }

        public static Martrix operator -(Martrix m1, Martrix m2)
        {
            int maxRow = m1.RowCount > m2.RowCount ? m1.RowCount : m2.RowCount;
            int maxCol = m1.ColCount > m2.ColCount ? m1.ColCount : m2.ColCount;

            Martrix newM = new Martrix(maxRow, maxCol);
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    newM[i, j] = m1[i, j] - m2[i, j];
                }
            }
            return newM;
        }

        public static Martrix operator *(Martrix m1, double x)
        {
            int maxRow = m1.RowCount;
            int maxCol = m1.ColCount;

            Martrix newM = new Martrix(maxRow, maxCol);
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    newM[i, j] = m1[i, j] * x;
                }
            }
            return newM;
        }

        public static Martrix operator /(Martrix m1, double x)
        {
            int maxRow = m1.RowCount;
            int maxCol = m1.ColCount;

            Martrix newM = new Martrix(maxRow, maxCol);
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    newM[i, j] = m1[i, j] / x;
                }
            }
            return newM;
        }

        /// <summary>
        /// 转置
        /// </summary>
        /// <returns></returns>
        public Martrix Transpose()
        {
            Martrix newM = new Martrix(this.ColCount, this.RowCount);
            for (int i = 0; i < ColCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    newM[ColCount, RowCount] = _instance[RowCount, ColCount];
                }
            }
            return newM;
        }

        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        public Martrix Multiply(Martrix m2)
        {
            int maxRow = this.RowCount;
            int maxCol = m2.ColCount;

            Martrix newM = new Martrix(maxRow, maxCol);
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    double temp = 0;
                    for (int m = 0; m < this.ColCount; m++)
                    {
                        for (int n = 0; n < m2.RowCount; n++)
                        {
                            temp += _instance[i, m] * m2[n, j];
                        }
                    }
                    newM[i, j] = temp;
                }
            }
            return newM;
        }

        /// <summary>
        /// 求逆
        /// </summary>
        /// <returns></returns>
        public Martrix Inverse()
        {
            var n = _instance;
            //判断是否可逆
            int m = n.GetLength(0);
            double[,] q = new double[m, m]; //求逆结果
            int i, j, k;//计数君
            double u, temp;//临时变量

            //初始单位阵
            for (i = 0; i < m; i++)
                for (j = 0; j <= m - 1; j++)
                    q[i, j] = (i == j) ? 1 : 0;

            /// 求左下
            ///
            for (i = 0; i <= m - 2; i++)
            {
                //提取该行的主对角线元素
                u = n[i, i];   //可能为0
                if (u == 0)  //为0 时，在下方搜索一行不为0的行并交换
                {
                    for (i = 0; i < m; i++)
                    {
                        k = i;
                        for (j = i + 1; j < m; j++)
                        {
                            if (n[j, i] != 0) //不为0的元素
                            {
                                k = j;
                                break;
                            }
                        }

                        if (k != i) //如果没有发生交换： 情况1 下方元素也全是0
                        {
                            for (j = 0; j < m; j++)
                            {
                                //行交换
                                temp = n[i, j];
                                n[i, j] = n[k, j];
                                n[k, j] = temp;
                                //伴随交换
                                temp = q[i, j];
                                q[i, j] = q[k, j];
                                q[k, j] = temp;
                            }
                        }
                        else //满足条件1 弹窗提示
                            throw new Exception("不可逆");

                    }
                }

                for (j = 0; j < m; j++)//该行除以主对角线元素的值 使主对角线元素为1  
                {
                    n[i, j] = n[i, j] / u;   //分母不为0
                    q[i, j] = q[i, j] / u;  //伴随矩阵
                }

                for (k = i + 1; k < m; k++)  //下方的每一行减去  该行的倍数
                {
                    u = n[k, i];   //下方的某一行的主对角线元素
                    for (j = 0; j < m; j++)
                    {
                        n[k, j] = n[k, j] - u * n[i, j];  //下方的每一行减去该行的倍数  使左下角矩阵化为0
                        q[k, j] = q[k, j] - u * q[i, j];  //左下伴随矩阵
                    }
                }
            }


            u = n[m - 1, m - 1];  //最后一行最后一个元素

            if (u == 0) //条件2 初步计算后最后一行全是0 在只上步骤中没有计算最后一行，所以可能会遗漏
                throw new Exception("不可逆");
            n[m - 1, m - 1] = 1;
            for (j = 0; j < m; j++)
            {
                q[m - 1, j] = q[m - 1, j] / u;
            }

            // 求右上
            for (i = m - 1; i >= 0; i--)
            {
                for (k = i - 1; k >= 0; k--)
                {
                    u = n[k, i];
                    for (j = 0; j < m; j++)
                    {
                        n[k, j] = n[k, j] - u * n[i, j];
                        q[k, j] = q[k, j] - u * q[i, j];
                    }
                }
            }
            return new Martrix(q);
        }
    }

}
