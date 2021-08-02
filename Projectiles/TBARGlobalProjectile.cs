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

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsAffectedByGlobalModifier)
                damage = (int)(damage * TBAR.DamageModifier);
        }

        public override void ModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if (IsAffectedByGlobalModifier)
                damage = (int)(damage * TBAR.DamageModifier);
        }

        public bool IsAffectedByGlobalModifier { get; set; }

        public bool HitRoadRollerInLifeTime { get; set; }
    }
}
