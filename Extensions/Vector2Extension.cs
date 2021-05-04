using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.Extensions
{
    public static class Vector2Extension
    {
        public static Vector2 DirectTo(this Vector2 vector, Vector2 position, float speed = 1)
        {
            return (position - vector).SafeNormalize(-Vector2.UnitY) * speed;
        }

        public static Vector2 ToMouse(this Vector2 v, float speed = 1) => v.DirectTo(Main.MouseWorld, speed);

        public static Vector2 RetardedMethodName(this Vector2 v, Vector2 clampPoint, float length)
        {
            if (Vector2.DistanceSquared(v, clampPoint) > length * length)
                return clampPoint - v.DirectTo(clampPoint, length);

            return v;
        }
    }
}
