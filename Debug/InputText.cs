using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace NokiKanColle.Debug
{
    public partial class InputText : Form
    {
        public InputText()
        {
            InitializeComponent();
        }

        private static InputText inputTextForm = null;
        private static AutoResetEvent are = new AutoResetEvent(false);
        private static bool isPass = false;

        public static string ShowInputText(Form owner, string presentationMessage)
        {
            while (true)
            {
                Thread thread = new Thread(StartForm);
                var obj = (owner, presentationMessage);
                thread.Start(obj);
                are.WaitOne();
                if (isPass)
                {
                    var result = inputTextForm.Debug_InputText_textBox.Text;
                    CloseForm();
                    return result;
                }
                CloseForm();
            }

        }
        private static void StartForm(object obj)
        {
            var owner = (((Form, string))obj).Item1;
            var message = (((Form, string))obj).Item2;
            if (owner.InvokeRequired)
            {
                owner.Invoke(new Action<object>(StartForm), obj);
            }
            else
            {
                inputTextForm = new InputText();
                inputTextForm.Text = message;
                inputTextForm.Show(owner);
            }
        }
        private static void CloseForm()
        {
            if (inputTextForm.InvokeRequired)
                inputTextForm.Invoke(new Action(CloseForm));
            else
            {
                inputTextForm.Dispose();
                inputTextForm.Close();
            }
        }


        private void Debug_Yes_button_Click(object sender, EventArgs e)
        {
            isPass = true;
            are.Set();
        }

        private void Debug_No_button_Click(object sender, EventArgs e)
        {
            isPass = false;
            are.Set();
        }

        private void InputText_FormClosing(object sender, FormClosingEventArgs e)
        {
            isPass = false;
            are.Set();
        }
    }
}
