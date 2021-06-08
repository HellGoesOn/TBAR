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

        public override SpriteAnimation AlbumEntryAnimation()
        {
            string path = "Projectiles/Stands/Italy/Aerosmith/";
            return new SpriteAnimation(path + "Idle", 18, 12, true);
        }

        public override void InitializeCombos()
        {
        }

        public override string GetDamageScalingText => "8 + 120% DPS";

        public override DamageType StandDamageType => DamageType.Ranged;
    }
}
