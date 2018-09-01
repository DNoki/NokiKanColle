using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NokiKanColle.Utility
{
    /// <summary>
    /// 软件许可类
    /// </summary>
    public class Permit
    {
//#warning 到期时间需要被更改
        ///// <summary>
        ///// 到期时间
        ///// </summary>
        //private readonly dynamic _expiration = new DateTime(2018, 10, 1);
        /// <summary>
        /// 软件使用许可
        /// </summary>
        private dynamic permit = false;
        /// <summary>
        /// 获取软件使用许可
        /// </summary>
        public bool GetPermit { get { return permit; } }
        ///// <summary>
        ///// 获取软件到期时间
        ///// </summary>
        //public DateTime Expiration { get { return _expiration; } }
        /// <summary>
        /// 当前程序版本号
        /// </summary>
        public Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        ///// <summary>
        ///// 将时间戳转化为DateTime
        ///// </summary>
        ///// <param name="timeStamp">时间戳（10位数）</param>
        ///// <returns>DateTime</returns>
        //public DateTime TimestampToTime(string timeStamp)
        //{
        //    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //    long lTime = long.Parse(timeStamp + "0000000");
        //    TimeSpan toNow = new TimeSpan(lTime);
        //    return dtStart.Add(toNow);
        //}

        /// <summary>
        /// 获取最新版本号（简书）
        /// </summary>
        /// <returns></returns>
        public Version GetVersionForJianshu()
        {
            var task = new Task<string>(() => HTTP.GETtoString("https://www.jianshu.com/p/d2d10a83ca77").Result);
            task.Start();
            var result = Regex.Match(task.Result, "-NokiKanColle Version:.+?-").Value;
            if (string.IsNullOrEmpty(result)) throw new Exception("数据为空。");
            return Version.Parse(Regex.Match(result, "[0-9.]+").Value);
        }
        /// <summary>
        /// 获取最新版本号（GitHub）
        /// </summary>
        /// <returns></returns>
        public Version GetVersionForGitHub()
        {
            var task = new Task<string>(() => HTTP.GETtoString("https://github.com/DNoki/NokiWorkConfiguration/blob/master/README.md").Result);
            task.Start();
            var result = Regex.Match(task.Result, "\"NokiKanColle Version:.+?\"").Value;
            if (string.IsNullOrEmpty(result)) throw new Exception("数据为空。");
            return Version.Parse(Regex.Match(result, "[0-9.]+").Value);
        }
        //public bool SecurityForGitHub()
        //{
        //    try
        //    {
        //        var task = new Task<string>(() =>
        //        {
        //            var text = HTTP.GETtoString("https://github.com/DNoki/NokiWorkConfiguration/blob/master/README.md").Result;

        //            return Regex.Match(text, "\"NokiKanColle Version:.+?\"").Value;
        //        });
        //        task.Start();
        //        var result = task.Result;
        //        if (!string.IsNullOrEmpty(task.Result))
        //        {
        //            var v = Regex.Match(result.Replace(".", ""), "[0-9]+").Value;
        //            var nv = Regex.Match(this.Version.Replace(".", ""), "[0-9]+").Value;

        //            if (Convert.ToInt32(v) <= Convert.ToInt32(nv))
        //            {
        //                permit = true;
        //                return true;
        //            }
        //        }
        //        MessageBox.Show("已检测到新的版本，请重新下载。");
        //        permit = false;
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        permit = false;
        //        throw new Exception("版本检测失败，请检查网络。");
        //        //MessageBox.Show("版本检测失败，请检查网络。");                
        //        //return false;
        //    }

        //}
        /// <summary>
        /// 从网络获取时间并判断是否过期
        /// </summary>
        /// <returns></returns>
        public bool Security(int mode)
        {
            try
            {
                var newVersion = mode == 1 ? GetVersionForJianshu() : GetVersionForGitHub();
                if (this.Version >= newVersion)
                    this.permit = true;
                else
                {
                    this.permit = false;
                    MessageBox.Show($"已检测到新的版本：V.{newVersion.ToString()}，请重新下载。");
                }
            }
            catch (Exception)
            {
                this.permit = false;
                MessageBox.Show($"认证失败，无法获取最新版本号，请检查网络。");
            }
            return this.GetPermit;
            //try
            //{
            //    return SecurityForGitHub();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("版本认证失败，将采用旧版认证法");
            //}

            //DateTime nowTime = GetHttpTime();
            //if (nowTime == DateTime.MaxValue)
            //{
            //    nowTime = GetServerTime();
            //    if (nowTime == DateTime.MaxValue)
            //    {
            //        MessageBox.Show("激活失败，无法正确获取网络时间！请检查网络连接。");
            //        _permit = false;
            //        return false;
            //    }
            //}
            //if (nowTime < this.Expiration)
            //{
            //    _permit = true;
            //    return true;
            //}
            //else
            //{
            //    MessageBox.Show("激活失败，软件已于 " + Expiration.ToLongDateString().ToString() + " 过期，请到群共享下载新版本。");
            //    _permit = false;
            //    return false;
            //}
        }

        ///// <summary>
        ///// 从服务器获取网络时间
        ///// </summary>
        ///// <returns>网络时间</returns>
        //public DateTime GetServerTime()
        //{
        //    // 客户端连接到服务器主机获取时间
        //    string[] timeServers = new string[]
        //    {
        //        "time-c.timefreq.bldrdoc.gov",// 美国 国家科技标准研究所
        //        "utcnist.colorado.edu",// 美国 科罗拉多大学Boulder校区
        //        "time-b.timefreq.bldrdoc.gov",// 美国 国家科技标准研究所
        //        //"time-a.nist.gov",// 美国
        //        //"time-nw.nist.gov",// 美国 Microsoft公司
        //        //"time.windows.com"// 美国 Microsoft公司
        //    };
        //    foreach (var address in timeServers)
        //    {
        //        try
        //        {
        //            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //            socket.ReceiveTimeout = 5000;// 超时5秒
        //            socket.Connect(address, 13);// 链接到网络主机
        //            if (!socket.Connected)// 若链接失败则执行下个链接
        //            {
        //                socket.Close();
        //                socket.Dispose();
        //                continue;
        //            }
        //            byte[] bytes = new byte[1024];// 存储位置并设定上限为1024
        //            int ReadedBytesLength = socket.Receive(bytes, 0, 1024, SocketFlags.None);// 读取的字节数
        //            socket.Close();
        //            socket.Dispose();
        //            string text = System.Text.Encoding.ASCII.GetString(bytes, 0, ReadedBytesLength);

        //            if (text.Contains("UTC"))
        //            {
        //                var str = text.Split(' ');
        //                return DateTime.Parse(str[1] + " " + str[2]);
        //            }
        //        }
        //        catch (Exception)
        //        { continue; }
        //    }
        //    return DateTime.MaxValue;
        //}
        ///// <summary>
        ///// 从网页获取网络时间
        ///// </summary>
        ///// <returns></returns>
        //public DateTime GetHttpTime()
        //{
        //    string[] timeHttp = new string[]
        //    {
        //        "http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1",// 香港 和记环球电讯
        //        "http://www.thetimenow.com/clock/gmt/greenwich_mean_time",// 美国 佛罗里达州坦帕市Noc4hosts公司
        //        //"http://shijian.duoshitong.com/time.php",// 广东省广州市海珠区 腾讯云服务器(广州市海珠区新港中路397
        //    };
        //    for (int i = 0; i < timeHttp.Length; i++)
        //    {
        //        try
        //        {
        //            WebRequest wrt = WebRequest.Create(timeHttp[i]);
        //            wrt.Timeout = 10000;
        //            wrt.Credentials = CredentialCache.DefaultCredentials;
        //            WebResponse wrp = wrt.GetResponse();
        //            StreamReader sr = new StreamReader(wrp.GetResponseStream(), Encoding.UTF8);
        //            string text = sr.ReadToEnd();
        //            sr.Close();
        //            wrp.Close();
        //            if (i == 0 && text.IndexOf("0=") == 0)
        //                return TimestampToTime(text.Replace("0=", "").Substring(0, 10)).ToUniversalTime();
        //            else if (i == 1 && text.IndexOf("GMT\":[") == 3060)
        //            {
        //                string[] str = text.Substring(3060 + 6, 43).Replace("\"", "").Split(',');
        //                int month;
        //                if (str[4] == "January") month = 1;
        //                else if (str[4] == "February") month = 2;
        //                else if (str[4] == "March") month = 3;
        //                else if (str[4] == "April") month = 4;
        //                else if (str[4] == "May") month = 5;
        //                else if (str[4] == "June") month = 6;
        //                else if (str[4] == "July") month = 7;
        //                else if (str[4] == "August") month = 8;
        //                else if (str[4] == "September") month = 9;
        //                else if (str[4] == "October") month = 10;
        //                else if (str[4] == "November") month = 11;
        //                else if (str[4] == "December") month = 12;
        //                else continue;
        //                return new DateTime(Convert.ToInt32(str[6]), month, Convert.ToInt32(str[5]),
        //                    Convert.ToInt32(str[0]), Convert.ToInt32(str[1]), Convert.ToInt32(str[2]));
        //            }
        //            else continue;
        //        }
        //        catch (Exception)
        //        { continue; }
        //    }
        //    return DateTime.MaxValue;
        //}

    }
}
