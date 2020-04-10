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

    class Player : MovingObject
    {
        int invincibility_timer = 0;
        int special_attacks;

        public bool Invincible
        {
            get => invincible;
            set
            {
                visuals.Next();
                invincible = value;
            }
        }

        public int Health
        {
            get => health;
        }

        public override void Damage(int rec)
        {
            if (invincible) return;
            health += rec;
            if (health <= 0) map.state = State.loss;
            else if (rec < 0) { Invincible = true; invincibility_timer = GameData.player_invincibility_time; }
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

        public void Shoot()
        {
            new NormalAttack(x + GameData.player_attack_xoff, y + GameData.player_attack_yoff, map, true);
        }

        public void SpecialAttack()
        {

        }

        public void Update()
        {
            if (invincible)
            {
                invincibility_timer--;
                if (invincibility_timer <= 0) Invincible = false;
            }
        }

        public Player(int _x, int _y, int _health, Map m) : base(_x, _y, m)
        {
            health = _health;
            height = GameData.player_height;
            width = GameData.player_width;
            visuals = new Visuals(GameData.player_texture, this);
            movement = new Movement.Player();
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
            spawnbox = new Rect(w + s,h);
        }

        public void SetPlayer(int h)
        {
            player = new Player( GameData.player_spawn_x, height/2 , h, this);
        }

        public void Add(MovingObject member)
        {
            list_of_moving.Add(member);
        }

        public void Remove(MovingObject member)
        {
            list_of_moving.Remove(member);
        }

        public void Update()
        {
            player.Update();

            foreach (MovingObject member in list_of_moving.ToList())
            {
                member.Move();

                
            }

//            foreach (MortalObject enemy in list_of_moving)
            {
                foreach (MovingObject projectile in list_of_moving)
                {
                        //TODO: collision events
                }
            }
        }

        Brush brush = new SolidBrush(GameData.bg_color);
        public void DrawMap(Graphics g)
        {
            Brush fg_brush = Brushes.Red;
            g.FillRectangle(brush, g.ClipBounds);

            foreach (MovingObject member in list_of_moving)
            {
                g.FillRectangle(fg_brush, member.x, member.y, 1, 1);
            }

            player.Draw(g);
        }
    }


    class MovingObject : Rect
    {
        public Map map;
        protected Movement movement = new Movement.Player();
        protected Visuals visuals;

        protected int health;
        protected int damage;
        protected bool invincible;
        public bool friendly = false;
        public bool ignores_projectiles = false;
        protected int despawn_timer = GameData.despawn_time;


        public void Destroy()
        {
            map.Remove(this);
        }

        public virtual void Damage(int rec)
        {
            if (invincible) return;
            health -= rec;
            if (health <= 0) Destroy();
        }

        public virtual void CollisionWith(MovingObject coll)
        {

        }

        public void Draw(Graphics sender)
        {
            if (visuals != null) 
            visuals.Draw(sender);
        }

        public virtual void Move()
        {
            if (movement != null)
            movement.Move();

            //TODO: mozna nebude treba uzit spawnbox
            if (!this.CollidesWith(map))
            {
                despawn_timer--;
                if (despawn_timer <= 0) Destroy();
            }
            else despawn_timer = GameData.despawn_time;
        }

        public MovingObject(int _x, int _y, Map m)
        {
            
            x = _x;
            y = _y;
            map = m;
            map.Add(this);
        }
    }
    
    class NormalAttack : MovingObject
    {
        public override void CollisionWith(MovingObject coll)
        {
            if (friendly != coll.friendly && !coll.ignores_projectiles)
            {
                coll.Damage(damage);
                Destroy();
            }
        }

        public NormalAttack(int _x, int _y, Map m, bool f):base(_x, _y, m)
        {
            damage = GameData.normal_attack_dmg;
            width = GameData.projectile_width;
            height = GameData.projectile_height;
            friendly = f;
            this.visuals = new Visuals(GameData.projectile_texture, this);
            if (friendly) movement = new Movement.Linear(-2, 1, 5, this);
            else movement = new Movement.Linear(2, 1, 5, this);

        }
    }

    class SpecialAttack
    {

    }

    class Boss : MovingObject
    {
        enum Phase { entry, stable, charge }

        Phase phase = Phase.entry;
        int entry_time = GameData.boss_entry_time;

        public Boss(int _x, int _y, Map m) : base(_x,_y,m) 
        {
            movement = new Movement.Linear(1, 2, this);
        }
    }
    

    abstract class Movement
    {
        protected MovingObject parent;

        public virtual void Move()
        {
        }

        public class Player : Movement
        {

        }

        public class Linear : Movement
        {
            int delay = 0;
            int modulo = 1, hor_speed, ver_speed;
            int reverse, temp = 0;
            bool wiggle = false;
            bool up = true;

            public override void Move()
            {
                if (delay == 0)
                {
                    if (wiggle)
                    {
                        if (up) parent.y -= ver_speed;
                        else parent.y += ver_speed;
                    }
                    parent.x -= hor_speed;
                    temp ++ ;
                }

                delay++;
                delay %= modulo;

                if (temp >= reverse|| parent.OnEdgeOf(parent.map))
                {
                    temp = 0;
                    up = !up;
                }
            }

            public Linear(int s, int m, MovingObject p)
            {
                parent = p;
                hor_speed = s;
                modulo = m;
            }

            public Linear(int s, int m, int rev, MovingObject p)
            {
                parent = p;
                hor_speed = s;
                ver_speed = s;
                modulo = m;
                reverse = rev;
                wiggle = true;
            }

            public Linear(int hs, int ws, int m, int rev, MovingObject p)
            {
                parent = p;
                hor_speed = hs;
                ver_speed = ws;
                modulo = m;
                reverse = rev;
                wiggle = true;
            }
        }

        class Boss : Movement
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
        MovingObject parent;
        int tick = 0;

        Texture data;

        public void Draw(Graphics sender)
        {
            if (data != null)
            sender.DrawImage(data.images[tick], parent.x + data.offx, parent.y + data.offy);
        }

        public void Next()
        {
            tick++;
            tick %= data.images.Count();
        }

        public Visuals(Texture t, MovingObject sender)
        {
            data = t;
            parent = sender;
        }
    }

    public struct GameData
    {
        public const int player_width = 4;
        public const int player_height = 5;
        public const int player_spawn_x = 5;
        public const int player_attack_xoff = 6;
        public const int player_attack_yoff = 2;
        public const int player_invincibility_time = 300;

        public static int map_width = 75;
        public static int spawnbox = 10;
        public static int despawn_time = 30;
        public static int map_height = 45;
        public static int scale = 18;
        public static Color bg_color = Color.FromArgb(133, 178, 149);

        static public Texture player_texture = new Texture(-7, -3, @"Resources\player1.png", @"Resources\player2.png");
        static public Texture projectile_texture = new Texture(0, 0, @"Resources\projectile.png");
        public static int projectile_width = 3;
        public static int projectile_height = 1;
        public static int normal_attack_dmg = 1;

        public const int boss_entry_time = 40;
    }
}
