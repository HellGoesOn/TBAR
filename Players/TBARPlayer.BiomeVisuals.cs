﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            player.ManageSpecialBiomeVisuals("TBA:FreezeSky", TimeStopManager.Instance.IsTimeStopped && !shockWaveExist);
            player.ManageSpecialBiomeVisuals("TBA:TimeStopInvert", shockWaveExist);
        }
    }
}