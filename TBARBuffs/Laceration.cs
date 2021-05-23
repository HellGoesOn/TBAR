using TBAR.Components;
using TBAR.Enums;
using TBAR.Projectiles;
using TBAR.Projectiles.Stands.Italy.KingCrimson;
using Terraria;

namespace TBAR.TBARBuffs
{
    public class Laceration : TBARBuff
    {
        public override string Name => "Laceration";

        public override void Initialize()
        {
            if(Owner is Projectile proj && proj.modProjectile is KingCrimsonProjectile king)
            {
                damage = king.LacerationDamage;
            }
        }

        public override void UpdateNPC(NPC e)
        {
            if (Duration % (2 * Global.TPS) == 0)
            {
                SliceProjectile.Create(e, EntityType.Npc, damage);
                damage = (int)(damage * 0.85);
            }

            Duration--;

        }

        private int damage;

    }
}
