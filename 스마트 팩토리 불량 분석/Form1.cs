using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Kmeans_and_Perceptron
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int[] target;
        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = "combine_data_shuffle_no.csv";
            double[,] data = Load_data(filePath);
            int cluster = 3;
            target = Make_target(filePath);

            Perceptron perceptron = new Perceptron(data.GetLength(1) - 1);
            double[] weights = perceptron.Learning(data, target, perceptron.lr, 10000);

            int[] predictions = perceptron_perdict(data, weights);
 
            Analysis(predictions, target, "퍼셉트론", textBox3);
            clsuter_res(data, cluster);
        }
        private void clsuter_res(double[,] data, int clusterCount)
        {
            Kmeasn_data(data, clusterCount, target, textBox1);  // K-means 결과를 TextBox1에 출력
            fCM_data(data, clusterCount, target, textBox4);
        }

        private void Kmeasn_data(double[,] data, int cluster, int[] target, TextBox textBox)
        {
            Kmeans kmeans = new Kmeans(cluster, data.GetLength(1) - 1);
            double[,] clusteringResult = kmeans.Learn(data);
            double[,] pattern = Make_pattern(clusteringResult, cluster);

            Perceptron_learning(pattern, target, "K-means", textBox);
        }
        private void fCM_data(double[,] data, int cluster, int[] target, TextBox textBox)
        {
            FCM fCM = new FCM(cluster, data.GetLength(1) - 1);
            double[,] clusteringResult = fCM.Learn(data);
            double[,] pattern = Make_pattern(clusteringResult, cluster);

            Perceptron_learning(pattern, target, "fcm", textBox);
        }

        public void Print_pattern(double[,] pattern, int cluster, TextBox textBox)
        {
            StringBuilder stringBuilder = new StringBuilder();
            // 클러스터 패턴 출력
            stringBuilder.AppendLine("클러스터 패턴 : \n");

            for (int i = 0; i < pattern.GetLength(0); i++)
            {
                stringBuilder.Append($"데이터 {i}: ");
                for (int j = 0; j < cluster; j++)
                {
                    stringBuilder.Append( $"{pattern[i, j]} , ");
                }

                stringBuilder.AppendLine("\n"); // 각 데이터 패턴 후에 줄 바꿈을 추가합니다.
            }
            textBox.Text += stringBuilder.ToString();
        }
        public double[,] Load_data(string file)
        {
            string[] lines = File.ReadAllLines(file);
            double[,] data = new double[lines.Length, lines[0].Split(',').Length - 1];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                for (int j = 0; j < values.Length - 1; j++)
                {
                    if (double.TryParse(values[j], out double parsedValue))
                    {
                        data[i, j] = parsedValue;
                    }
                    else
                    {
                        Console.WriteLine($"Data parsing error at line {i + 1}, column {j + 1}");
                        data[i, j] = 0; 
                    }
                }
            }
            return data; ;
        }
        public int[] Make_target(string file)
        {
            string[] lines = File.ReadAllLines(file);
            int[] target = new int[lines.Length]; // 각 행에 2개의 열이 있는 2차원 배열

            for (int i = 0; i < lines.Length; i++)
            {
                string last_value = lines[i].Split(',').Last().Trim();
                if (last_value == "Y")
                {
                    target[i] = 1; // 'Y'인 경우 첫 번째 열은 1

                }
                else
                {
                    target[i] = 0; // 'Y'가 아닌 경우 첫 번째 열은 0

                }
            }

            return target;
        }

        private double[,] Make_pattern(double[,] cluster_res, int cluster_cnt)
        {
            double[,] pattern = new double[cluster_res.GetLength(0), cluster_cnt];
            for (int i = 0; i < cluster_res.GetLength(0); i++)
            {
                for (int j = 0; j < cluster_cnt; j++)
                {
                    pattern[i, j] = cluster_res[i, j];
                }
            }
            return pattern;
        }

        private void Perceptron_learning(double[,] pattern, int[] target, string methodName, TextBox textBox)
        {
            Perceptron perceptron = new Perceptron(pattern.GetLength(1));
            double[] weights = perceptron.Learning(pattern, target, perceptron.lr, 10000);

            int[] predictions = perceptron_perdict(pattern, weights);
            Analysis(predictions, target, methodName, textBox);
        }
        private int[] perceptron_perdict(double[,] pattern, double[] weights)
        {
            int[] predictions = new int[pattern.GetLength(0)];
            for (int i = 0; i < pattern.GetLength(0); i++)
            {
                double[] input = new double[pattern.GetLength(1)];
                for (int j = 0; j < input.Length; j++)
                {
                    input[j] = pattern[i, j];
                }
                predictions[i] = Perceptron.Predict(input, weights);
            }
            return predictions;
        }
        public void data_res(int[] predictions, int[] target, string method, TextBox textbox)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"==== {method} 결과 ====");

            int trueCount = 0, falseCount = 0;
            for (int i = 0; i < predictions.Length; i++)
            {
                string actual = target[i] == 1 ? "불량" : "정상";
                string predicted = predictions[i] == 1 ? "불량" : "정상";
                string result = predictions[i] == target[i] ? "정확" : "틀림";

                stringBuilder.AppendLine($"데이터 {i}: 실제 {actual} | 예측 {predicted} ({result})");

                if (predictions[i] == target[i])
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("=========== 데이터 결과 ===========");
            stringBuilder.AppendLine($"전체 데이터 : {predictions.Length}");
            stringBuilder.AppendLine($"정확: {trueCount}");
            stringBuilder.AppendLine($"틀림: {falseCount}");

            textbox.Text += stringBuilder.ToString();
        }

        public void Analysis(int[] predict, int[] target, string method, TextBox textBox)
        {
            data_res(predict, target, method, textBox);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"==== {method} 결과 ====");
            int TP = 0, FN = 0, TN = 0, FP = 0;
            for (int i = 0; i < predict.GetLength(0); i++)
            {
                if (predict[i] == 1 && target[i] == 1)
                    TP++; // True Positive
                else if (predict[i] == 0 && target[i] == 1)
                    FN++; // False Negative
                else if (predict[i] == 0 && target[i] == 0)
                    TN++; // True Negative
                else if (predict[i] == 1 && target[i] == 0)
                    FP++; // False Positive
            }

            double accuracy = (TP + TN) / (double)(TP + TN + FP + FN);
            double sensitivity = (TP + FN) != 0 ? TP / (double)(TP + FN) : 0;
            double precision = (TP + FP) != 0 ? TP / (double)(TP + FP) : 0;
            double F1Score = (precision + sensitivity) != 0 ? 2 * (precision * sensitivity) / (precision + sensitivity) : 0;


            stringBuilder.AppendLine($"정확도: {accuracy:P2}\n");
            stringBuilder.AppendLine ($"민감도: {sensitivity:P2}\n");
            stringBuilder.AppendLine ($"정밀도: {precision:P2} \n");
            stringBuilder.AppendLine ($"F1 Score: {F1Score:P2} \n");

            stringBuilder.AppendLine ($"불량일 때 불량: {((TP + FN) != 0 ? (TP / (double)(TP + FN)).ToString("P2") : "N/A")}\n");
            stringBuilder.AppendLine ($"불량인데 정상: {((TP + FN) != 0 ? (FN / (double)(TP + FN)).ToString("P2") : "N/A")} \n");
            stringBuilder.AppendLine($"정상인데 정상: {((TN + FP) != 0 ? (TN / (double)(TN + FP)).ToString("P2") : "N/A")}  \n");
            stringBuilder.AppendLine ($"정상인데 불량: {((TN + FP) != 0 ? (FP / (double)(TN + FP)).ToString("P2") : "N/A")}  \n");
            stringBuilder.AppendLine();
            
            textBox2.Text += stringBuilder.ToString();

        }
    }
}
