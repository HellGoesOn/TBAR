using TBAR.Components;
using TBAR.UI.ScreenEffects.TimeSkip;

namespace TBAR
{
    public class TimeSkipManager
    {
        private TimeSkipManager()
        {
            VFX = new TimeSkipVFX();
        }

        public void Update()
        {
            VFX.Update();
        }

        public TimeSkipVFX VFX { get; }

        private static TimeSkipManager _instance;

        public static TimeSkipManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TimeSkipManager();

                return _instance;
            }
        }
    }
}
