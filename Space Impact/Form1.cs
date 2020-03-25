using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Impact
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            pictureBox1.Width = GameData.scale * GameData.map_width;
            pictureBox1.Height = GameData.scale * GameData.map_height;
            this.Width = GameData.scale * GameData.map_width; 
            this.Height = GameData.scale * GameData.map_height + this.Height - this.ClientSize.Height; 
        }

        Map map;
        Graphics g, h;
        Bitmap canvas, render;

        enum Direction {up, down, left, right};
        enum Action {none, shoot, special};

        Direction dir = Direction.left;
        Action action = Action.none;
        int step = 0;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {   
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (dir == Direction.up) step++;
                    else { 
                        step = 1;
                        dir = Direction.up;
                    }
                    break;

                case Keys.S:
                    if (dir == Direction.down) step++;
                    else
                    {
                        step = 1;
                        dir = Direction.down;
                    }
                    break;

                case Keys.A:
                    if (dir == Direction.left) step++;
                    else
                    {
                        step = 1;
                        dir = Direction.left;
                    }
                    break;

                case Keys.D:
                    if (dir == Direction.right) step++;
                    else
                    {
                        step = 1;
                        dir = Direction.right;
                    }
                    break;

                case Keys.L:
                    action = Action.shoot;
                    break;

                case Keys.K:
                    action = Action.special;
                    break;

                case Keys.Escape:
                    break;

                case Keys.P:
                    break;
            }

            e.Handled = true;

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //e.KeyCode
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            bStart.Visible = false;
            tbLevel.Visible = false;

            map = new Map(GameData.map_width, GameData.map_height, GameData.spawnbox);
            map.SetPlayer(3); //TODO
            canvas = new Bitmap(GameData.map_width, GameData.map_height);
            render = new Bitmap(GameData.map_width * GameData.scale, GameData.scale * GameData.map_height);
            g = Graphics.FromImage(canvas);
            h = Graphics.FromImage(render);
            h.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor ;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (step > 0) {
                switch (dir)
                {
                    case Direction.up:
                        map.player.Up();
                        break;
                    case Direction.down:
                        map.player.Down();
                        break;
                    case Direction.right:
                        map.player.Right();
                            break;
                    case Direction.left:
                        map.player.Left();
                        break;
                }
                step--;
            }

            switch (action)
            {
                case Action.shoot:
                    map.player.Shoot();
                    break;
                case Action.special:
                    map.player.SpecialAttack();
                    break;
            }
            action = Action.none;
            map.Update();
            this.Text = map.player.x.ToString();
            //(new Thread(delegate(){
            map.DrawMap(g);
            h.DrawImage(canvas, 0, 0, canvas.Width * GameData.scale, canvas.Height * GameData.scale);
            // Bitmap bitmap = new Bitmap(Data.map_width, Data.map_height);
            //   g.DrawImage(pictureBox1.Image, 0, 0, Data.map_width, Data.map_height);
            pictureBox1.Image = render;
            pictureBox1.Refresh();
            //})).Start();
        }
    }
}
