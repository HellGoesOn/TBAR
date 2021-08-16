using System;
using TBAR.Components;
using TBAR.Projectiles.Stands.Donator.SoulOfCinder;

namespace TBAR.Stands.Donator
{
    public class SoulOfCinder : SingleEntityStand<SoulOfCinderProjectile>
    {
        public SoulOfCinder() : base("Soul of Cinder")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Donator/SoulOfCinder/Idle", 10, 15, true);
        }

        public override void InitializeCombos()
        {

        }
    }
}
