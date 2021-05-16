using TBAR.Components;
using TBAR.Projectiles.Stands.Italy.KingCrimson;

namespace TBAR.Stands.Italy
{
    public class KingCrimson : SingleEntityStand
    {
        public KingCrimson() : base(new KingCrimsonProjectile(), "King Crimson")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Italy/KingCrimson/KCIdle", 5, 5, true);
        }

        public override void InitializeCombos()
        {

        }
    }
}
