using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using NokiKanColle.Data;
using NokiKanColle.Utility;
using static NokiKanColle.Function.Functions;

namespace NokiKanColle.Function
{

    /// <summary>
    /// 图形静态类
    /// </summary>
    public static class FunctionBitmap
    {
        /// <summary>
        /// 返回hWnd参数所指定的窗口的设备环境
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);
        /// <summary>
        /// 该函数创建与指定的设备环境相关的设备兼容的位图
        /// </summary>
        /// <param name="hdc">设备环境句柄</param>
        /// <param name="nWidth">指定位图的宽度，单位为像素</param>
        /// <param name="nHeight">指定位图的高度，单位为像素</param>
        /// <returns>如果函数执行成功，那么返回值是位图的句柄；如果函数执行失败，那么返回值为NULL</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        private static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
        /// <summary>
        /// 创建一个内存设备上下文环境
        /// </summary>
        /// <param name="hdc">现有设备上下文环境的句柄</param>
        /// <returns>返回内存设备上下文环境的句柄</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);
        /// <summary>
        /// 设备上下文环境的句柄
        /// </summary>
        /// <param name="hdc">设备上下文环境的句柄</param>
        /// <param name="hgdiobj">被选择的对象的句柄</param>
        /// <returns>被取代的对象的句柄</returns>
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        /// <summary>
        /// 复印窗口到位图
        /// </summary>
        /// <param name="hwnd">要复制的窗口句柄</param>
        /// <param name="hDC">要打印的窗口句柄</param>
        /// <param name="nFlags">可选项，指定绘图选项</param>
        /// <returns>布尔值</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        /// <summary>
        /// 删除指定的设备上下文环境
        /// </summary>
        /// <param name="hdc">设备上下文环境的句柄</param>
        /// <returns>布尔值</returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern bool DeleteDC([In] IntPtr hdc);
        [DllImport("Gdi32.dll")]
        private static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// 从图像中提取指定区域的颜色信息
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="xy"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        /// <returns></returns>
        public static string GetColorData(Bitmap bmp, string xy, int Width, int Height)
        {
            try
            {
                //DataJudge data = new DataJudge(xy + " @ " + "");
                //data.color += ColorToStr16(bmp.GetPixel(data.XY.X, data.XY.Y));
                string[] tempxy = xy.Split(',');
                var XY = new Point(Convert.ToInt32(tempxy[0]),Convert.ToInt32(tempxy[1]));
                string str = "";
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        str += ColorToStr16(bmp.GetPixel(XY.X + w, XY.Y + h)) + "|";
                    }
                    str = str.Substring(0, str.Length - 1) + ",";
                }
                return str.Substring(0, str.Length - 1);
            }
            catch (Exception e)
            {
                throw new Exceptions.DataErrorException($"无法获取正确的颜色值！{e.Message}");
            }
        }

        /// <summary>
        /// 获取整个屏幕图像
        /// </summary>
        /// <returns>窗口图像</returns>
        public static Bitmap CopyWindow()
        {
            Bitmap Win = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(Win);
            g.CopyFromScreen(0, 0, 0, 0, Win.Size);
            g.Dispose();
            return Win;
        }

        /// <summary>
        /// 获取句柄窗口图像
        /// </summary>
        /// <returns>窗口图像</returns>
        public static Bitmap PrtGameWindow(HandleBase hwnd)
        {
            return PrtGameWindow(hwnd.Handle);
        }
        /// <summary>
        /// 获取句柄窗口图像
        /// </summary>
        /// <returns>窗口图像</returns>
        public static Bitmap PrtGameWindow(IntPtr hwnd)
        {
            try
            {
                IntPtr hscrdc = GetWindowDC(hwnd);
                FunctionHandle.RECT rect = new FunctionHandle.RECT();
                FunctionHandle.GetClientRect(hwnd, out rect);
                IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, (int)rect.Right - (int)rect.Left, (int)rect.Bottom - (int)rect.Top);
                IntPtr hmemdc = CreateCompatibleDC(hscrdc);
                SelectObject(hmemdc, hbitmap);
                PrintWindow(hwnd, hmemdc, 0);
                Bitmap bmp = Image.FromHbitmap(hbitmap);
                FunctionJudge.ReleaseDC(hwnd, hscrdc);
                DeleteDC(hmemdc);
                DeleteObject(hbitmap);
                return bmp;
            }
            catch (Exception)
            { return null; }
        }


        /// <summary>
        /// 剪裁Bitmap图像
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <param name="Mode">是否释放原始图片</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Bitmap CutImage(Bitmap bmp, int StartX, int StartY, int iWidth, int iHeight, bool Mode)
        {
            if (bmp == null)
            { IsDCBitmap(bmp, Mode); return null; }
            int w = bmp.Width;
            int h = bmp.Height;
            if (StartX >= w || StartY >= h)
            { IsDCBitmap(bmp, Mode); return null; }
            if (StartX + iWidth > w)
            { iWidth = w - StartX; }
            if (StartY + iHeight > h)
            { iHeight = h - StartY; }

            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(bmp, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                IsDCBitmap(bmp, Mode);
                return bmpOut;
            }
            catch
            { IsDCBitmap(bmp, Mode); return null; }
        }
        /// <summary>
        /// 缩放Bitmap图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="Mode">是否释放原始图片</param>
        /// <returns>处理以后的图片</returns>
        public static Bitmap ResizeImage(Bitmap bmp, int newW, int newH, bool Mode)
        {
            if (bmp == null)
            { IsDCBitmap(bmp, Mode); return null; }
            try
            {
                Bitmap outbmp = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(outbmp);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                IsDCBitmap(bmp, Mode);
                return outbmp;
            }
            catch
            {
                IsDCBitmap(bmp, Mode);
                return null;
            }
        }

        /// <summary>
        /// 是否析构图像
        /// </summary>
        /// <param name="bmp">图像</param>
        /// <param name="Mode">是否</param>
        public static void IsDCBitmap(Bitmap bmp, bool Mode)
        {
            if (Mode)
            {
                if (bmp != null)
                    bmp.Dispose();
                bmp = null;
            }
        }

    }
}
