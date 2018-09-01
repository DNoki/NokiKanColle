using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Function
{
    public static class OperINI
    {
        /// <summary>
        /// 读取键值
        /// </summary>
        /// <param name="lpAppName">小节</param>
        /// <param name="lpKeyName">键名</param>
        /// <param name="lpDefault">默认键值</param>
        /// <param name="lpReturnedString">接收INI文件中的值</param>
        /// <param name="nSize">缓存器的大小</param>
        /// <param name="lpFileName">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(
       string lpAppName,
       string lpKeyName,
       string lpDefault,
       StringBuilder lpReturnedString,
       int nSize,
       string lpFileName);
        /// <summary>
        /// 写入键值
        /// </summary>
        /// <param name="lpAppName">小节</param>
        /// <param name="lpKeyName">键名</param>
        /// <param name="lpString">键值</param>
        /// <param name="lpFileName">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WritePrivateProfileString(string lpAppName,
            string lpKeyName, string lpString, string lpFileName);

        /// <summary>
        /// 读取键值
        /// </summary>
        /// <param name="section">小节</param>
        /// <param name="key">键</param>
        /// <param name="defValue">默认键值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns>读取的键值</returns>
        public static string ReadIni(string section, string key, string defValue, string filepath)
        {
            try
            {
                StringBuilder retValue = new StringBuilder(500);
                GetPrivateProfileString(section, key, defValue, retValue, 500, filepath);
                return retValue.ToString();
            }
            catch (Exception e)
            {
                FunctionExceptionLog.Write("INI文件读取失败！",e);
                return "";
            }
        }

        /// <summary>
        /// 写入键值
        /// </summary>
        /// <param name="section">小节</param>
        /// <param name="key">键</param>
        /// <param name="value">键值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>布尔值</returns>
        public static bool WriteIni(string section, string key, string value, string filePath)
        { return WritePrivateProfileString(section, key, value, filePath); }
        /// <summary>
        /// 删除节
        /// </summary>
        /// <param name="section">小节</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool DeleteSection(string section, string filePath)
        { return WritePrivateProfileString(section, null, null, filePath); }
        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="section">小节</param>
        /// <param name="key">键</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool DeleteKey(string section, string key, string filePath)
        { return WritePrivateProfileString(section, key, null, filePath); }

    }
}
