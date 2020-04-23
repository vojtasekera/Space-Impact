using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact
{
    enum State {pause, fight, spawn, loss, win}

    class Rect
    {
        public int x = 0;
        public int y = 0;
        public int width, height;

        public int left    {get => x;}
        public int right   {get => x + width - 1;}
        public int top     {get => y;}
        public int bottom  {get => y + height - 1;}

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
        int attack_cool;
        static int attack_cooldown = Constants.player_attack_cooldown;

        public override void Shoot()
        {
            if (attack_cool <= 0)
            {
                attack_cool = attack_cooldown;
                base.Shoot();
            }
        }

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
            if (Invincible) return;
            health -= rec;
            Invincible = true; 
            invincibility_timer = Constants.player_invincibility_time;
            if (health <= 0) map.state = State.loss;
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

        public void SpecialAttack()
        {

        }

        public override void Update()
        {
            if (invincible)
            {
                invincibility_timer--;
                if (invincibility_timer <= 0) Invincible = false;
            }

            if (attack_cool > 0) attack_cool--;
        }

        public Player(int _x, int _y, int _health, Map m) : base(_x, _y, m)
        {
            key = "player";
            health = _health;
            damage = 0;
            friendly = true;
            height = Constants.player_height;
            width = Constants.player_width;
            visuals = new Visuals(Texture.FromKey(key));
            visuals.Stop();
            shoot_xoff = Constants.player_attack_xoff;
            shoot_yoff = Constants.player_attack_yoff;
        }
    }

    class Map : Rect
    {
        public State state = State.spawn;

        public Player player;

        List<MovingObject> list_of_moving = new List<MovingObject>();

        public  Map(int w, int h, int p) : base(w,h)
        {
            player = new Player(Constants.player_spawn_x, height / 2, p, this);
        }

        public Map()
        {

        }

        public void Add(MovingObject member)
        {
            list_of_moving.Add(member);
        }

        public void Remove(MovingObject member)
        {
            list_of_moving.Remove(member);
        }

        public void Spawn()
        {
            //TESTING
            Random random = new Random();
            new NPC.BossA(70, 20, this);
            //new Enemy.BossA(random.Next(0,70), random.Next(0,45) , this);
            ///new Enemy.BossB(random.Next(0, 70), random.Next(0, 45), this);
        }

        void MoveAll()
        {
            foreach (MovingObject member in list_of_moving.ToList())
            {
                member.Update();
            }
        }

        void Update()
            {
            var list = list_of_moving.ToList();
           
            foreach (MovingObject foo in list)
            {
                foreach (MovingObject bar in list)
                {
                    if (foo.CollidesWith(bar) && foo!=bar)
                    {
                        foo.CollisionWith(bar);
                        bar.CollisionWith(foo);
                    }
                }
            }
        }

        Brush brush = new SolidBrush(Constants.bg_color);


        void DrawMap(Graphics g)
        {
            g.FillRectangle(brush, g.ClipBounds);

            foreach (MovingObject member in list_of_moving)
            {
                member.Draw(g);
                //TESTING
                //g.DrawRectangle(Pens.Red, member.x, member.y, member.width - 1, member.height- 1);
            }
        }

        public void MapOut(Graphics g)
        {
            MoveAll();
            DrawMap(g);
            Update();
        }

        public int UnitCount() => list_of_moving.Count;
    }

    class MovingObject : Rect
    {
       // static Dictionary<string, MovingObject> default_values;

        protected string key;
        public Map map;
        protected Behaviour behaviour;
        protected Visuals visuals;

        protected int health = 1;
        protected int damage = 1;
        protected bool invincible = false;
        public bool friendly = false;
        public bool ignores_projectiles = false;
        protected int despawn_timer = Constants.despawn_time;

        protected int shoot_xoff, shoot_yoff;

        public virtual void Shoot()
        {
            new NormalAttack(x + shoot_xoff, y + shoot_yoff, map, friendly);
        }

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
            if (friendly != coll.friendly) coll.Damage(damage);
        }

        public void Draw(Graphics sender)
        {
            if (visuals != null) 
            visuals.Draw(this, sender);
        }

        public virtual void Update()
        {
            if (behaviour != null)
            behaviour.Update(this);

            if (!this.CollidesWith(map))
            {
                despawn_timer--;
                if (despawn_timer <= 0) Destroy();
            }
            else despawn_timer = Constants.despawn_time;
        }

        public MovingObject(int _x, int _y, Map m)
        {
            
            x = _x;
            y = _y;
            map = m;
            map.Add(this);
        }

    }

    
    static class NPC
    {

        public class Jelly : MovingObject
        {
            public Jelly(int x, int y, Map m) : base(x, y, m)
            {
                key = "jelly";
                health = 1;
                width = 3;
                height = 5;
                behaviour = new Behaviour.OnlyMoving(new Movement.Linear(1));
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        public class Basket : MovingObject
        {
            static int[] x_movement = new int[]
                    { 2, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, -1, 0, -1, 0, -1, -1, -1, -2, -2, -1, -1, -1, 0, -1, -1, 0, -1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 2 };
            static int[] y_movement = new int[] { 1 };
            public Basket(int x, int y, Map m) : base(x, y, m)
            {
                key = "basket";
                health = 1;
                width = 3;
                height = 3;
                behaviour = new Behaviour.OnlyMoving(new Movement.General(x_movement, y_movement));
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        public class Bean : MovingObject
        {
            public Bean(int x, int y, Map m) : base(x, y, m)
            {
                key = "bean";
                health = 1;
                height = 3;
                width = 6;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        public class BossA : MovingObject
        {
            public BossA(int x, int y, Map m) : base(x, y, m)
            {
                key = "bossa";
                health = 30;
                height = 15;
                width = 15;
                visuals = new Visuals(Texture.FromKey(key));
                behaviour = new Behaviour.BossA(Movement.General.OnlyX(1,0), new Movement.Linear(0,1,50), 200, 
                    Movement.General.OnlyX(1,0,1,1,2,1,2,2,3,4,3,3,2,2,1,1,0,1,0,0,0,0,0,0,0,0,0,0,0));
            }
        }

        public class BossB : MovingObject
        {
            public BossB(int x, int y, Map m) : base(x, y, m)
            {
                key = "bossb";
                health = 50;
                height = 20;
                width = 12;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        class Dog : MovingObject
        {
            public Dog(int x, int y, Map m) : base(x, y, m)
            {
                key = "dog";
                health = 6;
                height = 7;
                width = 7;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        class Eater: MovingObject
        {
            public Eater(int x, int y, Map m) : base(x, y, m)
            {
                key = "eater";
                health = 3;
                height = 11;
                width = 6;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        class Fish : MovingObject
        {
            public Fish(int x, int y, Map m) : base(x, y, m)
            {
                key = "fish";
                health = 3;
                height = 11;
                width = 6;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        class Ship : MovingObject
        {
            public Ship(int x, int y, Map m) : base(x, y, m)
            {
                key = "ship";
                health = 5;
                height = 3;
                width = 9;
                visuals = new Visuals(Texture.FromKey(key));
            }
        }

        class Tadpole : MovingObject
        {
            public Tadpole(int x, int y, Map m) : base(x, y, m)
            {
                key = "tadpole";
                health = 1;
                height = 3;
                width = 6;
                visuals = new Visuals(Texture.FromKey(key));
            }
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
            key = "projectile";
            damage = Constants.normal_attack_dmg;
            width = Constants.projectile_width;
            height = Constants.projectile_height;
            friendly = f;
            this.visuals = new Visuals(Texture.FromKey(key));
            if (friendly) behaviour = new Behaviour.OnlyMoving(new Movement.Linear(-2));
           else behaviour = behaviour = new Behaviour.OnlyMoving(new Movement.Linear(2));

        }
    }

    class SpecialAttack
    {

    }

    class Boss : MovingObject
    {
        public Boss(int _x, int _y, Map m) : base(_x,_y,m) 
        {
           //TODO: movement = new Movement.Linear(1, 2, this);
        }
    }
    

    abstract class Movement
    {
        public bool ended;
        public virtual void Move(MovingObject parent) { }

        public class Linear : Movement
        {
            int hor_speed, ver_speed;
            int reverse, temp = 0;
            bool wiggle = false;
            bool up = true;

            public override void Move(MovingObject parent)
            {

                if (wiggle)
                {
                    if (up) parent.y -= ver_speed;
                    else parent.y += ver_speed;
                }
                parent.x -= hor_speed;
                temp++;

                if (temp >= reverse || parent.OnEdgeOf(parent.map))
                {
                    temp = 0;
                    up = !up;
                }
            }
            public Linear(int s)
            {
                hor_speed = s;
            }
            public Linear(int s, int rev)
            {
                hor_speed = s;
                ver_speed = s;
                reverse = rev;
                wiggle = true;
            }
            public Linear(int hs, int ws, int rev)
            {
                hor_speed = hs;
                ver_speed = ws;
                reverse = rev;
                wiggle = true;
            }
        }

        public class General : Movement
        {
            int x_pos, y_pos;
            int[] x;
            int[] y;
            int i = 1;

            public General(int[] x, int[] y)
            {
                this.x = x;
                this.y = y;
            }

            public General()
            {
            }

            public override void Move(MovingObject parent)
            {
                ended = false;

                if (x != null) 
                {
                    parent.x -= i*x[x_pos];
                    x_pos++;
                    x_pos %= x.Length;

                    // (x_pos == 0 && (y != null => x.Length >= y.Length))
                    // tuto implikaci zapíšeme pomocí negace
                    if (x_pos == 0 && !(y != null && !(x.Length >= y.Length))) ended = true;
                }
                if (y != null)
                {
                    parent.y -= i*y[y_pos];
                    y_pos++;
                    y_pos %= y.Length;

                    if (y_pos == 0 && !(x != null && !(y.Length >= x.Length))) ended = true;
                }

            }

            public void Reverse()
            {
                if (x != null) 
                {
                   // x_pos = x.Length - x_pos - 1 ;
                    Array.Reverse(x); 
                }
                if (y != null)
                {
                    //y_pos = y.Length - y_pos - 1;
                    Array.Reverse(y); 
                }

                i = -i;
            }

            public static General OnlyX(params int[] steps)
            {
                General a = new General();
                a.x = steps;
                return a;
            }
            public static General OnlyY(params int[] steps)
            {
                General a = new General();
                a.y = steps;
                return a;
            }
        }



    }


    abstract class Behaviour
        //tato třída spravuje chování nepřátel, projektilů a dalších jednotek - tedy pohyb a střelbu
    {
        static Dictionary<string, Behaviour> Data;

        public static Behaviour FromKey(string key)
        {
            Behaviour test;
            Data.TryGetValue(key, out test);
            return test;
        }

        public virtual void Update(MovingObject parent) { }

        public class OnlyMoving : Behaviour
        {
            protected Movement movement;

            public override void Update(MovingObject parent)
            {
                movement.Move(parent);
            }

            public OnlyMoving(Movement mov)
            {
                movement = mov;
            }
        }
      
        public class Shooting : OnlyMoving
        {

            int shoot_interval, si_temp;

            public Shooting(Movement mov, int shoot_interval) : base(mov)
            {
                this.shoot_interval = shoot_interval;
            }

            public override void Update(MovingObject parent)
            {
                if (movement != null)
                movement.Move(parent);

                si_temp--;
                if (si_temp <= 0)
                {
                    si_temp = shoot_interval;
                    parent.Shoot();
                }
            }
        }

        public class BossB : Behaviour
        {
            Behaviour phase;
            Behaviour.OnlyMoving entry;
            Behaviour.Shooting stable;

            public override void Update(MovingObject parent)
            {
                if (phase == entry && parent.map.right > parent.right) phase = stable;
                phase.Update(parent);
            }

            public BossB(Movement mov, Shooting sh)
            {
                entry = new OnlyMoving(mov);
                stable = sh;
                phase = entry;
            }
        }

        public class BossA : Behaviour
        {
            Movement entry, stable;
            Movement.General charge;
            enum Phase { entry, stable, charge, pause, retreat }
            Phase phase = Phase.entry;

            int stable_time; //= GameData.bossB_stable_time;
            int pause_time = 50;
            int timer;
            public override void Update(MovingObject parent)
            {
                switch (phase)
                {
                    case Phase.entry:
                        entry.Move(parent);
                        if (parent.map.right > parent.right)
                        {
                            
                            phase = Phase.stable;
                        }
                        break;
                    case Phase.stable:
                        stable.Move(parent);
                        timer++;
                        if (timer >= stable_time)
                        {
                            phase = Phase.charge;
                            
                            timer = 0;
                        }


                        break;
                    case Phase.charge:
                        charge.Move(parent);
                        if (charge.ended == true)
                        {
                            phase = Phase.pause;
                            charge.Reverse();
                        }
                        break;
                    case Phase.pause:

                        timer++;
                        if (timer >= pause_time)
                        {
                            phase = Phase.retreat;
                        }

                        break;
                    case Phase.retreat:

                        charge.Move(parent);
                        if (charge.ended == true)
                        {
                            phase = Phase.stable;
                            charge.Reverse();
                        }
                        break;
                    default:
                        break;
                }
            }


            public BossA(Movement entry, Movement stable, int stable_time, Movement.General charge)
            {
                this.entry = entry;
                this.stable = stable;
                this.charge = charge;
                this.stable_time = stable_time;
            }

          
        }
    }

    public class Texture
    {
        static Dictionary<string, Texture> Data;
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

        static public Texture FromKey(string key)
        {
            return Data[key];
        }

        public static void Load(string file)
        {
            List<string> fields;
            string name;
            int xoff, yoff;


            string[] lines = System.IO.File.ReadAllLines(file);
            foreach(string line in lines)
            {

                fields = line.Split(';').ToList();
                name = fields[0];
                xoff = int.Parse(fields[1]);
                yoff = int.Parse(fields[2]);
                fields.RemoveRange(0, 3);
                Data.Add(fields.First(), new Texture(xoff, yoff, fields.ToArray()));
            }
        }
    }

    class Visuals
    {
        bool loop = true;
        int delay = Constants.graphics_flash_interval, tick;
        int image;
        Texture data;

        public void Draw(MovingObject parent, Graphics sender)
        {
            if (data != null)
            sender.DrawImage(data.images[image], parent.x + data.offx, parent.y + data.offy);
            if (loop)
            {
                tick++;
                tick %= delay;

                if (tick == 0) Next();
            }
        }

        public void Next()
        {
            image++;
            image %= data.images.Count();
        }

        public void Stop()
        {
            loop = false;
        }

        public Visuals(Texture t)
        {
            data = t;
        }
    }

    public static class Constants
    {
        public const int player_width = 6;
        public const int player_height = 5;
        public const int player_spawn_x = 5;
        public const int player_attack_xoff = 6;
        public const int player_attack_yoff = 2;
        public const int player_invincibility_time = 50;
        public const int player_attack_cooldown = 10;

        public const int map_width = 75;
      
        public const int despawn_time = 10;
        public const int map_height = 45;
        public const int scale = 18;
        public static Color bg_color = Color.FromArgb(133, 178, 149);

        public const int graphics_flash_interval = 10;

        public static int projectile_width = 3;
        public static int projectile_height = 1;
        public static int normal_attack_dmg = 1;

        public const int bossB_stable_time = 40;
        public static int[] basket_ymovement = new int[] 
        { 2, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, -1, 0, -1, 0, -1, -1, -1, -2, -2, -1, -1, -1, 0, -1, -1, 0, -1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 2 };
        public static int[] basket_xmovement = new int[] {1};
    }
}
