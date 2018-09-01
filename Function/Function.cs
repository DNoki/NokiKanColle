using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

using NokiKanColle.Data;
using NokiKanColle.Utility;
using static NokiKanColle.Function.GlobalObject;
using static NokiKanColle.Function.Functions;


namespace NokiKanColle.Function
{
    /// <summary>
    /// 功能函数
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// String转IntPtr
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntPtr StrToIntPtr(string str)
        {
            return new IntPtr(Convert.ToInt32(str));
        }

        /// <summary>
        /// color转str(16进制)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToStr16(Color color)
        {
            return Convert.ToString(color.R, 16).PadLeft(2, '0').ToUpper() +
                Convert.ToString(color.G, 16).PadLeft(2, '0').ToUpper() +
                Convert.ToString(color.B, 16).PadLeft(2, '0').ToUpper();
            //return color.R.ToString("X2") + color.B.ToString("X2") + color.G.ToString("X2");
        }
        /// <summary>
        /// str(16进制)转Color
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color StringToColor(string str)
        {
            string R = str.Substring(0, 2);
            string G = str.Substring(2, 2);
            string B = str.Substring(4);
            return ColorTranslator.FromHtml("#" + R.ToLower() + G.ToLower() + B.ToLower());
        }

        /// <summary>
        /// 判断两个颜色是否相似
        /// </summary>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        /// <param name="r">阈值（0~441）</param>
        /// <returns>布尔值</returns>
        public static bool IsSimilarColor(Color color1, Color color2, int r)
        {
            try
            {
                if (r < 0) return false;
                else if (r == 0)
                {
                    if (color1.R == color2.R && color1.G == color2.G && color1.B == color2.B) return true;
                    else return false;
                }

                int p = (int)Math.Sqrt(Math.Pow(color1.R - color2.R, 2) + Math.Pow(color1.G - color2.G, 2) + Math.Pow(color1.B - color2.B, 2));
                
                if (r >= p)
                    return true;
                else return false;
            }
            catch (Exception)
            { return false; }

        }
        /// <summary>
        /// 判断两张图像是否相似
        /// </summary>
        /// <param name="bmp1">图像1</param>
        /// <param name="bmp2">图像2</param>
        /// <param name="r">阈值（0~441）</param>
        /// <returns>布尔值</returns>
        public static bool IsSimilarBitmap(Bitmap bmp1, Bitmap bmp2, int r)
        {
            if ((bmp1.Width != bmp2.Width) || bmp1.Height != bmp2.Height)
                return false;
            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    if (!IsSimilarColor(bmp1.GetPixel(x, y), bmp2.GetPixel(x, y), 30))
                        return false;
                }
            }
            return true;//若所有颜色全部相似，返回真
        }

        /// <summary>
        /// 将毫秒数转换为时间间隔类
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static TimeSpan MillisecondToTime(int millisecond)
        {
            long ticks = (long)millisecond * 10000;
            TimeSpan Time = new TimeSpan(ticks);
            return Time;
        }
        /// <summary>
        /// 将秒数转换为时间间隔类
        /// </summary>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static TimeSpan SecondToTime(int Second)
        {
            long ticks = (long)Second * 10000000;
            TimeSpan Time = new TimeSpan(ticks);
            return Time;
        }

        /// <summary>
        /// 取随机整数（随机数可取该下界值，但不能取该上界值）
        /// </summary>
        /// <param name="Min">下界值</param>
        /// <param name="Max">上界值</param>
        /// <returns>随机整数</returns>
        public static int Random(int Min, int Max)//随机数可取该下界值，但不能取该上界值
        {
            Thread.Sleep(23);
            Random RNum = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            int Num = RNum.Next(Min, Max);
            return Num;
        }
        /// <summary>
        /// 取随机整数（随机数可取0，但不能取该上界值）
        /// </summary>
        /// <param name="Max">上界值</param>
        /// <returns>随机整数</returns>
        public static int Random(int Max)//随机数可取0，但不能取该上界值
        {
            Thread.Sleep(23);
            Random RNum = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            return RNum.Next(Max);
        }

        public static void Delay(int Millisecond)
        { Thread.Sleep(Math.Abs(Millisecond)); }
    }

    /// <summary>
    /// 全局对象
    /// </summary>
    public static class GlobalObject
    {
        public static Permit ProgramPermit = new Permit();

        /// <summary>
        /// UI窗口对外接口
        /// </summary>
        public static Func<NokiKanColle.Window.Main_Form> GetUIForm;

        /// <summary>
        /// 游戏窗口句柄
        /// </summary>
        public static GameHandleClass GameHandle = new GameHandleClass();

        /// <summary>
        /// 手动偏移点击坐标
        /// </summary>
        public static Point ClickOffsetXY { get; set; } = Point.Empty;

        /// <summary>
        /// 线程池
        /// </summary>
        public static List<NokiKanColle.Utility.ThreadsWrapper> TotalThread = new List<ThreadsWrapper>();
        
    }
}
