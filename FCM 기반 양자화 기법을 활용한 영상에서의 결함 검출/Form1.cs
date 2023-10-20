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
    public partial class Form1 : Form
    {
        FuzzyCMenas FcmDialog = new FuzzyCMenas();
        enum RESULT_MODE { Center, Jet }
        RESULT_MODE result_mode;
        FCM fcm;

        Bitmap bitmap;
        Bitmap original;
        int idx = 0;
        //public int control_type = 0;
        Ceramic ceramic = new Ceramic();
        public Bitmap[] saveBitmap = new Bitmap[20];
        public Bitmap[] viewBitmap = new Bitmap[3];
        public Bitmap[] histoBitmap = new Bitmap[2];

        public Form1()
        {
            InitializeComponent();
            btnResultMode.Enabled = false;
            btnFCMQuan.Enabled = false;
            result_mode = RESULT_MODE.Center;
            fcm = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics gr = CreateGraphics();
            Image image;
            openFileDialog1.Title = "영상 파일 열기";
            openFileDialog1.Filter = "All Files(*.*) |*.*| Bitmap File(*.bmp) | *.bmp |Jpeg File(*.jpg) |*.jpg";

            pictureBox1.Width = 400;
            pictureBox1.Height = 400;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string strFilename = openFileDialog1.FileName;
                image = Image.FromFile(strFilename);
                String str_safeFileName = openFileDialog1.SafeFileName;
                if (strFilename != "No Image")
                {
                    bitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    original = new Bitmap(image);
                    pictureBox1.Image = bitmap;
                    saveBitmap[0] = original;
                    viewBitmap[0] = bitmap;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "파일저장";
            saveFileDialog1.Filter = "All File(*.*)|*.*|BitmapFile(*.bmp)|*.bmp|JpegFile(*.jpg)|*.jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string str = saveFileDialog1.FileName;
                string strsaver = str.ToLower();
                pictureBox1.Image.Save(strsaver);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
            if (ceramic.control_type == 0)
            {
                MessageBox.Show("크기를 선택해주세요");
            }
            else  if (ceramic.control_type == 8)
            {
                viewBitmap[1] = ceramic.OtusBinary(new Bitmap(viewBitmap[0]));
                viewBitmap[2] = ceramic.SobelMask(new Bitmap(viewBitmap[1]));

                saveBitmap[1] = ceramic.OtusBinary(new Bitmap(saveBitmap[0]));
                saveBitmap[2] = ceramic.SobelMask(new Bitmap(saveBitmap[1]));

                saveBitmap[3] = ceramic.pixelCount(new Bitmap(saveBitmap[2]), saveBitmap[0]);
                saveBitmap[4] = ceramic.MinFilter(new Bitmap(saveBitmap[3]));
                for (int i = 0; i < 3; i++)
                {
                    saveBitmap[5] = ceramic.GaussianBlurring(saveBitmap[4]);
                }
                saveBitmap[6] = ceramic.Streching(new Bitmap(saveBitmap[5]));
                saveBitmap[7] = ceramic.MedianFilter(new Bitmap(saveBitmap[6]));

                MessageBox.Show("Finish");
            }
            else
            {
                viewBitmap[1] = ceramic.OtusBinary(new Bitmap(viewBitmap[0]));
                viewBitmap[2] = ceramic.SobelMask(new Bitmap(viewBitmap[1]));

                saveBitmap[1] = ceramic.OtusBinary(new Bitmap(saveBitmap[0]));
                saveBitmap[2] = ceramic.SobelMask(new Bitmap(saveBitmap[1]));
                saveBitmap[3] = ceramic.pixelCount(new Bitmap(saveBitmap[2]), saveBitmap[0]);

              
                saveBitmap[4] = ceramic.MinFilter(new Bitmap(saveBitmap[3]));
                saveBitmap[5] = ceramic.GaussianBlurring(new Bitmap(saveBitmap[4]));
                saveBitmap[6] = ceramic.Streching(new Bitmap(saveBitmap[5]));
                saveBitmap[7] = ceramic.MedianFilter(new Bitmap(saveBitmap[6]));
                MessageBox.Show("Finish");
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ceramic.control_type = 8;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ceramic.control_type = 10;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            ceramic.control_type = 11;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ceramic.control_type = 16;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            ceramic.control_type = 22;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            idx++;


            switch (idx)
            {
                case 0://origianl Image
                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;

                case 1://ROI 
                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;

                case 2://Media Filter
                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;
                case 3:// end-in search 
                    pictureBox1.Width = original.Width;
                    pictureBox1.Height = ceramic.height_pix;
                    pictureBox1.Image = saveBitmap[idx];


                    pictureBox2.Width = original.Width;
                    pictureBox2.Height = ceramic.height_pix;
                    pictureBox2.Image = saveBitmap[idx];
                    break;

                case 4://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;


                case 5://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    btnFCMQuan.Enabled = true;
                    break;


                case 6://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 7://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 8://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 9://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 10://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                default: break;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            idx--;


            switch (idx)
            {

                case 0://origianl Image

                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;

                case 1://ROI 
                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;

                case 2://Media Filter
                    pictureBox1.Width = 400;
                    pictureBox1.Height = 400;
                    pictureBox1.Image = viewBitmap[idx];
                    break;

                case 3:// end-in search 
                    pictureBox1.Width = original.Width;
                    pictureBox1.Height = ceramic.height_pix;
                    pictureBox1.Image = saveBitmap[idx];
                    break;

                case 4://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;

                case 5://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;

                case 6://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 7://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 8://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 9://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                case 10://max-min
                    pictureBox1.Image = saveBitmap[idx];
                    break;
                default: break;
            }
        }

        private void btnFCMQuan_Click(object sender, EventArgs e)
        {
            if (FcmDialog.ShowDialog() == DialogResult.OK)
            {
                int classOfCount = FcmDialog.countOfClass;

                // FCM 양자화 클래스를 초기화한다.
                fcm = new FCM((Bitmap)pictureBox1.Image,classOfCount);
                // 양자화한다.
                fcm.Quantize();
                switch (result_mode)
                {
                    case RESULT_MODE.Center:
                        pictureBox1.Image = fcm.GetCenterClassMap();

                        pictureBox2.Image = ceramic.MedianFilter(saveBitmap[6]);
                        pictureBox3.Image = saveBitmap[3];
                        break;
                    case RESULT_MODE.Jet:
                        pictureBox1.Image = fcm.GetColorClassMap();
 
                        pictureBox2.Image = ceramic.MedianFilter(saveBitmap[6]);
                        pictureBox3.Image = saveBitmap[3];
                        break;
                }
                
                btnResultMode.Enabled = true;
                MessageBox.Show("학습이 완료되었습니다.");
            }
        }

        private void btnResultMode_Click(object sender, EventArgs e)
        {
            result_mode = (RESULT_MODE)((int)(result_mode + 1) % System.Enum.GetValues(typeof(RESULT_MODE)).Length);

            // 버튼 텍스트 변경
            switch (result_mode)
            {
                case RESULT_MODE.Center:
                    btnResultMode.Text = "색상 맵";
                    break;
                case RESULT_MODE.Jet:
                    btnResultMode.Text = "중심벡터";
                    break;
            }

            // 결과 영상 모드 변경


            switch (result_mode)
            {
                case RESULT_MODE.Center:
                    pictureBox1.Image = fcm.GetCenterClassMap();
                    break;
                case RESULT_MODE.Jet:
                    pictureBox1.Image = fcm.GetColorClassMap();
                    break;
            }
            
        }

        private void restartbutton6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            btnResultMode.Enabled = false;
            btnFCMQuan.Enabled = false;

            pictureBox1.Width = 400;
            pictureBox1.Height = 400;
            idx = 0;
        }
    }
}

