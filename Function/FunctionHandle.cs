using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

using static NokiKanColle.Function.Functions;

namespace NokiKanColle.Function
{

    /// <summary>
    /// 句柄静态类
    /// </summary>
    public static class FunctionHandle
    {
        /// <summary>
        /// 获取某一坐标的句柄
        /// </summary>
        /// <param name="point">基于屏幕的坐标</param>
        /// <returns>句柄</returns>
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(Point point);
        /// <summary>
        /// 判断窗体是否有效的API
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);
        /// <summary>
        /// 获得一个指定子窗口的父窗口句柄
        /// </summary>
        /// <param name="hWnd">子窗口句柄</param>
        /// <returns>父窗口句柄</returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hWnd);
        /// <summary>
        /// 在窗口列表中寻找与指定条件相符的第一个子窗口
        /// </summary>
        /// <param name="parentHandle">父窗口的句柄</param>
        /// <param name="childAfter">子窗口句柄。查找从在Z序中的下一个子窗口开始。</param>
        /// <param name="className">指向一个指定了类名的空结束字符串，或一个标识类名字符串的成员的指针</param>
        /// <param name="windowTitle">指向一个指定了窗口名（窗口标题）的空结束字符串。如果该参数为 NULL，则为所有窗口全匹配。</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        /// <summary>
        /// 获取客户区窗口大小
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpRect">传值矩形</param>
        /// <returns>布尔值</returns>
        [DllImport("user32.dll", EntryPoint = "GetClientRect")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        public struct RECT
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        }


        /// <summary>
        /// 获取当前鼠标位置的句柄
        /// </summary>
        /// <returns>句柄</returns>
        public static IntPtr GetMouseHandle()
        {
            return WindowFromPoint(Control.MousePosition);
        }

        /// <summary>
        /// 返回子窗口的顶层窗口句柄
        /// </summary>
        /// <param name="Child"></param>
        /// <returns></returns>
        public static IntPtr GetTopFatherHandle(IntPtr Child)
        {
            IntPtr Father = GetParent(Child);

            while (!(Father == null || Father == IntPtr.Zero))
            {
                Child = Father;
                Father = GetParent(Child);
            }
            return Child;
        }//*/
         /// <summary>
         /// 根据父窗口寻找子窗口句柄
         /// </summary>
         /// <param name="Father">父窗口句柄</param>
         /// <param name="Child">已知子窗口句柄（为0时查找父窗口下的第一个子窗口）</param>
         /// <returns>当前父窗口下的已知子窗口的下一个子窗口句柄</returns>
        public static IntPtr FindChildHandle(IntPtr Father, IntPtr Child)
        { return FunctionHandle.FindWindowEx(Father, Child, null, null); }
        /// <summary>
        /// 根据父窗口寻找子窗口句柄（String）
        /// </summary>
        /// <param name="Father">父窗口句柄</param>
        /// <param name="Child">已知子窗口句柄（为0时查找父窗口下的第一个子窗口）</param>
        /// <returns>当前父窗口下的已知子窗口的下一个子窗口句柄</returns>
        public static string FindChildHandle(string Father, string Child)
        { return FindChildHandle(StrToIntPtr(Father), StrToIntPtr(Child)).ToString(); }

        /// <summary>
        /// 游戏取图模式
        /// </summary>
        public enum MODE:int
        {
            Null = 0, Handle = 1, Desktop = 2, Chorme = 3
        }
    }
}
