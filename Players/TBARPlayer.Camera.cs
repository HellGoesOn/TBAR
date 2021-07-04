using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.ScreenModifiers;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public override void ModifyScreenPosition()
        {
            if (ScreenModifiers.Count > 0)
            {
                if (ScreenModifiers[0].Active)
                    ScreenModifiers[0].UpdateScreenPosition(ref Main.screenPosition);

                ScreenModifiers[0].UpdateModifier(player);

                if (ScreenModifiers[0].LifeTimeEnded)
                    ScreenModifiers.RemoveAt(0);
            }
        }

        public List<ScreenModifier> ScreenModifiers { get; private set; }
    }
}
