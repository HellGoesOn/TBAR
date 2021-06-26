using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace TBAR
{
    public class TBARConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnChanged()
        {
            inputDelay = comboInputDelay;
            tileGrabbyRange = tileGrabRange;
            disableTimeStopVisual = disableTSVFX;
            runnerType = comboDelayDisplayType;
        }

        public override void OnLoaded()
        {
            inputDelay = comboInputDelay;
            tileGrabbyRange = tileGrabRange;
            disableTimeStopVisual = disableTSVFX;
            runnerType = comboDelayDisplayType;
        }

        [Header("Controls")]

        [Label("Combo Activation Delay")]
        [DefaultValue(60)]
        [Range(10, int.MaxValue)]
        [Tooltip("Time (in ticks) that it takes for combo to activate after the last input has been made")]
        public int comboInputDelay;

        [Header("Visual")]
        [DefaultValue(0)]
        [Range(-128, 128)]
        [Label("King Crimson VFX Tile Grab Range")]
        [Tooltip("Changes how many tiles KC's VFX can affect.\nHeavily impacts performance")]
        public int tileGrabRange;

        [Label("Disable Time Stop Visual")]
        [DefaultValue(false)]
        [Tooltip("Disables Time Stop's graying effect")]
        public bool disableTSVFX;

        [Label("Combo Delay Display Type")]
        [DefaultValue(RunnerType.Default)]
        [Tooltip("Changes the position of Combo Delay Timer")]
        public RunnerType comboDelayDisplayType;

        internal static int inputDelay = 60;
        internal static int tileGrabbyRange = 0;
        internal static bool disableTimeStopVisual = false;
        internal static RunnerType runnerType = RunnerType.Default; 
    }

    public enum RunnerType
    {
        Default,
        Mouse,
        Static,
        Disabled
    }
}
