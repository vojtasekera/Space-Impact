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
        Graphics g;
        Bitmap canvas;

        private void bStart_Click(object sender, EventArgs e)
        {
            canvas = new Bitmap(Data.map_width, Data.map_height);
            map = new Map(Data.map_width, Data.map_height, Data.spawnbox);
            map.SetPlayer(3); //TODO
            g = Graphics.FromImage(canvas);

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
            map.Update();
            map.DrawMap(g, canvas);
            pictureBox1.Image = canvas;
            Refresh();
        }
    }
}
