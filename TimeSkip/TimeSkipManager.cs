using System.Collections.Generic;
using TBAR.Components;
using TBAR.UI.ScreenEffects.TimeSkip;

namespace TBAR.TimeSkip
{
    public class TimeSkipManager : GlobalEffectManager<TimeSkipInstance>
    {
        public TimeSkipManager() : base()
        {
            VisualEffects = new List<TimeSkipVisualEffect>();
        }

        public void UpdateVisuals()
        {
            foreach(TimeSkipVisualEffect ts in VisualEffects)
            {
                ts.Update();
            }

            VisualEffects.RemoveAll(x => !x.Animation.Active);
        }

        public List<TimeSkipVisualEffect> VisualEffects { get; } 
    }
}
