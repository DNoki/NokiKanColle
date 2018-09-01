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

using NokiKanColle.Function;

namespace NokiKanColle.Window
{
    class Main_Form_GameLuaScript
    {
    }
    public partial class Main_Form : Form
    {
        /// <summary>
        /// 添加一个 lua 脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLuaScript_ThreadAddLuaScript_button_Click(object sender, EventArgs e)
        {
            GameLuaScript_ThreadAddLuaScript_openFileDialog.InitialDirectory = Application.StartupPath + $@"\LuaScript";
            var result = GameLuaScript_ThreadAddLuaScript_openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //var fileAddress = GameStatus_ThreadAddLuaScript_openFileDialog.FileName;
                var fileName = GameLuaScript_ThreadAddLuaScript_openFileDialog.SafeFileName.Replace(".lua", "");
                new Utility.Process.LuaScript(fileName);
                //new Utility.Process.LuaScript("test");
            }
            
        }

        /// <summary>
        /// 结束Lua脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLuaScript_Stop_button_Click(object sender, EventArgs e)
        {
            Function.FunctionThread.CloseThread("Lua脚本");
        }
    }
}
