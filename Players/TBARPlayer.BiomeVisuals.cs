using System.Linq;
using TBAR.Projectiles.Visual;
using TBAR.TimeStop;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public override void UpdateBiomeVisuals()
        {
            bool shockWaveExist = Main.projectile.Count(x => x.active && x.modProjectile is TimeStopVFX) > 0;
            player.ManageSpecialBiomeVisuals("TBA:FreezeSky", TBAR.TimeStopManager.IsTimeStopped && !shockWaveExist);
            player.ManageSpecialBiomeVisuals("TBA:TimeStopInvert", shockWaveExist);
        }
    }
}
