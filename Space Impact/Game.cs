using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Impact
{
    delegate void M();

    enum State {pause, fight, spawn, loss, win}

    class Rect
    {
        public int x = 0;
        public int y = 0;
        public int width, height;

        int left    {get => x;}
        int right   {get => x + width;}
        int top     {get => y;}
        int bottom  {get => y + height;}

        public bool CollidesWith(Rect rect)
        {
            if (rect.left <= right && rect.right >= left &&
                rect.top <= bottom && rect.bottom >= top)
            {
                return true;
            }
            else return false;
        }

        public bool OnEdgeOf(Rect rect)
        {
            if (top <= rect.top || bottom >= rect.bottom) return true;
            else return false;
        }

        public Rect()
        {

        }

        public Rect(int w, int h)
        {
            width = w;
            height = h;
        }
    }

    class Player : Rect
    {
        public Map map;
        int health;
        Visuals visuals;

        public void RecDamage()
        {
            health--;
            if (health <= 0)
            {
                map.state = State.loss;
            }
        }

        public Player(int _x, int _y, int h, Map m):base(_x, _y)
        {
            health = h;
            map = m;
            height = Data.player_height;
            width = Data.player_width;
            //TODO: hero visual
        }
    }

    class Map : Rect
    {
        Rect spawnbox;

        public State state;

        Player player;

        public M endgame;

        List<MovingObject> list_of_moving = new List<MovingObject>();

        public  Map(int w, int h, int s) : base(w,h)
        {
            spawnbox = new Rect(w+s,h);
        }

        public void SetPlayer(int h)
        {
            player = new Player(0, height/2, h, this);
        }

        public void Update()
        {
            foreach (MovingObject member in list_of_moving)
            {
                member.behaviour.Move();

                if  (!member.CollidesWith(spawnbox))
                {
                    list_of_moving.Remove(member);
                    //((IDisposable)member).Dispose();

                }
            }

            foreach (PlayerProjectile projectile in list_of_moving)
            {
                foreach (MortalObject enemy in list_of_moving)
                {
                    if (projectile.CollidesWith(enemy))
                    {

                    }
                }
            }
        }

        public void DrawMap(Graphics g, Bitmap canvas)
        {
            Brush bg_brush = Brushes.Aquamarine;
            Brush fg_brush = Brushes.Black;
            g.FillRectangle(bg_brush, 0,0, canvas.Width, canvas.Height);

            foreach (MovingObject member in list_of_moving)
            {
                g.FillRectangle(fg_brush, member.x, member.y, 20, 20);
            }
        }
    }

    abstract class Behaviour
    {
        protected MovingObject parent;

        public virtual void Move()
        {
        }

        class Linear : Behaviour
        {
            int delay = 0;
            int speed, modulo;
            public override void Move()
            {
                if (delay == 0) parent.x -= speed;
                delay++;
                delay %= modulo;
            }
            public Linear(int s, int mod)
            {
                this.speed = s;
                this.modulo = mod;
            }
        }

        class Wiggly : Behaviour
        {
            int delay = 0;
            int modulo, speed;
            int length, temp = 0;
            bool up = true;
            public override void Move()
            {
                if (delay == 0)
                {
                    if (up) parent.y -= speed;
                    else parent.y += speed;
                    parent.x -= speed;
                    temp += speed;
                }
                delay++;
                delay %= modulo;

                if (temp >= length || parent.OnEdgeOf(parent.map))
                {
                    temp = 0;
                    up = !up;
                } 
            }

            public Wiggly(int s, int m, int l)
            {
                speed = s;
                modulo = m;
                length = l;
            }
        }

        class Boss : Behaviour
        {

        }
    }

    class MovingObject : Rect
    {
        public Map map;
        public Behaviour behaviour;
        Visuals visuals;
    }

    class MortalObject : MovingObject
    {
       
        int health;
    }

    class PlayerProjectile : MovingObject
    {
        class NormalAttack
        {

        }

        class SpecialAttack
        {

        }
    }

    class Texture
    {
        public int offx, offy;
        public List<Bitmap> images;

        public Texture(int x, int y, params string[] file_names)
        {
            offx = x;
            offy = y;
            images = new List<Bitmap>();
            foreach (string name in file_names)
            {
                images.Add(new Bitmap(name));
            }
        }
    }

    class Visuals
    {
        MovingObject parent;
        int tick;

        Texture data;

        public void Draw(Graphics sender)
        {
            sender.DrawImage(data.images[tick], parent.x + data.offx, parent.y + data.offy);
            tick++;
            tick %= data.images.Count();
        }

        public Visuals(Bitmap b)
        {

        }
    }

    public struct Data
    {
        public const int player_width = 3;
        public const int player_height = 3;

        public static int map_width = 75;
        public static int spawnbox = 10;
        public static int map_height = 45;
        public static int scale = 20;

    }
}
