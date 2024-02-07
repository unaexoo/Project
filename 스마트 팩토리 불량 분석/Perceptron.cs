using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans_and_Perceptron
{
    class Perceptron
    {
        public double[] weight;
        public double lr; // Learning rate

        public Perceptron(int input, double lr = 0.1)
        {
            weight = new double[input + 1]; // +1 for bias
            this.lr = lr;
        }

        public double[] Learning(double[,] input, int[] target, double learningRate, int maxEpochs)
        {
            int pattern_cnt = input.GetLength(0); // 행 개수
            int feature_cnt = input.GetLength(1); // 열 개수
            double[] weight = new double[feature_cnt + 1]; // +1 for bias
            bool flag;
            int epoch = 0;

            // 가중치 초기화
            Random rand = new Random();
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = rand.NextDouble() * 2 - 1; // -1과 1 사이의 값
            }

            //0으로 초기화
            //for (int i = 0; i < weight.Length; i++)
            //{
            //    weight[i] = 0;
            //}

            //// 또는 작은 난수로 초기화
            //Random rand = new Random();
            //for (int i = 0; i < weight.Length; i++)
            //{
            //    weight[i] = rand.NextDouble() * 0.02 - 0.01; // -0.005와 0.005 사이의 값
            //}

            double total_err = 0.0;
            double net = 0.0;
            double err = 0.0;
            do
            {
                flag = false;
                total_err = 0; // 총 오차 초기화
                for (int i = 0; i < pattern_cnt; i++)
                {
                    net = weight[0]; // bias
                    for (int j = 0; j < feature_cnt; j++)
                    {
                        net += input[i, j] * weight[j + 1];
                    }

                    int y = net >= 0 ? 1 : 0;

                    if (y != target[i])
                    {
                        err = target[i] - y;
                        total_err += err * err; // 오차 제곱 추가
                        for (int k = 0; k < feature_cnt; k++)
                        {
                            weight[k + 1] = weight[k + 1] + learningRate * err * input[i, k];
                        }
                        weight[0] += learningRate * err; // bias 업데이트
                        flag = true;
                    }
                }
                double mse = total_err / pattern_cnt; // 평균 제곱 오차 계산
                Console.WriteLine($"Epoch {epoch}: loss = {mse}"); // 에포크별 로스 출력
                epoch++;
            } while (flag && epoch < maxEpochs);

            return weight;
        }

        public static int Predict(double[] input, double[] weight)
        {
            double net = weight[0]; // bias
            for (int i = 0; i < input.Length; i++)
            {
                net += input[i] * weight[i + 1];
            }
            return net >= 0 ? 1 : 0;
        }
    }
}
