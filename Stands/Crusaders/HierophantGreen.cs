using TBAR.Components;
using TBAR.Enums;
using TBAR.Projectiles.Stands.Crusaders.Hierophant;

namespace TBAR.Stands.Crusaders
{
    public class HierophantGreen : SingleEntityStand<HierophantProjectile>
    {
        public HierophantGreen() : base("Hierophant Green")
        {
        }

        public override Animation2D AlbumEntryAnimation()
        {
            return new Animation2D("Projectiles/Stands/Crusaders/Hierophant/Idle", 6, 10, true);
        }

        public override void InitializeCombos()
        {
            // TO-DO: Make this irrelevant by a rewrite
        }

        public override DamageType StandDamageType => DamageType.Ranged;
    }
}
