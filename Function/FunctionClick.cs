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
    /// 界面点击静态类
    /// </summary>
    public static class FunctionClick
    {
        /// <summary>
        /// 高级鼠标功能
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dwData"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll")]
#pragma warning disable IDE1006 // 命名样式
        private static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint dwData, int dwExtraInfo);
#pragma warning restore IDE1006 // 命名样式

        /// <summary>
        /// 向窗口发送消息 需等待返回
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 向窗口发送消息 无需等待
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 设置鼠标动作的键值
        /// </summary>
        enum MouseEventFlag : uint
        {
            /// <summary>
            /// 发生移动
            /// </summary>
            MOVE = 0x0001,
            /// <summary>
            /// 鼠标按下左键
            /// </summary>
            LEFTDOWN = 0x0002,
            /// <summary>
            /// 鼠标松开左键
            /// </summary>
            LEFTUP = 0x0004,
            /// <summary>
            /// 鼠标按下右键
            /// </summary>
            RIGHTDOWN = 0x0008,
            /// <summary>
            /// 鼠标松开右键
            /// </summary>
            RIGHTUP = 0x0010,
            /// <summary>
            /// 鼠标按下中键
            /// </summary>
            MIDDLEDOWN = 0x0020,
            /// <summary>
            /// 鼠标松开中键
            /// </summary>
            MIDDLEUP = 0x0040,
            XDOWN = 0x0080,
            XUP = 0x0100,
            /// <summary>
            /// 鼠标轮被移动
            /// </summary>
            WHEEL = 0x0800,
            HWHEEL = 0x01000,
            /// <summary>
            /// 虚拟桌面
            /// </summary>
            VIRTUALDESK = 0x4000,
            ABSOLUTE = 0x8000,
        }
        /// <summary>
        /// 模拟鼠标消息
        /// </summary>
        public struct Msg
        {
            /// <summary>
            /// 移动鼠标
            /// </summary>
            public const uint WM_MOUSEMOVE = 0x0200;
            /// <summary>
            /// 摁下鼠标左键
            /// </summary>
            public const uint WM_LBUTTONDOWN = 0x201;
            /// <summary>
            /// 松开鼠标左键
            /// </summary>
            public const uint WM_LBUTTONUP = 0x202;
        }


        /// <summary>
        /// 上一次点击的坐标（防重复点击）
        /// </summary>
        public static Point PreviousXY = Point.Empty;

        /// <summary>
        /// 向窗口句柄发送点击指令
        /// </summary>
        /// <param name="Hwnd">窗口句柄</param>
        /// <param name="PXY">坐标</param>
        /// <returns></returns>
        public static bool ClickInHandle(HandleBase Hwnd, Point PXY, Point OffsetXY)
        {
            try
            {
                IntPtr XY = (IntPtr)(PXY.X + (PXY.Y << 16));
                PostMessage(Hwnd.Handle, Msg.WM_MOUSEMOVE, IntPtr.Zero, XY);
                Delay(20 + Random(20, 40));
                PostMessage(Hwnd.Handle, Msg.WM_LBUTTONDOWN, IntPtr.Zero, XY);
                Delay(50 + Random(5, 20));
                PostMessage(Hwnd.Handle, Msg.WM_LBUTTONUP, IntPtr.Zero, XY);
                Delay(20 + Random(20, 40));
                PostMessage(Hwnd.Handle, Msg.WM_MOUSEMOVE, IntPtr.Zero, (IntPtr)((1 + OffsetXY.X) + ((1 + OffsetXY.Y) << 16)));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 向屏幕发送点击指令
        /// </summary>
        /// <param name="XY"></param>
        /// <returns></returns>
        public static bool ClickInScreen(Point XY, Point offset)
        {
            try
            {
                //获取屏幕分辨率
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;

                //转换坐标
                int x = 65535 / SW * (XY.X + 1 + offset.X);
                int y = 65535 / SH * (XY.Y + 1 + offset.Y);

                mouse_event(MouseEventFlag.ABSOLUTE | MouseEventFlag.MOVE, x, y, 0, 0);
                Functions.Delay(Functions.Random(20, 40));
                mouse_event(MouseEventFlag.ABSOLUTE | MouseEventFlag.LEFTDOWN, 0, 0, 0, 0);
                Functions.Delay(Functions.Random(20, 40));
                mouse_event(MouseEventFlag.ABSOLUTE | MouseEventFlag.LEFTUP, 0, 0, 0, 0);
                Functions.Delay(Functions.Random(20, 40));
                mouse_event(MouseEventFlag.ABSOLUTE | MouseEventFlag.MOVE, (65535 / SW * (/*1 +*/ offset.X - 1)), (65535 / SH * (/*1 +*/offset.Y - 1)), 0, 0);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 鼠标点击指令
        /// </summary>
        /// <param name="xy">坐标</param>
        /// <param name="OffsetX">偏移X</param>
        /// <param name="OffsetY">偏移Y</param>
        /// <returns></returns>
        public static Point Click(DataClick xy, int delayTime = 0, int OffsetX = 0, int OffsetY = 0)
        {
            if (xy == null) return Point.Empty;
            if (GlobalObject.GameHandle.IsSuccess)
            {
                return Click(GlobalObject.GameHandle, xy.GetXY(), delayTime + Random(50, 100), OffsetX, OffsetY);
            }
            else throw new Exceptions.NoGameHandleException();
        }
        /// <summary>
        /// 鼠标点击指令
        /// </summary>
        /// <param name="GameHandle"></param>
        /// <param name="xy"></param>
        /// <param name="delayTime"></param>
        /// <param name="OffsetX"></param>
        /// <param name="OffsetY"></param>
        /// <returns></returns>
        public static Point Click(GameHandleClass GameHandle, Point xy, int delayTime, int OffsetX = 0, int OffsetY = 0)
        {
            try
            {
                GlobalObject.GetUIForm().ChangeClickCoordinate($"{xy.X},{xy.Y}", 2);
                if (!GameHandle.IsSuccess) throw new Exceptions.NoGameHandleException();

                Delay(delayTime + 50);//延迟

                var OffsetXY = new Point(OffsetX, OffsetY);//序列坐标偏移
                OffsetXY.Offset(GlobalObject.ClickOffsetXY);// 游戏窗口偏移坐标（poi）// //手动坐标偏移
                xy.Offset(OffsetXY);

                PreviousXY = xy;//记录本次点击坐标

                Delay(Random(100, 300));
                if (GameHandle.Mode == FunctionHandle.MODE.Handle)
                {
                    ClickInHandle(GameHandle, xy, OffsetXY);
                }
                else if (GameHandle.Mode == FunctionHandle.MODE.Desktop)
                {
                    ClickInScreen(xy, GameHandle.XY);
                }
                else if (GameHandle.Mode == FunctionHandle.MODE.Chorme)
                {
                    xy.Offset(GameHandle.XY);
                    OffsetXY.Offset(GameHandle.XY);
                    ClickInHandle(GameHandle, xy, OffsetXY);
                }
                else throw new Exceptions.DataErrorException("无法识别正确的游戏取色模式！");
                Delay(Random(200, 400));
                return PreviousXY;
            }
            finally
            {
                GlobalObject.GetUIForm().ChangeClickCoordinate($"{xy.X},{xy.Y}", 1);
            }
        }

    }
}