using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace NokiKanColle.Function
{
    /// <summary>
    /// 错误日志
    /// </summary>
    static class FunctionExceptionLog
    {
        //private static StreamWriter _logFile = new StreamWriter(Application.StartupPath + @"\错误日志.log", true, Encoding.Unicode);

        public struct Log
        {
            public DateTime Time { get; set; }
            public Exception Error { get; set; }
            /// <summary>
            /// 仅提取错误信息
            /// </summary>
            public string ShortMessage=>Error.Message.Replace("\n", "|").Replace("\r", "|");
            /// <summary>
            /// 不带时间的完整的错误信息（带异常位置）
            /// </summary>
            public string MessageCutTime=> Error.ToString().Replace("\n", "|").Replace("\r","|");
            /// <summary>
            /// 带时间的完整的错误信息
            /// </summary>
            public string Message => $"{Time.ToShortDateString()} {Time.ToLongTimeString()}: {MessageCutTime}";
            public Log(Exception error)
            {
                this.Error = error;
                this.Time = DateTime.Now;
            }
            public Log(Exception error, DateTime time)
            {
                this.Error = error;
                this.Time = time;
            }
        }

        /// <summary>
        /// 向日志添加一条错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public static void Write(string message, Exception error)
        {
            Write(new Log(new Exception(message + error.ToString())));
        }
        /// <summary>
        /// 向日志添加一条错误
        /// </summary>
        /// <param name="error">错误对象</param>
        public static void Write(Exception error)
        {
            Write(new Log(error));
        }
        /// <summary>
        /// 向日志添加一条错误
        /// </summary>
        /// <param name="log"></param>
        public static void Write(Log log)
        {
            //MessageBox.Show("添加了一条错误："+log.Message);
            GlobalObject.GetUIForm().AddStatusMessage(log.ShortMessage);
            //_logFile.WriteLineAsync(log.Message);
        }
    }
}