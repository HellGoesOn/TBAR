using System.Collections.Generic;
using TBAR.Components;
using TBAR.TimeSkip;
using TBAR.TimeStop;
using Terraria.ModLoader;

namespace TBAR
{
    public partial class TBAR : Mod
    {
        public static TimeSkipManager TimeSkipManager { get; private set; }

        public static TimeStopManager TimeStopManager { get; private set; }
    }
}
