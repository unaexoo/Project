using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace hough2
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        Bitmap origin, res;
        Image image;
        int width, height;
        bool isCircle;

        public Form1()
        {
            InitializeComponent();
        }
        void ToGray()
        {
            Bitmap gBitmap = new Bitmap(width, height);
            int br;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    br = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    gBitmap.SetPixel(x, y, Color.FromArgb(br, br, br));
                }
            }

            bitmap = gBitmap;
        }
        void GaussianBlur() 
        {
            Bitmap blurr_bitmap = new Bitmap(width, height);
            int br;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                    {
                        br = (
                            bitmap.GetPixel(x - 1, y - 1).R + bitmap.GetPixel(x, y - 1).R + bitmap.GetPixel(x + 1, y - 1).R +
                            bitmap.GetPixel(x - 1, y).R + bitmap.GetPixel(x, y).R + bitmap.GetPixel(x + 1, y).R +
                            bitmap.GetPixel(x - 1, y + 1).R + bitmap.GetPixel(x, y + 1).R + bitmap.GetPixel(x + 1, y + 1).R) / 9;
                        blurr_bitmap.SetPixel(x, y, Color.FromArgb(br, br, br));
                    }
                    else
                    {
                        blurr_bitmap.SetPixel(x, y, pixel);
                    }
                }
            }

            bitmap = blurr_bitmap;
        }

        void Threshold()
        {
            Bitmap binary_bitmap = new Bitmap(width, height);
            int threshold = OtsuThreshold();
            int br;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    br = (pixel.R > threshold) ? 255 : 0;
                    binary_bitmap.SetPixel(x, y, Color.FromArgb(br, br, br));
                }
            }
            bitmap = binary_bitmap;
        }
        int OtsuThreshold()
        {
            int[] histogram = new int[256];
            int br;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    br = pixel.R;
                    histogram[br]++;
                }
            }

            int totalPixels = width * height;
            double sum = 0;
            for (int i = 0; i < 256; i++)
            {
                sum += i * histogram[i];    // 픽셀 색상 값과 빈도를 곱한 값 누적
            }

            int threshold = 0;
            double maxVal = 0.0;

            double backSum = 0;
            int weightBack = 0;

            int sumForg = (int)sum;
            int weightForg = totalPixels;

            double varBetween, meanBack, meanForground;

            for (int i = 0; i < 256; i++)   // 임계값 후보들 검사
            {
                weightBack += histogram[i];  // 백그라운드 가중치
                if (weightBack == 0)
                    continue;

                weightForg -= histogram[i]; // 전경 가중치
                if (weightForg == 0)
                    break;

                backSum += i * histogram[i];    // 백그라운드 평균 계산을 위한 누적값

                meanBack = backSum / weightBack;    // 백그라운드 평균 : 백그라운드 영역의 픽셀 값의 평균 -> 픽셀의 평균적 밝기 계산
                meanForground = (sumForg - backSum) / weightForg;   // 전경 평균 : 전경에 속하는 픽셀의 평균적 밝기 계산

                varBetween = weightBack * weightForg * Math.Pow(meanBack - meanForground, 2);   // 클래스 간 분산계산 : weightBack * weightForg * (meanBack - meanForground)^2
                // -> 배경과 전경 간의 분산을 최대화 -> 배경과 전경의 구분

                if (varBetween > maxVal)
                {
                    maxVal = varBetween;
                    threshold= i * 2 > 180 ?  i+30 : i * 2;
                }
            }
            textBox2.Text = threshold.ToString();
            return threshold;
        }
        void SearchCircle()
        {
            Mat src = BitmapConverter.ToMat(bitmap);
            Mat srcGray = new Mat();
            Cv2.CvtColor(src, srcGray, ColorConversionCodes.BGR2GRAY); // gray화

            Mat blurred = new Mat();
            Cv2.GaussianBlur(srcGray, blurred, new OpenCvSharp.Size(7, 7), 2, 2); // 가우시안 블러링 (7,7) 사이즈의 마스크 적용, 표준편차 x : 2, y : 2
            Cv2.Threshold(blurred, blurred, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            CircleSegment[] circles = Cv2.HoughCircles(blurred, HoughMethods.Gradient, 1, 50, 150, 35); // Gradient 기반 방법 ,
                                                                                                        // 1 : 이미지와 동일한 해상도,
                                                                                                        // 50: 검출된 원들 사이의 최소 중심거리, 150 :
                                                                                                        // Canny 앳지 검출기에 전달되는 매개변수,
                                                                                                        // 35 : 허프 변환에 사용되는 누적자 값

            Mat dst = src;
            foreach (CircleSegment circle in circles)
            {
                OpenCvSharp.Point center = new OpenCvSharp.Point((int)circle.Center.X, (int)circle.Center.Y); // 중심값
                int radius = (int)circle.Radius; // 반지름

                Console.WriteLine($"center : {circle.Center.X}, {circle.Center.Y}\tRadius : {circle.Radius}");  
                Cv2.Circle(dst, center, radius, Scalar.White, 2); // 원을 dst그림에 흰색으로 그림
            }

            res = BitmapConverter.ToBitmap(src);

            isCircle = circles.Length > 0;
        }
        private void 원검출ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SearchCircle();
            ToGray();
            GaussianBlur();
            Threshold();
            pictureBox2.Image = bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int cnt = 0;
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string folderPath = dialog.SelectedPath;

                    string[] imageFiles = Directory.GetFiles(folderPath, "*.jpg");
                    foreach (string imagePath in imageFiles)
                    {
                        string fileName = Path.GetFileName(imagePath);
                        Image check_image = Image.FromFile(imagePath);
                        bitmap = new Bitmap(check_image, 500, 500);
                        origin = new Bitmap(check_image, 500, 500);
                        width = bitmap.Width;
                        height = bitmap.Height;
                        SearchCircle();
                        if (isCircle == true)
                        {
                            cnt++;
                            Console.WriteLine(fileName);

                        }
                        check_image.Dispose();
                    }
                    MessageBox.Show("끝");
                }
                else
                {
                    MessageBox.Show("폴더를 선택해야 합니다.");
                }
            }
            textBox2.Text = cnt.ToString();
            bitmap.Dispose();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save Image as ...";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "Jpeg File(*.jpg)|*.jpg|All Files(*.*)|*.*|Bitmap File(*.bmp) | *.bmp";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string strFilename = saveFileDialog1.FileName;
                string strLowerFilename = strFilename.ToLower();
                bitmap.Save(strLowerFilename);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string filename = open.FileName;
                image = Image.FromFile(filename);
                bitmap = new Bitmap(image, 500, 500);
                origin = new Bitmap(image, 500, 500);
                width = bitmap.Width;
                height = bitmap.Height;
                pictureBox1.Image = new Bitmap(filename);
            }
        }

        private void 결과ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = res;
            if (isCircle)
            {
                MessageBox.Show("정품입니다.");
            }
            else
            {
                MessageBox.Show("불량입니다.");
            }
        }
    }
}
