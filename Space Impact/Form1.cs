using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Impact
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Map map;
        static int map_width = 75;
        static int spawnbox = 10;
        static int map_height = 45;
        static int scale = 20;
        Bitmap canvas = new Bitmap(map_width * scale, map_height * scale);
        


        private void bStart_Click(object sender, EventArgs e)
        {
            map = new Map(map_width, map_height, spawnbox);
            bStart.Visible = false;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            map.Update();
            Graphics g = CreateGraphics();
            map.DrawMap(g, canvas);
            pictureBox1.Image = (Image)canvas;
            Refresh();
        }
    }
}
