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

            PlayerInput.Pause = PauseGame;
            PlayerInput.ReturnToMenu = EndGame;

           
            canvas = new Bitmap(GameData.map_width, GameData.map_height);
            render = new Bitmap(GameData.map_width * GameData.scale, GameData.scale * GameData.map_height);
            g = Graphics.FromImage(canvas);
            h = Graphics.FromImage(render);
            h.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor ;
        }

        Map map;
        Graphics g, h;
        Bitmap canvas, render;
        enum GameState { active, paused, menu };
        GameState win_state = GameState.menu;
        

        static class PlayerInput
        {
            public enum Direction { none, up, down, left, right };
            static Direction last_pressed;
            static bool[] keys_down = new bool[Enum.GetNames(typeof(Direction)).Length];

            static bool attack, a_down, 
                        special_attack, sa_down;
            public static bool SpecialAttack
                {
                get
                {
                    if (sa_down || special_attack)
                    {
                        special_attack = false;
                        return true;
                    }
                    return false;
                }
                }
            public static bool Attack
            {
                get
                {
                    if (a_down || attack)
                    {
                        attack = false;
                        return true;
                    }
                    return false;
                }
            }

            public delegate void GameControl();

            public static GameControl Pause, ReturnToMenu;
            
            public static void IssueKeys(bool change_to, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        last_pressed = Direction.up;
                        keys_down[(int)last_pressed] = change_to;
                        break;

                    case Keys.S:
                        last_pressed = Direction.down;
                        keys_down[(int)last_pressed] = change_to;
                        break;

                    case Keys.A:
                        last_pressed = Direction.left;
                        keys_down[(int)last_pressed] = change_to;
                        break;

                    case Keys.D:
                        last_pressed = Direction.right;
                        keys_down[(int)last_pressed] = change_to;
                        break;
                }

                switch (e.KeyCode)
                {
                    case Keys.L:
                        sa_down = change_to;
                        if (change_to) special_attack = true;
                        break;

                    case Keys.K:
                        a_down = change_to;
                        if (change_to) attack = true;
                        break;

                    case Keys.Escape:
                        if (change_to) ReturnToMenu();
                        break;

                    case Keys.P:
                        if (change_to) Pause();
                        break;
                }

                e.Handled = true;
            }

            public static Direction GetMovementKey()
            {
                if (keys_down[(int)last_pressed])
                {
                    return last_pressed;
                }
                else
                {
                    foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
                    {
                        if (keys_down[(int)dir])
                        {
                            return dir;
                        }

                    }
                }

                Direction a = last_pressed;
                last_pressed = Direction.none;
                return a;
            }
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            PlayerInput.IssueKeys(true, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            PlayerInput.IssueKeys(false, e);
        }

        void PauseGame()
        {
            //TODO: show paused message
            switch (win_state)
            {
                case GameState.active:
                    timer.Stop();
                    win_state = GameState.paused;
                    break;
                case GameState.paused:
                    timer.Start();
                    win_state = GameState.active;
                    break;
                case GameState.menu:
                    break;
                default:
                    break;
            }
         
        }


        void EndGame()
        {
            win_state = GameState.menu;
            bStart.Visible = true;
            tbLevel.Visible = true;
            pictureBox1.Visible = false;
            tbLevel.ReadOnly = false;
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            win_state = GameState.active;
            bStart.Visible = false;
            tbLevel.Visible = false;
            tbLevel.ReadOnly = true;


            //TODO: load level from file
            map = new Map(GameData.map_width, GameData.map_height, GameData.spawnbox);
            map.SetPlayer(3); //TODO
            pictureBox1.Image = null;
            pictureBox1.Visible = true;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (PlayerInput.GetMovementKey())
            {
                case PlayerInput.Direction.up:
                    map.player.Up();
                    break;
                case PlayerInput.Direction.down:
                    map.player.Down();
                    break;
                case PlayerInput.Direction.right:
                    map.player.Right();
                    break;
                case PlayerInput.Direction.left:
                    map.player.Left();
                    break;
            }

            if (PlayerInput.Attack) map.player.Shoot();
            if (PlayerInput.SpecialAttack) map.player.SpecialAttack();

            map.Update();
            map.DrawMap(g);

            h.DrawImage(canvas, 0, 0, canvas.Width * GameData.scale, canvas.Height * GameData.scale);
            pictureBox1.Image = render;
            pictureBox1.Refresh();
        }
    }
}
