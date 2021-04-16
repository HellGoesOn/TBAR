using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.ScreenModifiers
{
    public class SmoothStepScreenModifier : ScreenModifier
    {
        public SmoothStepScreenModifier(Vector2 position, Vector2 destination, float speed) : base(position)
        {
            Destination = destination;
            Speed = speed;
        }

        public SmoothStepScreenModifier(Vector2 position, Vector2 destination, float speed, int lifeTime) : base(position, lifeTime)
        {
            Destination = destination;
            Speed = speed;
        }

        public override void UpdateModifier(Player player)
        {
            LifeTime--;

            Position = Vector2.Lerp(Position, Destination, Speed);

            if (Vector2.Distance(Position, Destination) <= 2)
                LifeTime = 0;
        }

        public Vector2 Destination { get; set; }

        public float Speed { get; set; }
    }
}
