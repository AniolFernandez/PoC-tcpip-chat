using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
            textOutput.ScrollBars = ScrollBars.Vertical;
            textOutput.WordWrap = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.SendString(textInput.Text);
            textOutput.Text += Environment.NewLine+"Missatge enviat: " + textInput.Text;
            if (textInput.Text.Equals("prou"))
                this.Close();
            textInput.Text = "";
        }
    }
}
