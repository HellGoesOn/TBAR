using TBAR.Components;
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
    }
}
