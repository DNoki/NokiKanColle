using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using NokiKanColle.Function;
using static NokiKanColle.Function.Functions;

namespace NokiKanColle.Utility
{
    /// <summary>
    /// 游戏窗口句柄基类
    /// </summary>
    public class HandleBase
    {
        private IntPtr _handle = IntPtr.Zero;

        /// <summary>
        /// 句柄
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (!FunctionHandle.IsWindow(_handle))
                { _handle = IntPtr.Zero; }//若窗口不存在则返回句柄0
                return _handle;
            }
            set
            {
                _handle = value;
            }
        }

        /// <summary>
        /// 获得句柄窗口客户区窗口大小
        /// </summary>
        public FunctionHandle.RECT Rect
        {
            get
            {
                FunctionHandle.RECT rect = new FunctionHandle.RECT();
                FunctionHandle.GetClientRect(Handle, out rect);
                return rect;
            }
        }
    }

    /// <summary>
    /// 游戏窗口句柄类
    /// </summary>
    public class GameHandleClass : HandleBase, IDisposable
    {

        /// <summary>
        /// 是否已成功抓到游戏窗口
        /// </summary>
        private bool _isSuccess = false;
        /// <summary>
        /// 游戏窗口的相对坐标
        /// </summary>
        private Point _xy = new Point(0, 0);
        /// <summary>
        /// 模式
        /// </summary>
        private FunctionHandle.MODE _mode = FunctionHandle.MODE.Null;
        private Bitmap _photo = null;

        /// <summary>
        /// 游戏窗口的相对坐标
        /// </summary>
        public Point XY
        {
            set { _xy = value; }
            get { return _xy; }
        }

        /// <summary>
        /// 模式
        /// </summary>
        public FunctionHandle.MODE Mode
        {
            set
            {
                if (value != FunctionHandle.MODE.Null)
                {
                    _isSuccess = true;
                    _mode = value;
                }
                else
                {
                    _mode = FunctionHandle.MODE.Null;
                    _isSuccess = false;
                    Handle = IntPtr.Zero;
                    if (_photo != null)
                    {
                        _photo.Dispose(); _photo = null;
                    }
                }
            }
            get { return _mode; }
        }
        /// <summary>
        /// 是否已成功抓到游戏窗口
        /// </summary>
        public bool IsSuccess
        { get { return _isSuccess; } }


        /// <summary>
        /// 判别窗口分辨率是否为800x400
        /// </summary>
        /// <returns></returns>
        public bool IsWin_800x480()
        {
            if ((Rect.Right - Rect.Left) == 800 && (Rect.Bottom - Rect.Top) == 480)
                return true;
            else return false;
        }


        /// <summary>
        /// 获取游戏窗口图像（*注意析构图像）
        /// </summary>
        /// <returns>游戏图像</returns>
        public Bitmap GetGameBitmap()
        {
            if (!IsSuccess) return null;
            if (_photo != null)
            {
                _photo?.Dispose(); _photo = null;
            }
            if (_mode == FunctionHandle.MODE.Handle)
                _photo = FunctionBitmap.PrtGameWindow(this);
            else if (_mode == FunctionHandle.MODE.Desktop)
                _photo = FunctionBitmap.CutImage(FunctionBitmap.CopyWindow(), _xy.X, _xy.Y, 800, 480, true);
            else if (_mode == FunctionHandle.MODE.Chorme)
                _photo = FunctionBitmap.CutImage(FunctionBitmap.PrtGameWindow(this), _xy.X, _xy.Y, 800, 480, true);
            else return null;
            return _photo;
        }


        private bool _isDisposed = false;// 是否已释放资源的标志

        //由垃圾回收器调用，释放非托管资源
        ~GameHandleClass()
        {
            Dispose(false);
        }

        //实现接口方法
        //由类的使用者，在外部显示调用，释放类资源
        public void Dispose()
        {
            Dispose(true);// 释放托管和非托管资源
            GC.SuppressFinalize(this);//将对象从垃圾回收器链表中移除，从而在垃圾回收器工作时，只释放托管资源，而不执行此对象的析构函数
        }

        //参数为true表示释放所有资源，只能由使用者调用
        //参数为false表示释放非托管资源，只能由垃圾回收器自动调用
        //如果子类有自己的非托管资源，可以重载这个函数，添加自己的非托管资源的释放
        //但是要记住，重载此函数必须保证调用基类的版本，以保证基类的资源正常释放
        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)// 如果资源未释放 这个判断主要用了防止对象被多次释放
            {
                if (disposing)
                {
                    //在这里添加需要释放的托管资源
                }
                //在这里添加需要释放的非托管资源
                Handle = IntPtr.Zero;
                _photo?.Dispose(); _photo = null;
            }
            this._isDisposed = true; // 标识此对象已释放
        }//*/

        public GameHandleClass()
        {

        }
        public GameHandleClass(GameHandleClass copyGameHandle)
        {
            this.Handle = new IntPtr(copyGameHandle.Handle.ToInt32());
        }
    }




}
