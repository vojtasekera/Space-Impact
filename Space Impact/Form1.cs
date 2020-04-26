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
            pbHUD.Width = Constants.scale * Constants.hud_height;
            pbGame.Height = Constants.scale * Constants.map_width;
            pbGame.Top = pbHUD.Top + pbHUD.Height;
            pbGame.Width = Constants.scale * Constants.map_width;
            pbGame.Height = Constants.scale * Constants.map_height;

            pbHUD.BackColor = Color.Red;
           // pbGame.BackColor = Color.Blue;

            this.Width = pbHUD.Width + 200;
            this.Height = pbHUD.Height + pbGame.Height;
            this.BackColor = Constants.bg_color;

            //PlayerInput.Pause = map.Spawn; //PauseGame;
            PlayerInput.ReturnToMenu = EndGame;

           
            game_screen = new Bitmap(Constants.map_width, Constants.map_height);
            gs_out = new Bitmap(Constants.map_width * Constants.scale, Constants.scale * Constants.map_height);

            player_stats = new Bitmap(Constants.map_width, Constants.hud_height);
            ps_out = new Bitmap(Constants.map_width * Constants.scale, Constants.scale * Constants.hud_height);

            gs = Graphics.FromImage(game_screen);
            gs2 = Graphics.FromImage(gs_out);
            gs2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor ;

            pbGame.Size = gs2.ClipBounds.Size.ToSize();
        }

        Map map;
        Graphics gs, gs2, ps, ps2;
        Bitmap game_screen, gs_out, player_stats, ps_out;
        enum GameState { active, paused, menu };
        GameState win_state = GameState.menu;
        

        /*
         * Třída poskutující metody pro události KeyDown a KeyUp
         * pamatuje si poslední stisknuté klásvesy z W, A, S, D, L, K, P, ESCAPE
         */
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
            pbGame.Visible = false;
            tbLevel.ReadOnly = false;
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            win_state = GameState.active;
            bStart.Visible = false;
            tbLevel.Visible = false;
            tbLevel.ReadOnly = true;


            //TODO: load level from file
            map = new Map(Constants.map_width, Constants.map_height, 3);
            pbGame.Image = null;
            pbGame.Visible = true;

            //TESTING
            PlayerInput.Pause = map.Spawn;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (PlayerInput.GetMovementKey())
            {
                case PlayerInput.Direction.up:
                    map.AddPlayerAction(Map.PlayerAction.up);
                    break;
                case PlayerInput.Direction.down:
                    map.AddPlayerAction(Map.PlayerAction.down);

                    break;
                case PlayerInput.Direction.right:
                    map.AddPlayerAction(Map.PlayerAction.right);
                    break;
                case PlayerInput.Direction.left:
                    map.AddPlayerAction(Map.PlayerAction.left);
                    break;
            }

            if (PlayerInput.Attack) map.AddPlayerAction(Map.PlayerAction.attack);
            if (PlayerInput.SpecialAttack) map.AddPlayerAction(Map.PlayerAction.special_attack);

            map.MapOut(gs,gs2); //TODO: stats display
            this.Text = map.UnitCount().ToString();

            gs2.DrawImage(game_screen, 0, 0, game_screen.Width * Constants.scale, game_screen.Height * Constants.scale);
            pbGame.Image = gs_out;
            pbGame.Refresh();
        }
    }
}
