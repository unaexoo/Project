using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ceramic2
{
    class BmpControl
    {
        int width, height;

        public BmpControl(Bitmap bitmap)
        {
            this.width = bitmap.Width;
            this.height = bitmap.Height;
        }

        public int[,] GetBitmap(Bitmap bitmap)
        {
            int[,] bitmap_arr = new int[bitmap.Height, bitmap.Width];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmap_arr[y, x] = bitmap.GetPixel(x, y).R;
                }
            }
            return bitmap_arr;
        }

        public Bitmap SetBitmap(int [,] bitmap_arr, Bitmap bitmap)
        {
            Color color;
            for(int y=0; y<bitmap.Height; y++)
            {
                for(int x=0; x<bitmap.Width; x++)
                {
                    color = Color.FromArgb(bitmap_arr[y, x], bitmap_arr[y, x], bitmap_arr[y, x]);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }

        public int [,,] GetColorBitmap(Bitmap bitmap)
        {
            int[,,] bitmap_arr = new int[bitmap.Height, bitmap.Width, 3];  // 순서 변경
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    bitmap_arr[y, x, 0] = color.R;
                    bitmap_arr[y, x, 1] = color.G;
                    bitmap_arr[y, x, 2] = color.B;
                }
            }
            return bitmap_arr;

        }
        public Bitmap SetColorBitmap(int[,,] bitmap_arr, Bitmap bitmap)
        {
            Color color;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    color = Color.FromArgb(bitmap_arr[y, x,0], bitmap_arr[y, x,1], bitmap_arr[y, x,2]);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }
    }
}
