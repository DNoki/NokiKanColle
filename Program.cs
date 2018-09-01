using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NokiKanColle
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if true


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window.Main_Form());

#else
#warning 全局调试系统
            try
            {
                try
                {
                    /*
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Window.Main_Form());
                    */
                    throw new Exception("测试错误");
                }
                // 程序遇到不可处理异常，程序已崩溃
                catch (Exception e)
                {
                    MessageBox.Show($"已捕捉到错误：{e.ToString()}");
                    throw e;
                }
                finally
                {
                    MessageBox.Show("finally 语句已被执行");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show($"二级Catch已捕捉到错误");
            }
            MessageBox.Show("程序已结束");//*/
#endif
            }

    }




}
