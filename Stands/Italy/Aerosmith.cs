using TBAR.Components;
using TBAR.Enums;
using TBAR.Projectiles.Stands;
using TBAR.Projectiles.Stands.Italy.Aerosmith;

namespace TBAR.Stands.Italy
{
    public class Aerosmith : SingleEntityStand<AerosmithProjectile>
    {
        public Aerosmith() : base("Aerosmith")
        {
        }

        public override Animation2D AlbumEntryAnimation()
        {
            string path = "Projectiles/Stands/Italy/Aerosmith/";
            return new Animation2D(path + "Idle", 18, 12, true);
        }

        public override void InitializeCombos()
        {
        }

        public override string GetDamageScalingText => "8 + 120% DPS";

        public override string GetEffectiveRangeText => "200m";

        public override DamageType StandDamageType => DamageType.Ranged;
    }
}
