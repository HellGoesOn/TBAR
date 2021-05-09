using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles
{
    public class TBARGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void Kill(Projectile projectile, int timeLeft)
        {
            HitRoadRollerInLifeTime = false;
        }

        public bool HitRoadRollerInLifeTime { get; set; }
    }
}
