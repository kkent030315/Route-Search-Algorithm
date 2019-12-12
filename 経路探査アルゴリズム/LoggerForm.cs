using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 経路探査アルゴリズム
{
    public partial class LoggerForm : Form
    {
        public LoggerForm()
        {
            InitializeComponent();
        }

        public static void WriteLine(string text)
        {
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectedText = "[" + System.DateTime.Now.ToShortTimeString() + "]" + " " + text + "\r\n";
        }

        public static void WriteLine(string text, Color color)
        {
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = color;
            richTextBox1.SelectedText = "[" + System.DateTime.Now.ToShortTimeString() + "]" + " " + text + "\r\n";
        }

        public static void WriteSuccess(string text)
        {
            WriteLine(text, Color.Lime);
        }

        public static void WriteError(string text)
        {
            WriteLine("[Error] "+text, Color.Red);
        }

        public static void WriteWarn(string text)
        {
            WriteLine("[Warning] "+text, Color.Orange);
        }

        public static void WriteInfo(string text)
        {
            WriteLine("[INFO] "+text, Color.LightBlue);
        }
    }
}
