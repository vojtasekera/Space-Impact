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

    class Player : Rect
    {
        Map map;
        int health;
        Visuals visuals;

        public void RecDamage()
        {
            health--;
            if (health <= 0)
            {
                map.endgame();
            }
        }
        public Player(Map m, int h, Bitmap b)
        {
            map = m;
            health = h;
            visuals = new Visuals(b);
        }
    }

    class Rect
    {
        public int x = 0;
        public int y = 0;
        public int width, height;

        int left    {get => x;}
        int right   {get => x + width;}
        int top     {get => y;}
        int bottom  {get => y + height;}

        public bool CollidesWith(Rect member)
        {
            if ((member.left <= right || member.right <= left) && (member.top <= bottom || member.bottom <= top))
            {
                return true;
            }
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

    class Map : Rect
    {
        Rect spawnbox;
       // int spawn; //despawn limit
        Timer timer;
        Player player = new Player();

        public M endgame;

        public List<MovingObject> list_of_moving = new List<MovingObject>();

        public  Map(int w, int h, int s) : base(w,h)
        {
        //    spawn = s;
            spawnbox = new Rect(w+s,h);
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
        int time;

        public virtual void Move()
        {
        }
    }

    public class BE
    {
        class Linear: Behaviour
        {
            int delay = 0;
            int modulo;
            public override void Move()
            {
                if (delay == 0) parent.x --;
                delay++;
                delay %= modulo;
            }
            public Linear(int mod)
            {
                this.modulo = mod;
            }
        }

        class Wiggly: Behaviour
        {
            int delay = 0;
            int modulo;
            int length, temp = 0;
            bool up = true;
            public override void Move()
            {
                if (delay == 0)
                {
                    if (up) { parent.x--; };
                }
                delay++;
                delay %= modulo;
            }
        }

        class Boss: Behaviour
        {

        }
    }

    class MovingObject : Rect
    {
        Map map;
        public Behaviour behaviour;
        Visuals visuals;
    }

    class MortalObject : MovingObject
    {
       
        int health;
    }

    class PlayerProjectile : MovingObject
    {

    }

    class Texture
    {
        public Bitmap data;
    }

    class Visuals
    {
        MovingObject parent;
        Texture image;

        public void Draw(Graphics sender)
        {
            sender.DrawImage(this.image.data, parent.x, parent.y);
        }

        public Visuals(Bitmap b)
        {

            image.data = b;
        }
    }

    class TimedVisuals
    {
        int mod;
    }
}
