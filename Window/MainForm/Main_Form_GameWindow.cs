using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using NokiKanColle.Utility;
using NokiKanColle.Function;


namespace NokiKanColle.Window
{
    class Main_Form_GameWindow
    {
    }
    public partial class Main_Form : Form
    {
        private void GameWindow_Initialization()
        {

        }
        private void GameWindow_ReadPlacement(string strFilePath)
        {
            GlobalObject.ClickOffsetXY = new Point(
                Convert.ToInt32(OperINI.ReadIni("游戏窗口", "手动偏移X", 0.ToString(), strFilePath)),
                Convert.ToInt32(OperINI.ReadIni("游戏窗口", "手动偏移Y", 0.ToString(), strFilePath)));
        }
        private void GameWindow_WritePlacement(string strFilePath)
        {
            OperINI.WriteIni("游戏窗口", "手动偏移X", GlobalObject.ClickOffsetXY.X.ToString(), strFilePath);
            OperINI.WriteIni("游戏窗口", "手动偏移Y", GlobalObject.ClickOffsetXY.Y.ToString(), strFilePath);
        }

        //窗口句柄相关成员
        /// <summary>
        /// 自动后台模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_AutoHandle_button_Click(object sender, EventArgs e)
        {
            //自动查找游戏句柄
            int SH = Screen.PrimaryScreen.Bounds.Height;//屏幕分辨率高度
            int SW = Screen.PrimaryScreen.Bounds.Width;//屏幕分辨率宽度
            using (GameHandleClass SeekHandle = new GameHandleClass())
            {
                SeekHandle.Mode = FunctionHandle.MODE.Handle;
                Point XY = new Point(0, 0);
                do
                {
                    SeekHandle.Handle = FunctionHandle.WindowFromPoint(XY);
                    if (XY.X > SW)
                    {
                        XY.X = 0;
                        XY.Y += 100;
                    }
                    else
                        XY.X += 100;
                    if ((XY.X > SW) && (XY.Y > SH))
                    {
                        this.GameWindow_Message_textBox.Text = "自动后台模式抓取失败╭இɷஇ╮";
                        this.GameWindow_Message_textBox.BackColor = Color.Red;
                        GlobalObject.GameHandle = new GameHandleClass();
                        RefreshGameWindow_pictureBox();
                        return;
                    }
                } while (!SeekHandle.IsWin_800x480());

                GlobalObject.GameHandle = new GameHandleClass(SeekHandle);
                GlobalObject.GameHandle.Mode = FunctionHandle.MODE.Handle;// 更改模式
                RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);// 刷新树状图
                this.GameWindow_Message_textBox.Text = "后台模式抓取成功 句柄：" + GlobalObject.GameHandle.Handle.ToString();
                this.GameWindow_Message_textBox.BackColor = Color.Lime;
                RefreshGameWindow_pictureBox();
            }
        }
        /// <summary>
        /// Chorme后台模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_Handle_button_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (GameWindow_IsHandleClick)
            {
                GameWindow_IsHandleClick = false;
                return;
            }
            //手动查找游戏句柄
            using (GameHandleClass SeekHandle = new GameHandleClass())
            {
                SeekHandle.Handle = FunctionHandle.GetMouseHandle();
                SeekHandle.Mode = FunctionHandle.MODE.Handle;

                if (SeekHandle.IsWin_800x480())
                {
                    GlobalObject.GameHandle = new GameHandleClass(SeekHandle)
                    {
                        Mode = FunctionHandle.MODE.Handle
                    };
                    RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);
                    this.GameWindow_Message_textBox.Text = "后台模式抓取成功 句柄：" + GlobalObject.GameHandle.Handle.ToString();
                    this.GameWindow_Message_textBox.BackColor = Color.Lime;
                    RefreshGameWindow_pictureBox();
                    return;
                }
                else//chorme模式
                {
                    SeekHandle.Mode = FunctionHandle.MODE.Handle;
                    Point xy = new Point();
                    if (FunctionJudge.IsGameInWin(SeekHandle.GetGameBitmap(), out xy))
                    {
                        GlobalObject.GameHandle = new GameHandleClass(SeekHandle)
                        {
                            Mode = FunctionHandle.MODE.Chorme,
                            XY = xy
                        };
                        RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);
                        this.GameWindow_Message_textBox.Text = "后台模式抓取成功 chorme句柄：" + GlobalObject.GameHandle.Handle.ToString();
                        this.GameWindow_Message_textBox.BackColor = Color.Lime;
                        RefreshGameWindow_pictureBox();
                    }
                    else
                    {
                        // 尝试根父句柄的抓取
                        SeekHandle.Handle = FunctionHandle.GetTopFatherHandle(SeekHandle.Handle);
                        SeekHandle.Mode = FunctionHandle.MODE.Handle;

                        if (FunctionJudge.IsGameInWin(SeekHandle.GetGameBitmap(), out xy))
                        {
                            GlobalObject.GameHandle = new GameHandleClass(SeekHandle)
                            {
                                Mode = FunctionHandle.MODE.Chorme,
                                XY = xy
                            };
                            RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);
                            this.GameWindow_Message_textBox.Text = "后台模式抓取成功 chorme句柄：" + GlobalObject.GameHandle.Handle.ToString();
                            this.GameWindow_Message_textBox.BackColor = Color.Lime;
                            RefreshGameWindow_pictureBox();
                        }
                        else
                        {
                            RefreshGameWindow_treeView(SeekHandle.Handle);
                            this.GameWindow_Message_textBox.Text = "后台模式抓取失败 句柄：" + SeekHandle.Handle.ToString();
                            this.GameWindow_Message_textBox.BackColor = Color.Red;
                            GlobalObject.GameHandle = new GameHandleClass();
                            RefreshGameWindow_pictureBox();
                        }
                    }
                }
            }
        }
        private bool GameWindow_IsHandleClick = false;
        /// <summary>
        /// Chorme指定句柄模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_Handle_button_Click(object sender, EventArgs e)
        {
            GameWindow_IsHandleClick = true;
            using (GameHandleClass SeekHandle = new GameHandleClass())
            {
                try
                {
                    this.GameWindow_InputIntPtr_textBox.BackColor = Color.White;
                    var result = int.Parse(this.GameWindow_InputIntPtr_textBox.Text);
                    SeekHandle.Handle = new IntPtr(result);
                }
                catch (Exception)
                {
                    this.GameWindow_InputIntPtr_textBox.BackColor = Color.Red;
                }
                SeekHandle.Mode = FunctionHandle.MODE.Handle;

                Point xy = new Point();
                if (FunctionJudge.IsGameInWin(SeekHandle.GetGameBitmap(), out xy))
                {
                    GlobalObject.GameHandle = new GameHandleClass(SeekHandle)
                    {
                        Mode = FunctionHandle.MODE.Chorme,
                        XY = xy
                    };
                    RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);
                    this.GameWindow_Message_textBox.Text = "后台模式抓取成功 chorme句柄：" + GlobalObject.GameHandle.Handle.ToString();
                    this.GameWindow_Message_textBox.BackColor = Color.Lime;
                    RefreshGameWindow_pictureBox();
                }
                else
                {
                    RefreshGameWindow_treeView(SeekHandle.Handle);
                    this.GameWindow_Message_textBox.Text = "后台模式抓取失败 句柄：" + SeekHandle.Handle.ToString();
                    this.GameWindow_Message_textBox.BackColor = Color.Red;
                    GlobalObject.GameHandle = new GameHandleClass();
                    RefreshGameWindow_pictureBox();
                }
            }
        }
        /// <summary>
        /// 抓取前台模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_Desktop_button_Click(object sender, EventArgs e)
        {
            GlobalObject.GameHandle = new GameHandleClass();
            int SH = Screen.PrimaryScreen.Bounds.Height;//屏幕分辨率高度
            int SW = Screen.PrimaryScreen.Bounds.Width;//屏幕分辨率宽度
            Point xy = new Point();
            using (var bmp = FunctionBitmap.CopyWindow())
            {
                if (FunctionJudge.IsGameInWin(bmp, out xy))
                {
                    GlobalObject.GameHandle = new GameHandleClass();
                    GlobalObject.GameHandle.Handle = IntPtr.Zero;
                    GlobalObject.GameHandle.Mode = FunctionHandle.MODE.Desktop;
                    GlobalObject.GameHandle.XY = xy;
                    this.GameWindow_Message_textBox.Text = "前台模式抓取成功";
                    this.GameWindow_Message_textBox.BackColor = Color.Lime;
                    RefreshGameWindow_pictureBox();
                }
                else
                {
                    GlobalObject.GameHandle = new GameHandleClass();
                    this.GameWindow_Message_textBox.Text = "前台模式抓取失败";
                    this.GameWindow_Message_textBox.BackColor = Color.Red;
                    RefreshGameWindow_pictureBox();
                }
            }
        }
        /// <summary>
        /// 手动后台模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 13) return;
            else
            {
                //指定窗口句柄查找游戏窗口
                using (GameHandleClass SeekHandle = new GameHandleClass())
                {
                    SeekHandle.Handle = (IntPtr)Convert.ToInt32(GameWindow_HandleList_treeView.SelectedNode.Text);
                    SeekHandle.Mode = FunctionHandle.MODE.Handle;
                    Point xy = new Point();
                    if (FunctionJudge.IsGameInWin(SeekHandle.GetGameBitmap(), out xy))
                    {
                        GlobalObject.GameHandle = new GameHandleClass(SeekHandle)
                        {
                            Mode = FunctionHandle.MODE.Chorme,
                            XY = xy
                        };
                        RefreshGameWindow_treeView(GlobalObject.GameHandle.Handle);
                        this.GameWindow_Message_textBox.Text = "后台模式抓取成功 chorme句柄：" + GlobalObject.GameHandle.Handle.ToString();
                        this.GameWindow_Message_textBox.BackColor = Color.Lime;
                        RefreshGameWindow_pictureBox();
                    }
                    else
                    {
                        RefreshGameWindow_treeView(SeekHandle.Handle);
                        this.GameWindow_Message_textBox.Text = "后台模式抓取失败 句柄：" + SeekHandle.Handle.ToString();
                        this.GameWindow_Message_textBox.BackColor = Color.Red;
                        GlobalObject.GameHandle = new GameHandleClass();
                        RefreshGameWindow_pictureBox();
                    }
                }
            }
        }


        /// <summary>
        /// 点击图像刷新游戏缩略图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_pictureBox_Click(object sender, EventArgs e)
        {
            using (var temp = RefreshGameWindow_pictureBox())
            {
                if (temp != null)
                {
                    Clipboard.Clear();
                    Clipboard.SetImage(temp);
                }
            }
        }
        /// <summary>
        /// 刷新游戏缩略图
        /// </summary>
        private Bitmap RefreshGameWindow_pictureBox()
        {
            if (GameWindow_GamePicture_pictureBox.Image != null)
            {
                Image i = GameWindow_GamePicture_pictureBox.Image;
                GameWindow_GamePicture_pictureBox.Image = null;
                i.Dispose();
            }
            try
            {
                if (GlobalObject.GameHandle.IsSuccess)
                {
                    var temp = GlobalObject.GameHandle.GetGameBitmap();
                    GameWindow_GamePicture_pictureBox.Image = FunctionBitmap.ResizeImage(temp, 200, 120, false);
                    return temp;
                }
                else
                    GameWindow_GamePicture_pictureBox.Image = FunctionBitmap.ResizeImage(Properties.Resources.CrawlFailed, 200, 120, true);
            }
            catch (Exception)
            {
                /***************
                在这里加入失败图像
                ***************/
                if (GameWindow_GamePicture_pictureBox.Image != null)
                {
                    Image i = GameWindow_GamePicture_pictureBox.Image;
                    GameWindow_GamePicture_pictureBox.Image = FunctionBitmap.ResizeImage(Properties.Resources.CrawlFailed, 200, 120, true);
                    i.Dispose();
                }
            }
            return null;
        }


        /// <summary>
        /// 刷新游戏句柄树状图
        /// </summary>
        /// <param name="str">句柄</param>
        private void RefreshGameWindow_treeView(IntPtr str)
        {
            TreeNode FatherGameHandle = new TreeNode(FunctionHandle.GetTopFatherHandle(str).ToString());
            //fHandle.GetTopFatherHandle(GameHandle.Handle).ToString()
            GetAllChildrenHandles(ref FatherGameHandle);
            this.GameWindow_HandleList_treeView.Nodes.Clear();
            this.GameWindow_HandleList_treeView.Nodes.Add(FatherGameHandle);
            //选中节点
            foreach (TreeNode item in GameWindow_HandleList_treeView.Nodes)
            {
                ErgodicTreeView(item, str.ToString());
            }
        }
        /// <summary>
        /// 点击树状图上的某一节点，刷新图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_treeView_Click(object sender, EventArgs e)
        {
            if (GameWindow_HandleList_treeView.SelectedNode != null)
            {
                // 将当前节点文本添加到文本框
                this.GameWindow_InputIntPtr_textBox.Text = GameWindow_HandleList_treeView.SelectedNode.Text;
                if (GameWindow_GamePicture_pictureBox.Image != null)
                {
                    Image i = GameWindow_GamePicture_pictureBox.Image;
                    GameWindow_GamePicture_pictureBox.Image = null;
                    i.Dispose();
                }
                try
                {
                    GameWindow_GamePicture_pictureBox.Image = FunctionBitmap.ResizeImage(FunctionBitmap.PrtGameWindow(Function.Functions.StrToIntPtr(GameWindow_HandleList_treeView.SelectedNode.Text)), 200, 120, true);
                }
                catch (Exception)
                {
                    /***************
                    在这里加入失败图像
                    ***************/
                }
            }
        }
        /// <summary>
        /// 获取所有子窗口及子窗口下的子窗口句柄
        /// </summary>
        /// <param name="Father"></param>
        /// <returns></returns>
        private bool GetAllChildrenHandles(ref TreeNode Father)
        {
            Father.Nodes.Clear();
            bool IsEnd = GetChildrenHandles(ref Father);
            if (IsEnd == false) return false;//若该父窗口下没有子窗口

            TreeNode Child = new TreeNode();
            for (int i = 0; i < Father.Nodes.Count; i++)
            {
                Child = Father.Nodes[i];
                IsEnd = GetAllChildrenHandles(ref Child);
                if (!IsEnd) continue;
                else Father.Nodes[i] = Child;
            }
            return true;


        }//*/
         /// <summary>
         /// 获取所有子窗口句柄
         /// </summary>
         /// <param name="Father"></param>
         /// <returns></returns>
        private bool GetChildrenHandles(ref TreeNode Father)
        {
            Father.Nodes.Clear();
            string F = Father.Text;
            string C = FunctionHandle.FindChildHandle(Father.Text, "0");//查找第一个子句柄
            if (C == "0") return false;//当前句柄下没有子句柄
            Father.Nodes.Add(C);

            while (C != "0")
            {
                C = FunctionHandle.FindChildHandle(F, C);//查找下一个子句柄
                if (C != "0")//找到了子句柄
                {
                    Father.Nodes.Add(C);
                    continue;
                }
                else return true;//已遍历所有子句柄
            }

            return false;//error
        }
        /// <summary>
        /// 选中节点
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="str"></param>
        private void ErgodicTreeView(TreeNode tn, string str)
        {
            if (tn == null) return;
            if (tn.Text.Equals(str))//查找到某节点时
            {
                //遍历递归获取父节点，将父节点全部展开
                Prenode(tn);
                GameWindow_HandleList_treeView.SelectedNode = tn;
            }
            foreach (TreeNode item in tn.Nodes)
            {
                ErgodicTreeView(item, str);
            }
        }
        private void Prenode(TreeNode m)
        {
            try
            {
                if (m.Parent.Text != null)
                {
                    m.Parent.Expand();
                    //当为项级节点时
                    if (m.Parent.Level == 0)
                    {
                        m.Parent.Expand();
                    }
                    //不是项级节点时
                    else
                    { Prenode(m.Parent); }
                }
            }
            catch (Exception)
            { return; }
        }


        public SetXYOffset SetXYOffsetWindow = new SetXYOffset();
        /// <summary>
        /// 手动偏移坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_XYOffset_button_Click(object sender, EventArgs e)
        {
            if (SetXYOffsetWindow.IsDisposed)
            {
                SetXYOffsetWindow = new SetXYOffset();
            }
            SetXYOffsetWindow.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            SetXYOffsetWindow.SetTextXY(GlobalObject.ClickOffsetXY);
            SetXYOffsetWindow.Show();
        }

        /// <summary>
        /// 抓取说明
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameWindow_ClickTest_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本程序有三种抓取游戏窗口的方式：\n" +
                "   1.后台模式\n" +
                "   2.前台模式\n" +
                "   3.Chorme模式\n" +
                "\n" +
                "在抓取前请确认游戏窗口为100%大小（分辨率为800x480）\n" +
                "\n" +
                "推荐使用74eo浏览器配合此脚本一起使用！\n" +
                " * 使用74eo或KCV的童鞋只需点击一下后台即可。\n" +
                " * 使用Poi浏览器的童鞋需要使用Win7兼容模式打开Poi，并保持游戏界面为母港状态（母港右上角不能有远征回归的提示）。然后点住Chorme按钮按住不放，拖拽到Poi浏览器上，尝试将节点树给出的句柄输入文本框内后点击Chrome按钮（不保证100%成功）。根据情况自行设置手动偏移（若依旧无法成功点击请好♂自♂为♂之）\n" +
                " * 使用网页或其他浏览器的请尝试前台模式（保持游戏界面为母港状态，母港右上角不能有远征回归的提示）\n" +
                "\n" +
                "Lua 脚本文件必须放在程序根目录下的 LuaScript 文件夹内才可以被执行。");
        }
    }

    /// <summary>
    /// 设置坐标偏移窗口
    /// </summary>
    public class SetXYOffset : Form
    {
        internal TextBox SetX;
        private Label label1;
        private Label label2;
        internal TextBox SetY;

        public SetXYOffset()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
        }

        private void InitializeComponent()
        {
            this.SetX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SetY = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SetX
            // 
            this.SetX.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SetX.Location = new System.Drawing.Point(44, 12);
            this.SetX.MaxLength = 4;
            this.SetX.Name = "SetX";
            this.SetX.Size = new System.Drawing.Size(64, 23);
            this.SetX.TabIndex = 0;
            this.SetX.Text = "0";
            this.SetX.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "偏移X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "偏移Y";
            // 
            // SetY
            // 
            this.SetY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SetY.Location = new System.Drawing.Point(44, 45);
            this.SetY.MaxLength = 4;
            this.SetY.Name = "SetY";
            this.SetY.Size = new System.Drawing.Size(64, 23);
            this.SetY.TabIndex = 2;
            this.SetY.Text = "0";
            this.SetY.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // SetXYOffset
            // 
            this.ClientSize = new System.Drawing.Size(120, 77);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SetY);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SetX);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetXYOffset";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void SetClickOffsetXY()
        {
            try
            {
                var temp = new Point(Convert.ToInt32(SetX.Text), Convert.ToInt32(SetY.Text));
                if (GlobalObject.ClickOffsetXY != temp)
                {
                    GlobalObject.ClickOffsetXY = temp;
                }
            }
            catch (Exception)
            {
                this.SetX.Text = GlobalObject.ClickOffsetXY.X.ToString();
                this.SetY.Text = GlobalObject.ClickOffsetXY.Y.ToString();
            }
        }
        public void SetTextXY(Point xy)
        {
            this.SetX.Text = xy.X.ToString();
            this.SetY.Text = xy.Y.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SetX.Text = Regex.Replace(SetX.Text, @"[^\d-]*", "");
            if (SetX.Text == "")
                SetX.Text = "0";
            SetClickOffsetXY();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SetY.Text = Regex.Replace(SetY.Text, @"[^\d-]*", "");
            if (SetY.Text == "")
                SetY.Text = "0";
            SetClickOffsetXY();
        }
    }
}
