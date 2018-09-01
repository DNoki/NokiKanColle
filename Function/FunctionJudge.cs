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

using NokiKanColle.Data;
using NokiKanColle.Utility;
using static NokiKanColle.Function.GlobalObject;
using static NokiKanColle.Function.Functions;

namespace NokiKanColle.Function
{
    /// <summary>
    /// 界面判断静态类
    /// </summary>
    public static class FunctionJudge
    {
        /// <summary>
        /// 检索指定窗口的句柄
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        /// <summary>
        /// 释放窗口的句柄
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hdc"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        /// <summary>
        /// 获取句柄窗口的颜色
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="nXPos"></param>
        /// <param name="nYPos"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", EntryPoint = "GetPixel")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private static readonly object _locked = new object();

        /// <summary>
        /// 判断一个窗口图像中是否有游戏
        /// </summary>
        /// <param name="bmp">窗口图像</param>
        /// <param name="XY">如果含有图像则返回坐标</param>
        /// <returns></returns>
        public static bool IsGameInWin(Bitmap bmp, out Point XY)
        {
            XY = new Point(0, 0);
            Point PXY = new Point(0, 0);
            Color HomeColor = Color.FromArgb(255, 231, 237, 234);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    PXY.X = x; PXY.Y = y;
                    if (bmp.GetPixel(x, y) != HomeColor)
                        continue;

                    XY = new Point(x - 773, y - 449);

                    Point DXY = DataPond.DataJudgePonds.母港1.XY;//转换为点坐标
                    DXY.X += XY.X;
                    DXY.Y += XY.Y;

                    string DColor = DataPond.DataJudgePonds.母港1.Color;
                    string[] DColorHeight = DColor.Split(',');//整合颜色数据
                    DColor = string.Join("|", DColorHeight);
                    string[] DColorArray = DColor.Split('|');//所有颜色数据
                    int r = 0;
                    bool result = true;
                    for (int a = 0; a < DColorHeight.Length; a++)//循环High值
                    {
                        for (int b = 0; b < DColorArray.Length / DColorHeight.Length; b++)//循环width值
                        {
                            string PColor;
                            Color PP = bmp.GetPixel(x, y);
                            PColor = ColorToStr16(PP);


                            //RGB值转换
                            /*
                            string PCRed = Convert.ToString(PP.R, 16).ToUpper();
                            if (PCRed.Length == 1) PCRed = "0" + PCRed;
                            string PCGreen = Convert.ToString(PP.G, 16).ToUpper();
                            if (PCGreen.Length == 1) PCGreen = "0" + PCGreen;
                            string PCBlue = Convert.ToString(PP.B, 16).ToUpper();
                            if (PCBlue.Length == 1) PCBlue = "0" + PCBlue;
                            PColor = PCBlue + PCGreen + PCRed;*/

                            if (DColorArray[r++] != PColor)
                            {
                                result = false;
                                break;
                            }
                            x++;
                        }
                        if (!result) break;
                        x = PXY.X;
                        y++;
                    }
                    if (result)
                    { return true; }
                    else continue;
                }
            }
            XY = new Point(0, 0);
            return false;
        }


        /// <summary>
        /// 判断两个颜色是否相似
        /// </summary>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        /// <param name="r">阈值（0~441）</param>
        /// <returns></returns>
        public static bool IsSimilarColor(Color color1, Color color2, int r)
        {
            //方法取自网络
            //将RGB三值映射到三维坐标系，则颜色1与颜色2分别为坐标系内两个点，两点之间的距离若小于等于给定阈值则为近似
            try
            {
                if (r < 0) return false;
                else if (r == 0)//绝对相同
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
        /// 判断给定数据和图像中的颜色数据是否一样（相似）
        /// </summary>
        /// <param name="data">需要判定的数据</param>
        /// <param name="rin">容许色差阈值（0~441）</param>
        /// <param name="OffsetX">X坐标偏移</param>
        /// <param name="OffsetY">Y坐标偏移</param>
        /// <returns></returns>
        public static bool Judge(DataJudge data, Bitmap bitmap = null, int rin = 0, int OffsetX = 0, int OffsetY = 0)
        {
            if (data == null) throw new Exceptions.DataErrorException("给定的判定数据为 null ！");
            if (GlobalObject.GameHandle.IsSuccess)
            {
                if (bitmap == null) bitmap = GameHandle.GetGameBitmap();
                return Judge(bitmap, data, rin, OffsetX, OffsetY);
            }
            else throw new Exceptions.NoGameHandleException();
        }
        /// <summary>
        /// 判断给定数据和图像中的颜色数据是否一样（相似）
        /// </summary>
        /// <param name="bmp">需要判断的游戏图像</param>
        /// <param name="data">需要判定的数据</param>
        /// <param name="rin">容许色差阈值（0~441）</param>
        /// <param name="OffsetX">X坐标偏移</param>
        /// <param name="OffsetY">Y坐标偏移</param>
        /// <returns></returns>
        private static bool Judge(Bitmap bmp, DataJudge data, int rin = 0, int OffsetX = 0, int OffsetY = 0)
        {
            lock (_locked)
            {
                // 更改ui控件显示
                GlobalObject.GetUIForm().ChangeJudgeCoordinate(data.XYToStr, 2);
                Delay(Random(2, 10));
                try
                {
                    DataJudge DecisionData = new DataJudge($"{(data.XY.X + OffsetX).ToString()},{(data.XY.Y + OffsetY).ToString()} @ {FunctionBitmap.GetColorData(bmp, $"{(data.XY.X + OffsetX).ToString()},{(data.XY.Y + OffsetY).ToString()}", data.Width, data.Height)}", "取得数据-" + data.Name);
                    if (!DecisionData.ColorCollection.IsExist())
                    { throw new Exceptions.DataErrorException("从图像中提取的颜色集合数据错误！"); }
                    else if ((DecisionData.ColorCollection.X != data.ColorCollection.X) || (DecisionData.ColorCollection.Y != data.ColorCollection.Y))
                    { throw new Exceptions.DataErrorException("从图像中提取的颜色集合与给定的判定数据大小不符合！"); }
                    for (int y = 0; y < DecisionData.ColorCollection.Y; y++)
                    {
                        for (int x = 0; x < DecisionData.ColorCollection.X; x++)
                        {
                            if (!IsSimilarColor(DecisionData.ColorCollection[x, y], data.ColorCollection[x, y], rin))
                            { return false; }
                        }
                    }
                    Delay(Random(2, 10));
                    return true;
                }
                finally
                {
                    GlobalObject.GetUIForm().ChangeJudgeCoordinate(data.XYToStr, 1);                    
                }
            }
        }
    }
}
