using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Impact
{
    

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
            set
            {
                map.StatsChanged();
                health = value;
            }
        }
        public int SpecialAttacks
        {
            get => special_attacks;
            set
            {
                map.StatsChanged();
                special_attacks = value;
            }
        }
        public override void Damage(int rec)
        {
            if (Invincible) return;
            Health -= rec;
            Invincible = true; 
            invincibility_timer = Constants.player_invincibility_time;
            if (Health <= 0) map.state = Map.State.loss;
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
        public enum State { pause, fight, spawn, loss, win }
        public enum PlayerAction { up, down, left, right, attack, special_attack }

        public State state = State.spawn;
        List<PlayerAction> player_actions = new List<PlayerAction>();

        public Player player;
        long score;
        long Score
        {
            get => score;
            set
            {
                stats_changed = true;
                score = value;
            }
        }
        bool stats_changed;

        public void StatsChanged()
        {
            stats_changed = true;
        }

        List<MovingObject> list_of_moving = new List<MovingObject>();

        public  Map(int w, int h, int p) : base(w,h)
        {

            //TODO: proper init
            Texture.Load(@"Resources\textures.csv");
            player = new Player(Constants.player_spawn_x, height / 2, p, this);
        }

        public Map()
        {
        }


        #region player handling
        public void AddPlayerAction(PlayerAction action)
        {
            player_actions.Add(action);
        }

        void RealizePlayerAction(PlayerAction actions)
        {
            switch (actions)
            {
                case PlayerAction.up:
                    player.Up();
                    break;
                case PlayerAction.down:
                    player.Down();
                    break;
                case PlayerAction.left:
                    player.Left();
                    break;
                case PlayerAction.right:
                    player.Right();
                    break;
                case PlayerAction.attack:
                    player.Shoot();
                    break;
                case PlayerAction.special_attack:
                    player.SpecialAttack();
                    break;
                default:
                    break;
            }
        }

        void HandlePlayer()
        {
            foreach (PlayerAction action in player_actions)
            {
                RealizePlayerAction(action);
            }
            player_actions.Clear();
        }
        #endregion

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


        #region updating and drawing 
        void MoveAll()
        {
            foreach (MovingObject member in list_of_moving.ToList())
            {
                member.Update();
            }
        }

       
        void Update()
            {

            HandlePlayer();


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
            g.Clear(Constants.bg_color);

            foreach (MovingObject member in list_of_moving)
            {
                member.Draw(g);
            }
        }

        public void MapOut(Graphics g, Graphics h)
        {
            MoveAll();
            DrawMap(g);
            Update();
            if (stats_changed) DrawStats(h);

        }
        #endregion

        public void DrawStats(Graphics g)
        {
            Bitmap pic;
            int width;

            pic = Texture.FromKey("heart").Image();
            width = pic.Width;

            for (int i = 0; i < player.Health; i++)
            {
                g.DrawImage(pic, 1, i*(width + 1) + 1);
            }

            //TODO: max hp -> offset

            int dec = 1;
            string num;
            int edge = (int)g.Clip.GetBounds(g).Width;

            for (int i = 0; i < 5; i++)
            {
                dec *= 10;
                num = (score % dec).ToString();
                pic = Texture.FromKey("num"+num).Image();
                width = pic.Width;
                g.DrawImage(pic, (edge - width - 1), 1);
            }

            stats_changed = false;
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

        public MovingObject(string key, Map map, Behaviour behaviour, Visuals visuals, int health, int damage, bool invincible, bool friendly, bool ignores_projectiles, int despawn_timer, int shoot_xoff, int shoot_yoff)
        {
            this.key = key;
            this.map = map;
            this.behaviour = behaviour;
            this.visuals = visuals;
            this.health = health;
            this.damage = damage;
            this.invincible = invincible;
            this.friendly = friendly;
            this.ignores_projectiles = ignores_projectiles;
            this.despawn_timer = despawn_timer;
            this.shoot_xoff = shoot_xoff;
            this.shoot_yoff = shoot_yoff;
        }

       
    }

    
    class NPC
    {
        struct NPCStats
        {
            int width;
            int height;
            int health;
            int score; //TODO
            bool friendly;
            bool invincible;
            bool ignores_projectiles;
            string visuals_key;
            string behaviour_key;
            int shoot_xoff;
            int shoot_yoff;

            public NPCStats(int width, int height, int health, int score, bool friendly, bool invincible, bool ignores_projectiles, string visuals_key, string behaviour_key, int shoot_xoff, int shoot_yoff)
            {
                this.width = width;
                this.height = height;
                this.health = health;
                this.score = score;
                this.friendly = friendly;
                this.invincible = invincible;
                this.ignores_projectiles = ignores_projectiles;
                this.visuals_key = visuals_key;
                this.behaviour_key = behaviour_key;
                this.shoot_xoff = shoot_xoff;
                this.shoot_yoff = shoot_yoff;
            }
        }
        static Dictionary<string,NPCStats> default_npc_stats = new Dictionary<string,NPCStats>();

        public static void LoadNPCData(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] fields;

            foreach (string line in lines)
            {
                try
                {
                    fields = line.Split(',');
                    default_npc_stats[fields[0]] = new NPCStats(
                        int.Parse(fields[1]),
                        int.Parse(fields[2]),
                        int.Parse(fields[3]),
                        int.Parse(fields[4]),
                        bool.Parse(fields[5]),
                        bool.Parse(fields[6]),
                        bool.Parse(fields[7]),
                        fields[8],
                        fields[9],
                        int.Parse(fields[10]),
                        int.Parse(fields[11])
                        );
                }
                catch (Exception)
                {
                    MessageBox.Show("Nastala chyba při načítání databáze NPC");
                }
            }
        }

        public static void Spawn(Map m, int x, int y, int h, string key, string beh)
        {

        }

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
}
