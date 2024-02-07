using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans_and_Perceptron
{
    class Kmeans
    {
        private int CLUSTER; // 클러스터의 개수
        private int DATA; // 데이터 포인트의 개수
        private int COORD; // 좌표의 개수 (예: x, y)

        private double[,] x; // 데이터 포인트
        private double[,] d; // 각 데이터와 클러스터 중심 사이의 거리
        private double[,] v; // 클러스터 중심
        private double[,] v_OLD; // 이전 반복에서의 클러스터 중심
        private double[,] U; // 데이터 포인트가 속한 클러스터

        public Kmeans(int cluster, int coord)
        {
            this.CLUSTER = cluster;
            this.COORD = coord;
        }
        public double[,] Learn(double[,] input)
        {
            DATA = input.GetLength(0);
            x = new double[DATA, COORD];
            d = new double[DATA, CLUSTER];
            v = new double[CLUSTER, COORD];
            v_OLD = new double[CLUSTER, COORD];
            U = new double[DATA, CLUSTER];

            int count, v_count;
            double min, sumx, sumy;

            // 입력된 데이터를 x 배열에 할당
            for (int i = 0; i < DATA; i++)
            {
                for (int j = 0; j < COORD; j++)
                {
                    x[i, j] = input[i, j];
                }
            }

            // U 초기화
            for (int i = 0; i < DATA; i++)
            {
                for (int j = 0; j < CLUSTER; j++)
                {
                    U[i, j] = 0;
                }
            }

            // 초기 U 설정
            for (int i = 0; i < DATA; i++)
            {
                U[i, i % CLUSTER] = 1;
            }


            // 초기 클러스터 중심 계산
            for (int i = 0; i < CLUSTER; i++)
            {
                sumx = sumy = 0;
                count = 0;
                for (int j = 0; j < DATA; j++)
                {
                    if (U[j, i] == 1)
                    {
                        sumx += x[j, 0];
                        sumy += x[j, 1];
                        count++;
                    }
                }
                v[i, 0] = sumx / count;
                v[i, 1] = sumy / count;
            }

            // K-means 반복 실행
            do
            {
                // U 초기화
                for (int i = 0; i < DATA; i++)
                {
                    for (int j = 0; j < CLUSTER; j++)
                    {
                        U[i, j] = 0;
                    }
                }

                v_count = 0;

                // 각 데이터 포인트에 대해 가장 가까운 클러스터 중심 찾기
                for (int i = 0; i < DATA; i++)
                {
                    min = double.MaxValue;
                    count = 0;
                    for (int j = 0; j < CLUSTER; j++)
                    {
                        d[i, j] = Math.Sqrt(Math.Pow((v[j, 0] - x[i, 0]), 2) + Math.Pow((v[j, 1] - x[i, 1]), 2));
                        if (d[i, j] < min)
                        {
                            min = d[i, j];
                            count = j;
                        }
                    }
                    U[i, count] = 1;
                }

                // 이전 클러스터 중심 저장
                Array.Copy(v, v_OLD, v.Length);

                // 새로운 클러스터 중심 계산
                for (int i = 0; i < CLUSTER; i++)
                {
                    sumx = sumy = 0;
                    count = 0;
                    for (int k = 0; k < DATA; k++)
                    {
                        if (U[k, i] == 1)
                        {
                            sumx += x[k, 0];
                            sumy += x[k, 1];
                            count++;
                        }
                    }
                    v[i, 0] = count > 0 ? sumx / count : 0;
                    v[i, 1] = count > 0 ? sumy / count : 0;
                }

                // 클러스터 중심의 변화 체크
                for (int i = 0; i < CLUSTER; i++)
                {
                    for (int j = 0; j < COORD; j++)
                    {
                        if (v_OLD[i, j] != v[i, j])
                            v_count++;
                    }
                }
            } while (v_count != 0);

            // 속하는지 안 하는지
            return U;
        }
    }
}
