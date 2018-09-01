using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle
{
    /// <summary>
    /// 实现HTTP的GET、POST
    /// </summary>
    public static class HTTP
    {
        /// <summary>
        /// 获取代理
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="isUseCredentials"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static WebProxy GetProxy(string host, int port, bool isUseCredentials = false, string username = "", string password = "")
        {
            // 设置代理
            var proxy = new WebProxy(host, port);

            if (isUseCredentials)
            {
                proxy.Credentials = new NetworkCredential(username, password);
                proxy.UseDefaultCredentials = true;
            }
            return proxy;
        }

        /// <summary>
        /// 将下载的数据保存到文件
        /// </summary>
        /// <param name="streamTask">请求地址</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public async static Task<bool> StreamToFile(Task<Stream> streamTask, string filePath, long speed = 1048576)
        {
            var task = new Task<bool>(() =>
            {
                try
                {
                    var stream = streamTask.Result;
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        byte[] buffer = new byte[speed];//这里的大小可以设置

                        var size = stream.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            buffer.Initialize();//将buffer初始化，避免出现后半部不更新的情况。
                            fileStream.Write(buffer, 0, size);
                            size = stream.Read(buffer, 0, buffer.Length);
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    //throw new Exception($"下载文件失败。{e.Message}");
                    return false;
                }
            });
            task.Start();
            return await task;
        }
        /// <summary>
        /// 将下载的数据保存到文件
        /// </summary>
        /// <param name="stream">请求地址</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="speed">断点传输速度，默认1Mb</param>
        /// <returns></returns>
        public async static Task<bool> StreamToFile(Stream stream, string filePath, long speed = 1048576)
        {
            var task = new Task<bool>(() =>
            {
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[speed];//这里的大小可以设置

                        var size = stream.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            buffer.Initialize();//将buffer初始化，避免出现后半部不更新的情况。
                            fileStream.Write(buffer, 0, size);
                            size = stream.Read(buffer, 0, buffer.Length);
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    //throw new Exception($"下载文件失败。{e.Message}");
                    return false;
                }
            });
            task.Start();
            return await task;
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">头标识</param>
        /// <param name="overTime">超时时间</param>
        /// <param name="proxy">代理</param>
        /// <returns></returns>
        public async static Task<Stream> GET(string url, Dictionary<string, string> headers = null, int overTime = -1, WebProxy proxy = null)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            // 添加头
            if (headers != null)
                foreach (var herder in headers)
                    request.Headers.Add(herder.Key, herder.Value);

            // 设置超时时间
            if (overTime != -1) request.Timeout = overTime;

            // 设置代理
            if (proxy != null) request.Proxy = proxy;

            try
            {
                var response = await request.GetResponseAsync() as HttpWebResponse;
                return response.GetResponseStream();
            }
            catch (Exception e)
            {
                throw new Exception($"GET请求失败。{e.Message}");
            }
        }
        /// <summary>
        /// 发送GET请求，并返回指定编码的文本
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">头标识</param>
        /// <param name="overTime">超时时间</param>
        /// <param name="proxy">代理</param>
        /// <returns></returns>
        public async static Task<string> GETtoString(string url, Dictionary<string, string> headers = null, int overTime = -1, WebProxy proxy = null, Encoding encoding = null)
        {
            using (var reader = new StreamReader(await GET(url, headers, overTime, proxy), encoding ?? Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }

            //    var content = string.Empty;
            //    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            //    {
            //        content = await reader.ReadToEndAsync();
            //    }
            //    return content;
            //}
            //catch (Exception e)
            //{
            //    OperIO.WriteLog($"GET请求失败。{e.Message}");
            //    return "";
            //}
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="overTime"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public async static Task<Stream> POST(string url, Dictionary<string, string> headers, string data, string contentType, int overTime = -1, WebProxy proxy = null)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";

            // 添加头
            foreach (var herder in headers)
                request.Headers.Add(herder.Key, herder.Value);

            // Post数据
            var byteData = Encoding.UTF8.GetBytes(data);
            request.ContentType = contentType;
            request.ContentLength = byteData.LongLength;

            // 设置超时时间
            if (overTime != -1) request.Timeout = overTime;

            // 设置代理
            if (proxy != null) request.Proxy = proxy;

            try
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }

                var response = await request.GetResponseAsync() as HttpWebResponse;
                return response.GetResponseStream();
            }
            catch (Exception e)
            {
                throw new Exception($"POST请求失败。{e.Message}");
            }
        }
        /// <summary>
        /// 发送POST请求，并返回指定编码的文本
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">头标识</param>
        /// <param name="data">POST数据</param>
        /// <param name="contentType">媒体类型</param>
        /// <param name="overTime">超时时间</param>
        /// <param name="proxy">代理</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public async static Task<string> POSTtoString(string url, Dictionary<string, string> headers, string data, string contentType, int overTime = -1, WebProxy proxy = null, Encoding encoding = null)
        {
            using (var reader = new StreamReader(await POST(url, headers, data, contentType, overTime, proxy), encoding ?? Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
