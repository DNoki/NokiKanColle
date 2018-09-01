using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Exceptions
{
    /// <summary>
    /// 软件未激活
    /// </summary>
    [Serializable]
    public class NoPermitException : Exception
    {
        public NoPermitException() : base("软件尚未激活！") { }
        public NoPermitException(string message) : base(message) { }
        public NoPermitException(string message, Exception inner) : base(message, inner) { }
        protected NoPermitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 未检测到游戏窗口错误类
    /// </summary>
    [Serializable]
    public class NoGameHandleException : Exception
    {
        public NoGameHandleException() : base("未检测到游戏窗口！") { }
        public NoGameHandleException(string message) : base(message) { }
        public NoGameHandleException(string message, Exception inner) : base(message, inner) { }
        protected NoGameHandleException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 超时错误类
    /// </summary>
    [Serializable]
    public class TimeOutException : Exception
    {
        public TimeOutException() : base("超时错误！") { }
        public TimeOutException(string message) : base(message) { }
        public TimeOutException(string message, Exception inner) : base(message, inner) { }
        protected TimeOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 数据异常错误类
    /// </summary>
    [Serializable]
    public class DataErrorException : Exception
    {
        public DataErrorException(): base("数据异常！") { }
        public DataErrorException(string message) : base(message) { }
        public DataErrorException(string message, Exception inner) : base(message, inner) { }
        protected DataErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// 不移除消息队列重新激活线程
    /// </summary>
    [Serializable]
    public class ReactivateThreadException : Exception
    {
        public ReactivateThreadException():base ("重新开始线程！") { }
        public ReactivateThreadException(string message) : base(message) { }
        public ReactivateThreadException(string message, Exception inner) : base(message, inner) { }
        protected ReactivateThreadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
