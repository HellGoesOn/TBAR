using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public event PlayerEventHandler OnLeftClick;

        public event PlayerEventHandler OnRightClick;
    }
}
