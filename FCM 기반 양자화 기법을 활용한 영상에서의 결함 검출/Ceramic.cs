using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ceramic2
{
    class Ceramic
    {
        public int height_pix;
        private const int HISTO_WIDTH = 256;
        private const int HISTO_HEIGHT = 240;
        public int control_type = 0;
        public Bitmap MedianFilter3D(Bitmap bitmap)
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,,] bitmap_arr = getbmp.GetColorBitmap(bitmap);

            int[,,] res_arr = new int[bitmap.Height, bitmap.Width, 3];

            for (int c = 0; c < 3; c++) // 0: R, 1: G, 2: B
            {
                int[,] channel = new int[bitmap.Height, bitmap.Width];
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        channel[y, x] = bitmap_arr[y, x, c];
                    }
                }

                int[,] filter_channel = Median_calc(channel, bitmap.Width, bitmap.Height, 5, 5);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        res_arr[y, x, c] = filter_channel[y, x];
                    }
                }
            }

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetColorBitmap(res_arr, bitmap);
        }
        public Bitmap MedianFilter(Bitmap bitmap) // 중간값 필터 이진 영상(fcm 후 중심벡터 영상에 적용)
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            int[,] res_arr = Median_calc(bitmap_arr, bitmap.Width, bitmap.Height, 5, 5);
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }
        int[,] Median_calc(int[,] G, int width, int height, int wcol, int wrow)
        {
            int[,] res_arr = new int[height, width];

            int xPad = wcol / 2;
            int yPad = wrow / 2;
            int idx = 0;

            for (int y = 0; y < height - 2 * yPad; y++)
            {
                for (int x = 0; x < width - 2 * xPad; x++)
                {
                    int[] values = new int[wcol * wrow];
                    idx = 0;

                    for (int u = 0; u < wrow; u++)
                    {
                        for (int v = 0; v < wcol; v++)
                        {
                            values[idx++] = G[y + u, x + v];
                        }
                    }

                    Array.Sort(values);
                    res_arr[y + yPad, x + xPad] = values[values.Length / 2];
                }
            }

            for (int y = 0; y < yPad; y++)
            {
                for (int x = xPad; x < width - xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (int x = 0; x < xPad; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    res_arr[y, x] = res_arr[y, xPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }
            return res_arr;
        }

        public Bitmap Laplacian(Bitmap bitmap)  // 라플라시안 마스크
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            double[,] mask1 = {{0.0 , 1.0 , 0.0},
                                {1.0 , -4.0 , 1.0},
                                {0.0 , 1.0 , 0.0}};

            int[,] res_arr = convolve(bitmap_arr, bitmap.Width, bitmap.Height, mask1, 3, 3, 0);

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }

        int[,] convolve(int[,] G, int width, int height, double[,] mask, int mcol, int mrow, int bias)
        {
            int[,] res_arr = new int[height, width];
            int xPad = mcol / 2;
            int yPad = mrow / 2;
            double sum = 0.0;

            for (int y = 0; y < height - 2 * yPad; y++)
            {
                for (int x = 0; x < width - 2 * xPad; x++)
                {
                    sum = 0.0;
                    for (int u = 0; u < mrow; u++)
                    {
                        for (int v = 0; v < mcol; v++)
                        {
                            sum += G[y + u, x + v] * mask[u, v];
                        }
                    }
                    sum += bias;

                    if (sum > 255.0) sum = 255.0;
                    if (sum < 0.0) sum = 0.0;

                    res_arr[y + yPad, x + xPad] = (int)sum;
                }
            }

            for (int y = 0; y < yPad; y++)
            {
                for (int x = 0; x < xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (int x = 0; x < xPad; x++)
            {
                for (int y = 0; y < yPad; y++)
                {
                    res_arr[y, x] = res_arr[y, yPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }

            return res_arr;
        }
        public Bitmap OtusBinary(Bitmap bitmap) // 오츠이진화
        {
            int width = bitmap.Width, height = bitmap.Height;

            int[] histo = new int[256];
            int T = 0;
            int[,] res_img = new int[height, width];
            int[,] gray_arr = new int[height, width];
            histo.Initialize();

            double Wb = 0.0, Wf = 0.0, Mb = 0.0, Mf = 0.0;
            int Sb = 0, Sf = 0;
            int Cb = 0, Cf = 0;
            double max_var = 0.0, variance = 0.0;

            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    histo[bitmap_arr[y, x]]++;
                }
            }

            for (int idx = 0; idx < 256; idx++)
            {
                Sf += idx * histo[idx];
            }

            Cb = histo[0];
            Cf = (height * width) - histo[0];

            for (int idx = 1; idx < 256; idx++)
            {
                Cb += histo[idx];
                Cf -= histo[idx];

                if (Cb == 0 || Cf == 0) continue;

                Sb += idx * histo[idx];
                Sf -= idx * histo[idx];

                Mb = (double)Sb / Cb;
                Mf = (double)Sf / Cf;

                Wb = (double)Cb / (height * width);
                Wf = (double)Cf / (height * width);

                variance = Wb * Wf * Math.Pow((Mb - Mf), 2.0);

                if (variance > max_var)
                {
                    T = idx;
                    max_var = variance;
                }

            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (bitmap_arr[y, x] > T)
                    {
                        bitmap_arr[y, x] = 0;
                    }
                    else
                    {
                        bitmap_arr[y, x] = 255;
                    }
                }
            }

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(bitmap_arr, bitmap);
        }

        public Bitmap SobelMask(Bitmap bitmap) // 소벨마스크
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            double[,] mask1 = {{-1.0 , 0.0 , 1.0},
                                {-2.0 , 0.0 , 2.0},
                                {-1.0 , 0.0 , 1.0}};
            double[,] mask2 = {{-1.0, -2.0, -1.0 },
                                { 0.0,  0.0,  0.0 },
                                { 1.0,  2.0,  1.0 }};

            int[,] res_arr = convolveXY(bitmap_arr, bitmap.Width, bitmap.Height, mask1, mask2, 3, 3);

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }

        public Bitmap PrewittMask(Bitmap bitmap) // 프리윗마스크
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);
            double[,] mask1 = { { -1.0, 0.0 , 1.0},
                                 {-1.0, 0.0,1.0 },
                                 {-1.0, 0.0, 1.0}};
            double[,] mask2 = { { 1.0, 1.0, 1.0 },
                                { 0.0, 0.0, 0.0 },
                                { -1.0, -1.0, -1.0 } };

            int[,] res_arr = convolve(bitmap_arr, bitmap.Width, bitmap.Height, mask1, 3, 3, 0);

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }
        private int[,] convolveXY(int[,] G, int width, int height, double[,] mask1, double[,] mask2, int mcol, int mrow)
        {
            int[,] res_arr = new int[height, width];
            int xPad = mcol / 2, yPad = mrow;
            double sum, sum1, sum2;

            for (int y = 0; y < height - 2 * yPad; y++)
            {
                for (int x = 0; x < width - 2 * xPad; x++)
                {
                    sum1 = sum2 = 0.0;
                    for (int u = 0; u < mrow; u++)
                    {
                        for (int v = 0; v < mcol; v++)
                        {
                            sum1 += G[y + u, x + v] * mask1[u, v];
                            sum2 += G[y + u, x + v] * mask2[u, v];
                        }
                    }

                    sum = Math.Abs(sum1) + Math.Abs(sum2);

                    if (sum > 255.0) sum = 255.0;
                    if (sum < 0.0) sum = 0.0;
                    res_arr[y + yPad, x + xPad] = (int)sum;

                }
            }

            for (int y = 0; y < yPad; y++)
            {
                for (int x = 0; x < xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (int x = 0; x < xPad; x++)
            {
                for (int y = 0; y < yPad; y++)
                {
                    res_arr[y, x] = res_arr[y, yPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }

            return res_arr;
        }
        public Bitmap pixelCount(Bitmap bitmap, Bitmap orginal)
        {
            BmpControl bmpControl = new BmpControl(bitmap);
            int[,] bmp = bmpControl.GetBitmap(bitmap);
            int width_pix = bitmap.Width / 4;

            int height_one_point = 0;
            int height_two_point = 0;
            int height_three_point = 0;
            int height_four_point = 0;

            int x = 0;
            x = width_pix;

            for (int y = 0; y < bitmap.Height; y++)
            {

                if (bmp[y, x] != 0)
                {

                    height_one_point = y;
                    break;
                }

            }
            x = width_pix * 2;
            for (int y = 0; y < bitmap.Height; y++)
            {

                if (bmp[y, x] != 0)
                {

                    height_two_point = y;
                    break;

                }
            }
            x = width_pix * 3;
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (bmp[y, x] != 0)
                {

                    height_three_point = y;
                    break;
                }

            }


            int height_max = Math.Max(height_one_point, height_two_point);
            height_max = Math.Max(height_max, height_three_point);


            int[,] reset_bmp = new int[200, orginal.Width];
            int reset_he = 0, reset_we = 0;

            for (int y = height_max - 100; y < height_max + 100; y++, reset_he++)
            {
                for (x = 0; x < orginal.Width; x++)
                {

                    reset_bmp[reset_he, x] = orginal.GetPixel(x, y).R;
                }
            }

            Bitmap setbitmap = new Bitmap(orginal, new Size(orginal.Width, 200));

            Color col;
            for (int y = 0; y < setbitmap.Height; y++)
            {
                for (x = 0; x < setbitmap.Width; x++)
                {
                    col = Color.FromArgb(reset_bmp[y, x], reset_bmp[y, x], reset_bmp[y, x]);
                    setbitmap.SetPixel(x, y, col);

                }
            }
            height_pix = 200;
            return setbitmap;
        }
        public Bitmap GaussianBlurring(Bitmap bitmap)
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            double[,] mask = {{1,2,1 },
                               { 2, 4, 2 },
                               {1,2,1} };

            MaskNormalize(mask, 3, 3);
            int[,] res_arr = convolveGB(bitmap_arr, bitmap.Width, bitmap.Height, mask, 3, 3, 0);
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);

        }
        void MaskNormalize(double[,] mask, int width, int height)
        {
            double sum = 0.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum += mask[y, x];
                }
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mask[y, x] /= sum;
                }
            }
        }
        int[,] convolveGB(int[,] G, int width, int height, double[,] mask, int mcol, int mrow, int bias)
        {
            int[,] res_arr = new int[height, width];
            int xPad = mcol / 2;
            int yPad = mrow / 2;
            double sum = 0.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum = 0.0;
                    for (int u = 0; u < mrow; u++)
                    {
                        for (int v = 0; v < mcol; v++)
                        {
                            int posX = x + v - xPad;
                            int posY = y + u - yPad;

                            if (posX >= 0 && posX < width && posY >= 0 && posY < height)
                            {
                                sum += G[posY, posX] * mask[u, v];
                            }
                        }
                    }
                    sum += bias;

                    if (sum > 255.0) sum = 255.0;
                    if (sum < 0.0) sum = 0.0;

                    if (x >= xPad && x < width - xPad && y >= yPad && y < height - yPad)
                    {
                        res_arr[y, x] = (int)sum;
                    }
                    else
                    {
                        res_arr[y, x] = G[y, x]; 
                    }
                }
            }

            return res_arr;
        }
        int minimum(int[] target, int tsize)
        {
            int min = target[0];
            for (int i = 0; i < tsize; i++)
            {
                if (target[i] < min)
                    min = target[i];
            }
            return min;
        }

        public Bitmap MinFilter(Bitmap bitmap)
        {
            BmpControl getbitmap = new BmpControl(bitmap);
            int[,] bmp = getbitmap.GetBitmap(bitmap);
            int row = 0, col = 0;

            if (control_type == 8 || control_type == 10 || control_type == 22) {
                row = bitmap.Width / 50;
                col = bitmap.Height / 50;
            }
            else if (control_type == 11)
            {
                row = bitmap.Width / 30;
                col = bitmap.Height / 30;
            }
            else if (control_type == 16)
            {
                row = bitmap.Width / 20;
                col = bitmap.Height / 20;
            }

            int[,] res_arr = convolveMin(bmp, bitmap.Width, bitmap.Height, row, col);
            int cou_de = 0;

            int[,] save_de = new int[bitmap.Height, bitmap.Width];
            save_de.Initialize();

            int save_min = 255, save_max = 0;

            int histo_max = 0;
            int histo_i = 0;

            Bitmap histoBitmap = new Bitmap(HISTO_WIDTH, HISTO_HEIGHT);
            int[] histogram = new int[256];
            histogram.Initialize();


            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)

                {
                    save_de[y, x] = Math.Abs(bmp[y, x] - res_arr[y, x]);

                    if (save_de[y, x] < save_min)  save_min = save_de[y, x];

                    if (save_de[y, x] > save_max) save_max = save_de[y, x];
                }
            }
            int save_avg = (save_max + save_min) / 2;


            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    histogram[save_de[y, x]]++;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > histo_max)  histo_i = 1;
            }
            
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)

                {

                    if (save_de[y, x] < histo_i)  bmp[y, x] = 0;
                    else bmp[y, x] = save_de[y, x];
                }
            }


            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(bmp, bitmap);

        }
        int[,] convolveMin(int[,] G, int width, int height, int wcol, int wrow)
        {
            int[,] res_arr = new int[height, width];
            int x, y, r, c;
            int xPad = wcol / 2;
            int yPad = wrow / 2;
            int winSize = wcol * wrow;
            int[] target = new int[winSize];

            for (y = 0; y < height - 2 * yPad; y++)
            {
                for (x = 0; x < width - 2 * xPad; x++)
                {
                    int index = 0;
                    for (r = 0; r < wrow; r++)
                        for (c = 0; c < wcol; c++)
                            target[index++] = G[y + r, x + c];


                    res_arr[y + yPad, x + xPad] = minimum(target, index);
                }

            }

            for (y = 0; y < yPad; y++)
            {
                for (x = xPad; x < width - xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (x = 0; x < xPad; x++)
            {
                for (y = 0; y < height; y++)
                {
                    res_arr[y, x] = res_arr[y, xPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }
            return res_arr;
        }


        public Bitmap Streching(Bitmap bitmap) // 스트레칭
        {
            int max = 0, min = 255;
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap_arr[y, x] > max) max = bitmap_arr[y, x];
                    if (bitmap_arr[y, x] < min) min = bitmap_arr[y, x];
                }
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmap_arr[y, x] = (bitmap_arr[y, x] - min) * 255 / (max - min);
                }
            }
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(bitmap_arr, bitmap);
        }


        private double fuzzy(double Imin, double Imax, double Imid, double p)
        {
            double res = 0.0;

            if (p <= Imin || p >= Imax)
            {
                res = 0.0;
            }
            else if (p > Imid)
            {
                res = (Imax - p) / (Imax - Imid);
            }
            else if (p < Imid)
            {
                res = (p - Imin) / (Imid - Imin);
            }
            else if (p == Imid)
            {
                res = 1;
            }
            return res;
        }
        public Bitmap Fuzzy_Binary(Bitmap bitmap) // 퍼지이진화
        {
            int Xmean, Xmin = 255, Xmax = 0, Dmin, Dmax, a, Imax, Imin, Imid;
            int r = 0, g = 0, b = 0;
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            int[,] fuzzy_function = new int[bitmap.Height, bitmap.Width];
            fuzzy_function.Initialize();
            double[,] fuzzy_fun = new double[bitmap.Height, bitmap.Width];
            fuzzy_fun.Initialize();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (Xmin > bitmap_arr[y, x]) Xmin = bitmap_arr[y, x];
                    if (Xmax < bitmap_arr[y, x]) Xmax = bitmap_arr[y, x];
                }
            }

            Xmean = r / (bitmap.Height * bitmap.Width);

            Imax = Xmax;
            Imin = Xmin;
            Imid = (Imax + Imin) / 2;
            MessageBox.Show(" Max = " + Imax + "min = " + Imin);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    fuzzy_fun[y, x] = fuzzy(Imin, Imax, Imid, bitmap_arr[y, x]);
                }
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (fuzzy_fun[y, x] > 0.5) fuzzy_function[y, x] = 0;
                    else if (fuzzy_fun[y, x] < 0.5) fuzzy_function[y, x] = 255;
                }
            }

            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(fuzzy_function, bitmap);
        }

        public Bitmap TinyDeleteHeight(Bitmap bitmap) // 픽셀이 중복될 경우 제거하는 코드(잡음제거)
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            int cnt = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                cnt = 0;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap_arr[y, x] == 255) cnt++;

                    if (bitmap_arr[y, x] == 0 && cnt > 0 && cnt <= 20)
                    {
                        bitmap_arr[y, x - 1] = 0;
                        bitmap_arr[y, x] = 0;
                    }
                    else if (cnt > 20)
                    {
                        cnt = 0;
                    }
                }
            }
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(bitmap_arr, bitmap);
        }
        public Bitmap TinyDeleteWidth(Bitmap bitmap)
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);

            int cnt = 0;
            for (int x = 0; x < bitmap.Width; x++)
            {
                cnt = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (bitmap_arr[y, x] == 255) cnt++;

                    if (bitmap_arr[y, x] == 0 && cnt > 0 && cnt <= 20)
                    {
                        bitmap_arr[y - 1, x] = 0;
                        bitmap_arr[y, x] = 0;
                    }
                    else if (cnt > 20)
                    {
                        cnt = 0;
                    }
                }
            }
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(bitmap_arr, bitmap);
        }

        public Bitmap Dilation(Bitmap bitmap)// 팽창
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);
            int[,] res_arr = convolveClatlr(bitmap_arr, bitmap.Width, bitmap.Height, 3, 3);
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }
        int binErosion(int[] target, int tsize)
        {
            int back = 0, fore = 255;

            for (int i = 1; i < tsize; i++)
            {
                if (target[i] != fore) return back;
            }
            return fore;
        }
        int[,] convolveClatlr(int[,] G, int width, int height, int wcol, int wrow)
        {
            int[,] res_arr = new int[height, width];
            int xPad = wcol / 2;
            int yPad = wrow / 2;
            int win_size = wcol * wrow;
            int[] target = new int[win_size];
            int idx = 0;
            for (int y = 0; y < height - 2 * yPad; y++)
            {
                for (int x = 0; x < width - 2 * xPad; x++)
                {
                    idx = 0;
                    for (int u = 0; u < wrow; u++)
                    {
                        for (int v = 0; v < wcol; v++)
                        {
                            target[idx++] = G[y + u, x + v];
                        }
                    }
                    res_arr[y + yPad, x + xPad] = binErosion(target, win_size);
                }
            }

            for (int y = 0; y < yPad; y++)
            {
                for (int x = 0; x < width - xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (int x = 0; x < xPad; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    res_arr[y, x] = res_arr[y, xPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }

            return res_arr;
        }

        public Bitmap Erision(Bitmap bitmap) // 침식
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);
            int[,] res_arr = convolvepdckd(bitmap_arr, bitmap.Width, bitmap.Height, 3, 3);
            BmpControl setbmp = new BmpControl(bitmap);
            return setbmp.SetBitmap(res_arr, bitmap);
        }
        int vodckd(int[] target, int tsize)
        {
            int back = 0, fore = 255;
            for (int i = 0; i < tsize; i++)
            {
                if (target[i] != back) return fore;
            }
            return back;
        }
        int[,] convolvepdckd(int[,] G, int width, int height, int wcol, int wrow)
        {
            int[,] res_arr = new int[height, width];
            int xPad = wcol / 2;
            int yPad = wrow / 2;
            int win_size = wcol * wrow;
            int[] target = new int[win_size];
            int idx = 0;

            for (int y = 0; y < height - 2 * yPad; y++)
            {
                for (int x = 0; x < width - 2 * xPad; x++)
                {
                    idx = 0;
                    for (int u = 0; u < wrow; u++)
                    {
                        for (int v = 0; v < wcol; v++)
                        {
                            target[idx++] = G[y + u, x + v];
                        }
                    }
                    res_arr[y + yPad, x + xPad] = vodckd(target, win_size);
                }
            }

            for (int y = 0; y < yPad; y++)
            {
                for (int x = 0; x < width - xPad; x++)
                {
                    res_arr[y, x] = res_arr[yPad, x];
                    res_arr[height - 1 - y, x] = res_arr[height - 1 - yPad, x];
                }
            }

            for (int x = 0; x < xPad; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    res_arr[y, x] = res_arr[y, xPad];
                    res_arr[y, width - 1 - x] = res_arr[y, width - 1 - xPad];
                }
            }

            return res_arr;
        }

        public Bitmap histogramBinary(Bitmap bitmap) // 히스토그램이진화
        {
            BmpControl getbmp = new BmpControl(bitmap);
            int[,] bitmap_arr = getbmp.GetBitmap(bitmap);
            int[] histogram = new int[256];
            int histo_max = 0;

            int acc = 0, L = 0, T = 0;
            histogram.Initialize();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    histogram[bitmap_arr[y, x]]++;
                }
            }

            for (int i = 1; i < 256; i++)
            {
                if (histo_max < histogram[i]) histo_max = histogram[i];
            }

            for (int i = 1; i < 256; i++)
            {
                L += histogram[i];
                acc += (histogram[i] * i);
            }

            T = acc / L;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap_arr[y, x] > T)
                    {
                        bitmap_arr[y, x] = 255;
                    }
                    else
                    {
                        bitmap_arr[y, x] = 0;
                    }
                }
            }

            BmpControl setbitmap = new BmpControl(bitmap);
            return setbitmap.SetBitmap(bitmap_arr, bitmap);
        }

    }
}
