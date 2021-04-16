using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.ScreenModifiers
{
    public class PlayerChaseScreenModifier : SmoothStepScreenModifier
    {
        public PlayerChaseScreenModifier(Vector2 position, Vector2 destination, float speed) : base(position, destination, speed)
        {
        }

        public PlayerChaseScreenModifier(Vector2 position, Vector2 destination, float speed, int lifeTime) : base(position, destination, speed, lifeTime)
        {
        }

        public override void UpdateModifier(Player player)
        {
            base.UpdateModifier(player);

            Destination = player.Center + new Vector2(1);
        }
    }
}
