using Microsoft.Xna.Framework;
using TBAR.Extensions;
using TBAR.Players;
using TBAR.Projectiles.Stands.Italy.Aerosmith;
using TBAR.Stands.Italy;
using Terraria;

namespace TBAR.ScreenModifiers
{
    public class AerosmithScreenModifier : ScreenModifier
    {
        public AerosmithScreenModifier(Vector2 position) : base(position)
        {
        }

        public AerosmithScreenModifier(Vector2 position, int lifeTime) : base(position, lifeTime)
        {
        }

        public override void UpdateModifier(Player player)
        {
            TBARPlayer tPlayer = TBARPlayer.Get(player);

            if(tPlayer.PlayerStand is Aerosmith aero && aero.ActiveInstance != null)
            {
                AerosmithProjectile proj = aero.ActiveInstance;

                //Active = !aero.IsPatroling;

                Position = proj.projectile.Center + proj.projectile.Center.DirectTo(proj.MousePosition, 16);
            }
            else
            {
                LifeTime = 0;
                tPlayer.ScreenModifiers.Add(new PlayerChaseScreenModifier(Position, player.Center, 0.12f));
            }
        }
    }
}
