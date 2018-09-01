using System;
using System.Threading;

using NokiKanColle.Function;


namespace NokiKanColle.Utility
{
    /// <summary>
    /// 线程抽象类
    /// </summary>
    public abstract class ThreadsWrapper
    {
        private string _name = "";
        private Thread _thread = null;
        /// <summary>
        /// 线程名(在线程池中的唯一标识)
        /// </summary>
        public string Name
        {
            get
            {
                if (_thread != null)
                {
                    if (_name != _thread.Name)
                    {
                        _thread.Name = _name;
                    }
                }
                return _name;
            }
            set
            {
                _name = value;
                if (_thread != null)
                {
                    if (_thread.Name != _name)
                    {
                        _thread.Name = _name;
                    }
                }
            }
        }
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _startThreadLocked = new object();
        private static readonly object _addMessage = new object();
        /// <summary>
        /// 指示该线程是否已被释放
        /// </summary>
        public bool IsDisposed { get; protected set; } = false;

        /// <summary>
        /// 线程
        /// </summary>
        public Thread Thread
        {
            get { return this._thread; }
            protected set
            {
                this._thread = value;
                this._thread.Name = _name;
            }
        }
        /// <summary>
        /// 线程是否存活
        /// </summary>
        public bool IsThreadAlive
        {
            get
            {
                if (Thread == null)
                {
                    return false;
                }
                return Thread.IsAlive;
            }
        }
        /// <summary>
        /// 获取UI窗口
        /// </summary>
        protected NokiKanColle.Window.Main_Form GetMain_Form => NokiKanColle.Function.GlobalObject.GetUIForm();


        /// <summary>
        /// 添加一条信息到状态信息栏
        /// </summary>
        /// <param name="text">要添加的信息</param>
        protected virtual void Set_AddMessage(string text)
        {
            lock (_addMessage)
            {
                this.GetMain_Form.AddStatusMessage(text);
            }
        }

        /// <summary>
        /// 延时当前线程
        /// </summary>
        /// <param name="Millisecond">延时毫秒数</param>
        public void Delay(int Millisecond)
        { Thread.Sleep(Math.Abs(Millisecond)); }

        /// <summary>
        /// 线程入口
        /// </summary>
        protected abstract void Entrance();

        /// <summary>
        /// 开启线程
        /// </summary>
        public virtual void StartThread()
        {
            lock (_startThreadLocked)// 使用lock语句确保异步线程不会创建同一个线程
            {
                if (!IsThreadAlive)
                {
                    Thread = new Thread(this.Entrance);
                }

                if (FunctionThread.Contains(this.Name))//若线程池有同名线程则不允许开启线程
                    return;
                if (this.Thread != null)
                {
                    Function.FunctionThread.AddThread(this);
                    this.Thread.Start();
                }
            }
        }
        /// <summary>
        /// 关闭线程
        /// </summary>
        public virtual void StopThread()
        {
            if (this.Thread == null)
                return;
            if (this.Thread.IsAlive)
            {
                Function.FunctionThread.RemoveThread(this);
                this.IsDisposed = true;
                this.Thread.Abort();
                this.Thread.Join();
            }
        }

        /// <summary>
        /// 创建并运行一个线程
        /// </summary>
        /// <param name="name">线程名</param>
        public ThreadsWrapper(string name)
        {
            this.Name = name;
        }
    }
}
