using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServerWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Server.Protocol _prol;
        private void Form1_Load(object sender, EventArgs e)
        {
            Deflate.cs_head = Settings.Default.cs_head;
            _prol = Server.Protocol.Create(Settings.Default.socket_port);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _prol.Dispose();
        }
    }
}