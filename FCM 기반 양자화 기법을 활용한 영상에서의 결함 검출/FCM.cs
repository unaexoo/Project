using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ceramic2
{
    struct Color3D
    {
        public double R;
        public double G;
        public double B;
    }

    class FCM
    {
        Ceramic ceramic = new Ceramic();
        Bitmap origin;
        Color3D[,] img_arr;

        double[,] U;             // 초기 소속 행렬
        double[,] U_old;

        int countOfClass; // 클러스터중심
        Color3D[] V;            // centerOfClass

        double m; // 지수의 가중치 2
        double error; // 임계값

        public FCM(Bitmap origin, int countOfClass) // FCM 초기화
        {
            this.origin = origin;
            img_arr = ImageToArray(origin);

            this.countOfClass = countOfClass;
            m = 2;
            error = 0.01;

            U = new double[countOfClass, origin.Width * origin.Height];
            U_old = new double[countOfClass, origin.Width * origin.Height];
            V = new Color3D[countOfClass];

            Random rand = new Random();
            for (int i = 0; i < origin.Width * origin.Height; i++) // 소속 초기행렬
            {
                int numOfClass = rand.Next(countOfClass);
                for (int c = 0; c < countOfClass; c++)
                {
                    if (c == numOfClass)
                    {
                        U[c, i] = 1;
                        U_old[c, i] = 1;
                    }
                    else
                    {
                        U[c, i] = 0;
                        U_old[c, i] = 0;
                    }
                }
            }
        }

        public Image Quantize() // 양자화 및 fcm 학습
        {
            int countOfpix = origin.Width * origin.Height;
            bool isOver;

            do
            {
                isOver = true;

                // 클래스 중심 벡터 계산
                for (int c = 0; c < countOfClass; c++)
                {
                    double v_R = 0.0, v_G = 0.0, v_B = 0.0;
                    double v_bot = 0.0;

                    for (int px = 0; px < countOfpix; px++)
                    {
                        int row = (int)(px / (double)origin.Width);
                        int col = px % origin.Width;
                        double U_pow_m = Math.Pow(U[c, px], m);
                        v_R += U_pow_m * img_arr[row, col].R;
                        v_G += U_pow_m * img_arr[row, col].G;
                        v_B += U_pow_m * img_arr[row, col].B;
                        v_bot += U_pow_m;
                    }

                    V[c].R = v_R / v_bot;
                    V[c].G = v_G / v_bot;
                    V[c].B = v_B / v_bot;
                }


                // 중심 벡터 거리 계산
                double[,] D = new double[countOfClass, countOfpix];
                for (int c = 0; c < countOfClass; c++)
                {
                    for (int px = 0; px < countOfpix; px++)
                    {
                        int row = (int)(px / (double)origin.Width);
                        int col = px % origin.Width;

                        D[c, px] = Math.Sqrt(Math.Pow(img_arr[row, col].R - V[c].R, 2.0 / (m - 1)) +
                            Math.Pow(img_arr[row, col].G - V[c].G, 2.0 / (m - 1)) +
                            Math.Pow(img_arr[row, col].B - V[c].B, 2.0 / (m - 1)));
                    }
                }

                // 새로운 소속 행렬
                for (int c = 0; c < countOfClass; c++)
                {
                    for (int px = 0; px < countOfpix; px++)
                    {
                        U_old[c, px] = U[c, px];
                    }
                }

                for (int c = 0; c < countOfClass; c++)
                {
                    for (int px = 0; px < countOfpix; px++)
                    {
                        double new_u = 0;

                        for (int other_c = 0; other_c < countOfClass; other_c++)
                        {
                            try
                            {
                                new_u += Math.Pow(D[c, px] / D[other_c, px], 2 / (m - 1));
                            }
                            catch (DivideByZeroException) // 0으로 나눠지지 않을 경우
                            {
                                new_u = 0;
                                break;
                            }
                        }

                        U[c, px] = Math.Pow(new_u, -1);
                    }
                }

                // 최대 오류 확인
                for (int c = 0; c < countOfClass; c++)
                {
                    for (int px = 0; px < countOfpix; px++)
                    {
                        if (Math.Abs(U[c, px] - U_old[c, px]) > error)
                        {
                            isOver = false;
                            break;
                        }
                    }

                    if (!isOver) break;
                }

            } while (!isOver); // 최대 오류 미만이면 종료

            return GetCenterClassMap();
        }

        public Bitmap GetCenterClassMap()
        {
            // 소속값이 최대인 경우 해당 class 
            Color3D[,] out_arr = new Color3D[origin.Height, origin.Width];
            for (int col = 0; col < origin.Width; col++)
            {
                for (int row = 0; row < origin.Height; row++)
                {
                    int px = row * origin.Width + col;
                    double max_u = 0;
                    int classNum = 0;

                    for (int c = 0; c < countOfClass; c++)
                    {
                        if (U[c, px] > max_u)
                        {
                            max_u = U[c, px];
                            classNum = c;
                        }
                    }

                    out_arr[row, col].R = V[classNum].R;
                    out_arr[row, col].G = V[classNum].G;
                    out_arr[row, col].B = V[classNum].B;
                }
            }
            origin = ceramic.MedianFilter(origin);
            return ArrayToImage(out_arr, origin.Width, origin.Height);
        }
        
        public Bitmap GetColorClassMap()
        {
            Color3D[] JetLUT = new Color3D[countOfClass];
            double min_val = 255.0;
            double max_val = 0;

            for (int c = 0; c < countOfClass; c++)
            {
                double gray = 0.299 * V[c].R + 0.587 * V[c].G + 0.114 * V[c].B;
                if (min_val > gray) min_val = gray;
                if (max_val < gray) max_val = gray;
            }

            for (int c = 0; c < countOfClass; c++)
            {
                double gray = 0.299 * V[c].R + 0.587 * V[c].G + 0.114 * V[c].B;
                double ratio = 2*(gray - min_val) / (max_val - min_val);
                JetLUT[c].B = Math.Max(0, 255 * (1 - ratio));
                JetLUT[c].R = Math.Max(0, 255 * (ratio - 1));
                JetLUT[c].G = 255 - JetLUT[c].B - JetLUT[c].R;
            }

            Color3D[,] outArray = new Color3D[origin.Height, origin.Width];
            for (int col = 0; col < origin.Width; col++)
            {
                for (int row = 0; row < origin.Height; row++)
                {
                    int px = row * origin.Width + col;
                    double max_u = 0;
                    int classNum = 0;

                    for (int c = 0; c < countOfClass; c++)
                    {
                        if (U[c, px] > max_u)
                        {
                            max_u = U[c, px];
                            classNum = c;
                        }
                    }

                    outArray[row, col].R = JetLUT[classNum].R;
                    outArray[row, col].G = JetLUT[classNum].G;
                    outArray[row, col].B = JetLUT[classNum].B;
                }
            }
            return ArrayToImage3D(outArray, origin.Width, origin.Height);
        }



        private Color3D[,] ImageToArray(Bitmap bitmap)
        {
            Color3D[,] img_arr = new Color3D[bitmap.Height, bitmap.Width];

            for (int row = 0; row < bitmap.Height; row++)
            {
                for (int col = 0; col < bitmap.Width; col++)
                {
                    Color c = bitmap.GetPixel(col, row);
                    img_arr[row, col].R = c.R;
                    img_arr[row, col].G = c.G;
                    img_arr[row, col].B = c.B;
                }
            }

            return img_arr;
        }

        private Bitmap ArrayToImage(Color3D[,] img_arr, int width, int height)
        {
            Bitmap bitmap = origin;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Color color = Color.FromArgb((Byte)img_arr[row, col].R,
                                            (Byte)img_arr[row, col].G,
                                            (Byte)img_arr[row, col].B);

                    bitmap.SetPixel(col, row, color);
                }
            }
            Bitmap res_bitmap = ceramic.MedianFilter(bitmap);
            return res_bitmap;
        }
        private Bitmap ArrayToImage3D(Color3D[,] img_arr, int width, int height)
        {
            Bitmap bitmap = origin;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Color color = Color.FromArgb((Byte)img_arr[row, col].R,
                                            (Byte)img_arr[row, col].G,
                                            (Byte)img_arr[row, col].B);

                    bitmap.SetPixel(col, row, color);
                }
            }
            Bitmap res_bitmap = ceramic.MedianFilter3D(bitmap);
            return res_bitmap;
        }
    }
}
