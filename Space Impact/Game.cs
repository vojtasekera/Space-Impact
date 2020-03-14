using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Impact
{
    enum State {pause, fight, spawn, loss, win}

    class Rect
    {
        public int x = 0;
        public int y = 0;
        public int width, height;

        public int left    {get => x;}
        public int right   {get => x + width;}
        public int top     {get => y;}
        public int bottom  {get => y + height;}

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

        public bool IsContainedIn(Rect rect)
        {
            if (top >= rect.top && bottom <= rect.bottom && 
                left >= rect.left && right <= rect.right) return true;
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
        enum Status {normal, damaged};

        Status status = Status.normal;
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

        public void Draw(Graphics sender)
        {
            visuals.Draw(sender);
        }

        public void Up()
        {
            if (top >= map.top) y--;
        }

        public void Down()
        {
            if (bottom <= map.bottom) y++;
        }

        public void Right()
        {
            if (right < map.right) x++;
        }
        public void Left()
        {
            if (left > map.left) x--;
        }
        public Player(int _x, int _y, int h, Map m):base(_x, _y)
        {
            x = _x;
            y = _y;
            health = h;
            map = m;
            height = Data.player_height;
            width = Data.player_width;
            visuals = new Visuals(Data.player_damaged_texture, this);
        }
    }

    class Map : Rect
    {
        Rect spawnbox;

        public State state = State.spawn;

        public Player player;

        List<MovingObject> list_of_moving = new List<MovingObject>();

        public  Map(int w, int h, int s) : base(w,h)
        {
            spawnbox = new Rect(w+s,h);
        }

        public void SetPlayer(int h)
        {
            player = new Player( Data.player_spawn_x, height/2 , h, this);
        }

        public void Remove(MovingObject member)
        {
            list_of_moving.Remove(member);
        }

        public void Update()
        {
            foreach (MovingObject member in list_of_moving)
            {
                member.Move();

                if  (!member.CollidesWith(spawnbox))
                {
                    member.Destroy();
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

        Brush brush = new SolidBrush(Data.bg_color);
        public void DrawMap(Graphics g)
        {
            Brush fg_brush = Brushes.Black;
            g.FillRectangle(brush, g.ClipBounds);

            foreach (MovingObject member in list_of_moving)
            {
                g.FillRectangle(fg_brush, member.x, member.y, 20, 20);
            }

            player.Draw(g);
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
        Behaviour behaviour;
        Visuals visuals;

        public void Destroy()
        {
            map.Remove(this);
            ((IDisposable)this).Dispose();
        }

        public void Draw(Graphics sender)
        {
            visuals.Draw(sender);
        }

        public void Move()
        {
            behaviour.Move();
        }
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

    public class Texture
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
                images.Add((Bitmap)Bitmap.FromFile(name));
            }
        }
    }

    class Visuals
    {
        Rect parent;
        int tick = 0;

        Texture data;

        public void Draw(Graphics sender)
        {
            sender.DrawImage(data.images[tick], parent.x + data.offx, parent.y + data.offy);
            tick++;
            tick %= data.images.Count();
        }

        public Visuals(Texture t, Rect sender)
        {
            data = t;
            parent = sender;
        }
    }

    public struct Data
    {
        public const int player_width = 4;
        public const int player_height = 5;
        public const int player_spawn_x = 5;

        public static int map_width = 75;
        public static int spawnbox = 10;
        public static int map_height = 45;
        public static int scale = 18;
        public static Color bg_color = Color.FromArgb(133, 178, 149);

        static public Texture player_healthy_texture = new Texture(-7, -3, @"Resources\player1.png");
        static public Texture player_damaged_texture = new Texture(-7, -3, @"Resources\player1.png", @"Resources\player2.png");

    }

    
    
}
