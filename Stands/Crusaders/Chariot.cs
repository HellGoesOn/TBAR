using TBAR.Components;
using TBAR.Projectiles.Stands.Crusaders.Chariot;

namespace TBAR.Stands.Crusaders
{
    public class Chariot : SingleEntityStand<ChariotProjectile>
    {
        public Chariot() : base("Silver Chariot")
        {
        }

        public override Animation2D AlbumEntryAnimation()
        {
            return new Animation2D("Projectiles/Stands/Crusaders/Chariot/Idle", 9, 12, true);
        }

        public override void InitializeCombos()
        {
        }
    }
}
