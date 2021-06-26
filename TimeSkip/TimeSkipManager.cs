using System.Collections.Generic;
using TBAR.Components;
using TBAR.UI.ScreenEffects.TimeSkip;

namespace TBAR.TimeSkip
{
    public class TimeSkipManager : GlobalEffectManager<TimeSkipInstance>
    {
        public TimeSkipManager() : base()
        {
            VisualEffects = new List<TimeSkipVisual>();
        }

        public void UpdateVisuals()
        {
            foreach(TimeSkipVisual ts in VisualEffects)
            {
                ts.Update();
            }

            VisualEffects.RemoveAll(x => x.Animation.CurrentFrame == x.Animation.FrameCount - 1);
        }

        public bool IsTimeSkipped => EffectCount > 0;

        public List<TimeSkipVisual> VisualEffects { get; } 
    }
}
