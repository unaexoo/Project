using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Pen P_black = new Pen(Color.FromArgb(58, 58, 58));
        Pen P_white = new Pen(Color.White);
        Pen P_pink = new Pen(Color.FromArgb(254, 180, 215));
        Pen P_black4 = new Pen(Color.FromArgb(58, 58, 58), 4.0f);
        Pen Mouse_black = new Pen(Color.Black, 2.0f);
        Pen P_black30 = new Pen(Color.FromArgb(58, 58, 58), 30.0f);

        Brush B_black = new SolidBrush(Color.FromArgb(58, 58, 58));
        Brush B_white = new SolidBrush(Color.White);
        Brush B_pink = new SolidBrush(Color.FromArgb(254, 180, 215));
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Body(g);
            Head(g);
        }

        public void Head(Graphics g)
        {
            Rectangle reclef_ear = new Rectangle(55, 30, 25, 25);
            Rectangle recright_ear = new Rectangle(470, 30, 25, 25);
            Rectangle pink_head = new Rectangle(250, 150, 40, 40);
            Rectangle rec_phead = new Rectangle(260, 175, 20, 20);
            Rectangle left_bpoint = new Rectangle(255, 170, 10, 10);
            Rectangle right_bpoint = new Rectangle(275, 170, 10, 10);

            Point[] left_ear = new Point[5];
            left_ear[0].X = 70; left_ear[0].Y = 50;
            left_ear[1].X = 80; left_ear[1].Y = 175;
            left_ear[2].X = 120; left_ear[2].Y = 210;
            left_ear[3].X = 190; left_ear[3].Y = 130;
            left_ear[4].X = 180; left_ear[4].Y = 105;

            Point[] right_ear = new Point[5];
            right_ear[0].X = 355; right_ear[0].Y = 120;
            right_ear[1].X = 365; right_ear[1].Y = 105;
            right_ear[2].X = 476; right_ear[2].Y = 50;
            right_ear[3].X = 465; right_ear[3].Y = 175;
            right_ear[4].X = 420; right_ear[4].Y = 210;

            Point[] face_ret = new Point[3];
            face_ret[0].X = 150; face_ret[0].Y = 160;
            face_ret[1].X = 270; face_ret[1].Y = 225;
            face_ret[2].X = 400; face_ret[2].Y = 160;

            Point[] left_eye = new Point[3];
            left_eye[0].X = 175; left_eye[0].Y = 240;
            left_eye[1].X = 210; left_eye[1].Y = 265;
            left_eye[2].X = 200; left_eye[2].Y = 230;

            Point[] right_eye = new Point[3];
            right_eye[0].X = 350; right_eye[0].Y = 245;
            right_eye[1].X = 320; right_eye[1].Y = 265;
            right_eye[2].X = 330; right_eye[2].Y = 210;

            Point[] lleyebrow = { new Point(180, 247), new Point(175, 240), new Point(170, 230) };
            Point[] lseyebrow = { new Point(175, 255), new Point(170, 248), new Point(167, 242) };

            Point[] rleyebrow = { new Point(345, 250), new Point(355, 240), new Point(360, 230) };
            Point[] rseyebrow = { new Point(355, 255), new Point(360, 250), new Point(365, 242) };

            // 귀
            g.FillPolygon(B_black, left_ear);
            g.FillPolygon(B_black, right_ear);
            g.FillPolygon(B_black, face_ret);

            // 얼굴
            g.FillEllipse(B_black, 80, 105, 380, 300);
            g.FillEllipse(B_white, 105, 180, 330, 222);
            g.FillPolygon(B_black, face_ret);
            g.FillEllipse(B_black, reclef_ear);
            g.FillEllipse(B_black, recright_ear);

            // 핑크 해골
            g.FillEllipse(B_pink, pink_head);
            g.FillRectangle(B_pink, rec_phead);
            g.FillEllipse(B_black, left_bpoint);
            g.FillEllipse(B_black, right_bpoint);

            // 핑크 해골 블랙 선
            g.DrawLine(P_black, new Point(265, 187), new Point(265, 195));
            g.DrawLine(P_black, new Point(270, 187), new Point(270, 195));
            g.DrawLine(P_black, new Point(275, 187), new Point(275, 195));

            // 눈
            g.FillEllipse(B_black, 170, 240, 40, 70);
            g.FillPolygon(B_white, left_eye);

            g.FillEllipse(B_black, 320, 240, 40, 70);
            g.FillPolygon(B_white, right_eye);

            // 눈썹
            g.DrawCurve(P_black4, lleyebrow);
            g.DrawCurve(P_black4, lseyebrow);

            g.DrawCurve(P_black4, rleyebrow);
            g.DrawCurve(P_black4, rseyebrow);

            // 코
            g.FillEllipse(B_pink, 258, 305, 30, 20);

            // 입
            g.DrawBezier(Mouse_black, new Point(230, 340), new Point(220, 365), new Point(320, 365), new Point(310, 340));

        }
        public void Body(Graphics g)
        {
            Point[] polygon1 = new Point[3];
            polygon1[0].X = 160; polygon1[0].Y = 375;
            polygon1[1].X = 95; polygon1[1].Y = 420;
            polygon1[2].X = 220; polygon1[2].Y = 390;

            Point[] polygon2 = new Point[3];
            polygon2[0].X = 175; polygon2[0].Y = 375;
            polygon2[1].X = 200; polygon2[1].Y = 445;
            polygon2[2].X = 285; polygon2[2].Y = 395;

            Point[] polygon3 = new Point[3];
            polygon3[0].X = 245; polygon3[0].Y = 395;
            polygon3[1].X = 325; polygon3[1].Y = 445;
            polygon3[2].X = 355; polygon3[2].Y = 385;

            Point[] polygon4 = new Point[3];
            polygon4[0].X = 325; polygon4[0].Y = 390;
            polygon4[1].X = 435; polygon4[1].Y = 420;
            polygon4[2].X = 380; polygon4[2].Y = 370;

            Rectangle pinkpoint1 = new Rectangle(85, 410, 20, 20);
            Rectangle pinkpoint2 = new Rectangle(190, 435, 20, 20);
            Rectangle pinkpoint3 = new Rectangle(315, 435, 20, 20);
            Rectangle pinkpoint4 = new Rectangle(425, 410, 20, 20);

            Rectangle body = new Rectangle(170, 385, 200, 160);
            Rectangle left_hand = new Rectangle(80, 460, 70, 70);
            Rectangle right_hand = new Rectangle(400, 460, 70, 70);

            Rectangle left_leg = new Rectangle(170, 500, 80, 130);
            Rectangle right_leg = new Rectangle(290, 500, 80, 130);

            Point[] left_arm = new Point[4];
            left_arm[0].X = 155; left_arm[0].Y = 380;
            left_arm[1].X = 100; left_arm[1].Y = 470;
            left_arm[2].X = 145; left_arm[2].Y = 500;
            left_arm[3].X = 205; left_arm[3].Y = 400;

            Point[] right_arm = new Point[4];
            right_arm[0].X = 330; right_arm[0].Y = 400;
            right_arm[1].X = 400; right_arm[1].Y = 490;
            right_arm[2].X = 435; right_arm[2].Y = 460;
            right_arm[3].X = 370; right_arm[3].Y = 380;

            Point[] tail = new Point[4];
            tail[0].X = 365; tail[0].Y = 535;
            tail[1].X = 405; tail[1].Y = 525;
            tail[2].X = 440; tail[2].Y = 510;
            tail[3].X = 480; tail[3].Y = 462;

            Point[] tail_triangle = new Point[3];
            tail_triangle[0].X = 455; tail_triangle[0].Y = 450;
            tail_triangle[1].X = 500; tail_triangle[1].Y = 435;
            tail_triangle[2].X = 505; tail_triangle[2].Y = 480;

            // 몸통
            g.FillRectangle(B_white, body);

            // 꼬리
            g.DrawCurve(P_black30, tail);
            g.FillPolygon(B_black, tail_triangle);

            // 팔, 손
            g.FillPolygon(B_white, left_arm);
            g.FillEllipse(B_white, left_hand);
            g.FillPolygon(B_white, right_arm);
            g.FillEllipse(B_white, right_hand);

            // 넥 카라 부분
            g.FillPolygon(B_black, polygon1);
            g.FillPolygon(B_black, polygon2);
            g.FillPolygon(B_black, polygon3);
            g.FillPolygon(B_black, polygon4);

            g.FillEllipse(B_pink, pinkpoint1);
            g.FillEllipse(B_pink, pinkpoint2);
            g.FillEllipse(B_pink, pinkpoint3);
            g.FillEllipse(B_pink, pinkpoint4);

            // 다리
            g.FillRectangle(B_white, left_leg);
            g.FillRectangle(B_white, right_leg);
            g.FillEllipse(B_white, 150, 600, 100, 50);
            g.FillEllipse(B_white, 290, 600, 100, 50);
        }
    }
}
