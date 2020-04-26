using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact
{
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
            bool _x, _y;
            int i = 1;

            public General(int[] x, int[] y)
            {
                this.x = x;
                if (x != null && x.Length > 0) _x = true;
                this.y = y;
                if (y != null && y.Length > 0) _y = true;
            }

            public General()
            {
            }

            public override void Move(MovingObject parent)
            {
                ended = false;

                if (_x)
                {
                    parent.x -= i * x[x_pos];
                    x_pos++;
                    x_pos %= x.Length;

                    // (x_pos == 0 && (y != null => x.Length >= y.Length))
                    // tuto implikaci zapíšeme pomocí negace
                    if (x_pos == 0 && !(y != null && !(x.Length >= y.Length))) ended = true;
                }
                if (_y)
                {
                    parent.y -= i * y[y_pos];
                    y_pos++;
                    y_pos %= y.Length;

                    if (y_pos == 0 && !(x != null && !(y.Length >= x.Length))) ended = true;
                }

            }

            public void Reverse()
            {
                

                i = -i;
            }

            public static General OnlyX(params int[] steps)
            {
                return new General(steps, null);
            }
            public static General OnlyY(params int[] steps)
            {
                return new General(null, steps);
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
}
