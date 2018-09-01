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
    /// 消息队列类
    /// </summary>
    public class MessageQueue : ThreadsWrapper
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        private List<GameProcessWrapper> TotalQueue { get; set; } = new List<GameProcessWrapper>();

        //private Queue<GameProcessWrapper> TotalQueue { get; set; } = new Queue<GameProcessWrapper>();
        /// <summary>
        /// 队列包含的元素数
        /// </summary>
        private int Count => this.TotalQueue.Count;

        /// <summary>
        /// 线程入口
        /// </summary>
        protected override void Entrance()
        {
            try
            { QueueStart(); }
            catch (Exception e)
            { throw e; }
        }
        /// <summary>
        /// 消息队列入口
        /// </summary>
        public void QueueStart()
        {
            while (true)
            {
                Delay(1000);
                if (Count == 0)//消息队列中无元素
                {
                    continue;
                }
                try
                {
                    // 取得第一项并设置为工作模式
                    this.Peek().IsWorking = true;
                    // 修改控件队列文本内容
                    this.GetMain_Form.ChangeThreadNowText(this.Peek().Thread.Name);
                    
                    // 给流程控制权，流程开始工作，工作结束后流程必须给队列返回信息
                    while (this.Peek().IsWorking)
                    {
                        //等待工作结束
                        Delay(1000);
                    }
                    Set_AddMessage(" ");
                    this.Dequeue();//将其移出队列
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 判断队列中是否包含同名线程
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public bool Contains(GameProcessWrapper queue)
        {
            foreach (var q in this.TotalQueue)
            {
                if (q.Thread.Name == queue.Thread.Name)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 将线程添加到消息队列末尾（自动排除同名元素）
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>是否成功添加</returns>
        public bool Enqueue(GameProcessWrapper queue)
        {
            if (Contains(queue)) return false;
            if (queue.IsWorking) return false;
            this.TotalQueue.Add(queue);
            return true;
        }
        /// <summary>
        /// 返回位于 队列 开始处的对象但不将其移除。
        /// </summary>
        /// <returns></returns>
        public GameProcessWrapper Peek()
        {
            if (Count > 0)
                return this.TotalQueue[0];
            else return null;
        }
        /// <summary>
        /// 移除并返回位于 队列 开始处的对象。
        /// </summary>
        /// <returns></returns>
        public GameProcessWrapper Dequeue()
        {
            var temp = this.TotalQueue?[0];
            if (temp!=null)
            {
                this.TotalQueue.RemoveAt(0);
            }
            // 修改控件队列文本内容
            this.GetMain_Form.ChangeThreadNowText();
            return temp;
        }
        /// <summary>
        /// 结束并移除指定索引位置的线程
        /// </summary>
        /// <param name="i"></param>
        public void Remove(int i)
        {
            if (i >= Count)
            {
                return;
            }
            if (i==0)
            {
                TotalQueue[i].IsWorking = false;
                TotalQueue[i].StopThread();
                return;
            }
            TotalQueue.RemoveAt(i);
            TotalQueue[i].StopThread();
        }
        /// <summary>
        /// 移除队列中指定名称的线程，并选择是否关闭线程
        /// </summary>
        /// <param name="queue">线程名</param>
        /// <param name="isCloseThread">true时关闭线程</param>
        public void Remove(GameProcessWrapper queue,bool isCloseThread)
        {
            try
            {
                for (int i = 0; i < TotalQueue.Count; i++)
                {
                    if (TotalQueue[i].Thread.Name == queue.Thread.Name)
                    {
                        if (i == 0)
                        {
                            TotalQueue[i].IsWorking = false;
                            return;
                        }
                        TotalQueue.RemoveAt(i);
                        return;
                    }
                }
            }
            finally
            {
                if (isCloseThread)
                    queue.StopThread();
            }
            
        }
        /// <summary>
        /// 交换队列中的两个项目（这并不符合消息队列机制）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Change(int a, int b)
        {
            if (a == 0 || b == 0) return;
            if (a >= Count || b >= Count) return;
            var temp = TotalQueue[a];
            TotalQueue[a] = TotalQueue[b];
            TotalQueue[b] = temp;
        }
        
        /*
        /// <summary>
        /// 建立并运行队列
        /// </summary>
        public override void StartThread()
        {

            if (!IsThreadAlive)
            {
                Thread = new Thread(QueueStart);
            }
            base.StartThread();
        }*/

        /// <summary>
        /// 消息队列构造函数
        /// </summary>
        public MessageQueue() : base("消息队列（不可关闭）") { this.StartThread(); }
    }
}
