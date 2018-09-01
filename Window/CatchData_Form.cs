using NokiKanColle.Function;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NokiKanColle.Data;

namespace NokiKanColle.Window
{
    public partial class CatchData_Form : Form
    {
        public CatchData_Form()
        {
            InitializeComponent();
        }

        public DataJudge CatchData = new DataJudge("0,0 @ 000000", "CatchData");

        //载入图像
        private void CatchData_LoadPicture_Click(object sender, EventArgs e)
        {
            if (CatchData_GamePicture.Image != null)
            {
                using (Image i = CatchData_GamePicture.Image)
                {
                    CatchData_GamePicture.Image = null;
                }
            }
            try
            {
                CatchData_GamePicture.Image = GlobalObject.GameHandle.GetGameBitmap();
            }
            catch (Exception)
            {
                /***************
                在这里加入失败图像
                ***************/
            }
        }


        //当鼠标在界面上摁下时
        private void CatchData_GamePicture_MouseDown(object sender, MouseEventArgs e)
        {
            CatchData_CatchXY.Text = e.X.ToString() + "," + e.Y.ToString();
        }
        //当鼠标在界面上释放时
        private void CatchData_GamePicture_MouseUp(object sender, MouseEventArgs e)
        {
            string[] xy = CatchData_CatchXY.Text.Split(',');
            CatchData_CatchWH.Text = (e.X - Convert.ToInt32(xy[0]) + 1).ToString() + "," + (e.Y - Convert.ToInt32(xy[1]) + 1).ToString();
            
        }

        //当鼠标在界面上移动时
        private void CatchData_GamePicture_MouseMove(object sender, MouseEventArgs e)
        {
            CatchData_CurrentXY.Text = e.X.ToString() + "," + e.Y.ToString();
        }

        private void CatchData_GetData_Click(object sender, EventArgs e)
        {
            CatchData = new DataJudge("0,0 @ 000000", "CatchData");
            CatchData_GetDataShow.Text = "";
            try
            {
                CatchData.XYToStr = CatchData_CatchXY.Text;                
                CatchData_GetDataShow.Text += CatchData.XYToStr + " @ ";

                string[] wh = CatchData_CatchWH.Text.Split(',');                
                CatchData.Color = FunctionBitmap.GetColorData(new Bitmap(CatchData_GamePicture.Image), CatchData.XYToStr, Convert.ToInt32(wh[0]), Convert.ToInt32(wh[1]));
                CatchData_GetDataShow.Text += CatchData.Color;
            }
            catch (Exception)
            {
                CatchData_GetDataShow.Text = "颜色信息获取失败，请检查" + Environment.NewLine + "    1.是否成功获取游戏窗口" + Environment.NewLine + "    2.数值输入是否错误";
            }
            
        }
    }
}
