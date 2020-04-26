using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Space_Impact
{
    public static class Constants
    {
        public static Dictionary<string, int> settings;

        public const int player_width = 6;
        public const int player_height = 5;
        public const int player_spawn_x = 5;
        public const int player_attack_xoff = 6;
        public const int player_attack_yoff = 2;
        public const int player_invincibility_time = 50;
        public const int player_attack_cooldown = 10;

        public const int map_width = 75;
        public const int hud_height = 7;


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
        public static int[] basket_xmovement = new int[] { 1 };
    }
}
