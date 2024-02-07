using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans_and_Perceptron
{
    class FCM
    {


        private int CLUSTER;
        private int DATA;
        private int COORD;

        private double[,] x; // 데이터 포인트
        private double[,] U; // 소속도 행렬
        private double[,] U_old; // 이전 소속도 행렬
        private double[,] v; // 클러스터 중심

        private double m = 2.0; // 퍼지지수
        private double error = 0.001; // 종료 임계값

        public FCM(int cluster, int coord)
        {
            CLUSTER = cluster;
            COORD = coord;
        }

        public double[,] Learn(double[,] input)
        {
            DATA = input.GetLength(0);
            x = new double[DATA, COORD];
            U = new double[DATA, CLUSTER];
            U_old = new double[DATA, CLUSTER];
            v = new double[CLUSTER, COORD];
            double[] v_sum = new double[COORD];
            double u_sum = 0.0;
            double u_pow_m = 0.0;
            double ratio = 0.0;
            double sum = 0.0;

            // 입력 데이터 할당
            for (int i = 0; i < DATA; i++)
                for (int j = 0; j < COORD; j++)
                    x[i, j] = input[i, j];

            // 소속도 행렬 초기화
            Random rand = new Random();
            for (int i = 0; i < DATA; i++)
            {
                sum = 0;
                for (int j = 0; j < CLUSTER; j++)
                {
                    U[i, j] = rand.NextDouble();
                    sum += U[i, j];
                }

                for (int j = 0; j < CLUSTER; j++)
                {
                    U[i, j] /= sum;
                }
            }

            bool isOver;
            do
            {
                isOver = true;

                // 클러스터 중심 계산
                for (int c = 0; c < CLUSTER; c++)
                {
                    u_sum = 0.0;
                    Array.Clear(v_sum, 0, v_sum.Length); // v_sum 배열 초기화

                    for (int i = 0; i < DATA; i++)
                    {
                        u_pow_m = Math.Pow(U[i, c], m);
                        u_sum += u_pow_m;
                        for (int j = 0; j < COORD; j++)
                        {
                            v_sum[j] += u_pow_m * x[i, j];
                        }
                    }

                    for (int j = 0; j < COORD; j++)
                    {
                        v[c, j] = v_sum[j] / u_sum;
                    }
                }

                // 소속도 업데이트
                for (int i = 0; i < DATA; i++)
                {
                    for (int c = 0; c < CLUSTER; c++)
                    {
                        sum = 0.0;
                        for (int k = 0; k < CLUSTER; k++)
                        {
                            ratio = Distance(x, i, v, c) / Distance(x, i, v, k);
                            sum += Math.Pow(ratio, 2 / (m - 1));
                        }
                        U_old[i, c] = U[i, c];
                        U[i, c] = 1.0 / sum;
                    }
                }

                // 종료 조건 체크
                for (int i = 0; i < DATA; i++)
                {
                    for (int c = 0; c < CLUSTER; c++)
                    {
                        if (Math.Abs(U[i, c] - U_old[i, c]) > error)
                        {
                            isOver = false;
                            break;
                        }
                    }
                }
            } while (!isOver);

            return U;
        }

        private double Distance(double[,] x, int dataIndex, double[,] v, int clusterIndex)
        {
            double sum = 0.0;
            for (int j = 0; j < COORD; j++)
                sum += Math.Pow(x[dataIndex, j] - v[clusterIndex, j], 2);
            return Math.Sqrt(sum);
        }
    }

}
