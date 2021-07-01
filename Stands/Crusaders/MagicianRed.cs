using TBAR.Components;
using TBAR.Enums;
using TBAR.Players;
using TBAR.Projectiles.Stands.Crusaders.Chicken;

namespace TBAR.Stands.Crusaders
{
    public sealed class MagicianRed : SingleEntityStand<MagicianRedProjectile>
    {
        public MagicianRed() : base("Magician's Red")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/Chicken/Idle", 7, 15, true);
        }

        public override void InitializeCombos()
        {
        }

        public override bool CanAcquire(TBARPlayer player)
        {
            return true;
        }

        public override DamageType StandDamageType => DamageType.Magic;

        public override string GetDamageScalingText => "12 + 140%(75%) DPS";

        public override string GetEffectiveRangeText => "2m(melee)";
    }
}
