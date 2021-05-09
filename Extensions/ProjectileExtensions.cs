using Microsoft.Xna.Framework;
using TBAR.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Extensions
{
    public static class ProjectileExtensions
    {
        /// <summary>
        /// Changes width/height of the projectile and adjusts its position accordingly
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ChangeSize(this Projectile p, int width, int height)
        {
            p.width = width;
            p.height = height;

            p.Center -= new Vector2(width * 0.5f, height * 0.5f);
        }

        /// <summary>
        /// Changes width/height of the projectile and adjust its position
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size">Width AND Height</param>
        public static void ChangeSize(this Projectile p, int size)
        {
            p.width = size;
            p.height = size;

            p.Center -= new Vector2(size * 0.5f, size * 0.5f);
        }

        public static TBARGlobalProjectile GetGlobal(this Projectile p)
        {
            return p.GetGlobalProjectile<TBARGlobalProjectile>();
        }

        public static TBARGlobalProjectile GetGlobal(this ModProjectile p)
        {
            return p.projectile.GetGlobalProjectile<TBARGlobalProjectile>();
        }
    }
}
