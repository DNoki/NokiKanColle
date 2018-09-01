using static NokiKanColle.Function.GlobalObject;

namespace NokiKanColle.Function
{
    /// <summary>
    /// 线程静态类
    /// </summary>
    public static class FunctionThread
    {
        /// <summary>
        /// 添加线程
        /// </summary>
        /// <param name="thread"></param>
        public static void AddThread(NokiKanColle.Utility.ThreadsWrapper thread)
        {
            GlobalObject.GetUIForm().AddThreadItem(thread.Name);
            GlobalObject.TotalThread.Add(thread);
        }
        /// <summary>
        /// 移除线程
        /// </summary>
        /// <param name="thread"></param>
        public static void RemoveThread(NokiKanColle.Utility.ThreadsWrapper thread)
        {
            GlobalObject.GetUIForm().RemoveThreadItem(thread.Name);
            GlobalObject.TotalThread.Remove(thread);
        }


        /// <summary>
        /// 判断线程池中是否包含同名线程
        /// </summary>
        /// <param name="name">线程名</param>
        /// <returns></returns>
        public static bool Contains(string name)
        {
            foreach (NokiKanColle.Utility.ThreadsWrapper t in TotalThread)
            {

                if (t.Thread.Name == name)
                {
                    if (t.Thread.IsAlive)
                    {
                        return true;
                    }
                    else
                    {
                        RemoveThread(t);//移出死亡线程
                        return false;
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// 取得同名线程
        /// </summary>
        /// <param name="name">线程名</param>
        /// <returns></returns>
        public static T GetThread<T>(string name) where T : NokiKanColle.Utility.ThreadsWrapper
        {
            foreach (NokiKanColle.Utility.ThreadsWrapper t in TotalThread)
            {
                if (t.Thread.Name == name)
                    return t as T;
            }
            return null;
        }
        /// <summary>
        /// 通过线程名关闭某一线程
        /// </summary>
        /// <param name="name">线程名</param>
        /// <returns></returns>
        public static bool CloseThread(string name)
        {
            foreach (NokiKanColle.Utility.ThreadsWrapper t in TotalThread)
            {
                if (t.Thread.Name == name)
                {
                    t.StopThread();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 杀死所有池内线程
        /// </summary>
        public static void KillAllThreads()
        {
            while (TotalThread.Count != 0)
            {
                if (TotalThread[0].Thread.IsAlive)
                    TotalThread[0].StopThread();
                else
                    RemoveThread(TotalThread[0]);//移出死亡线程
            }
        }
    }
}
