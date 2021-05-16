using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.Components;
using TBAR.UI.ScreenEffects.TimeSkip;
using Terraria;

namespace TBAR.TimeSkip
{
    public class TimeSkipInstance : IGlobalEffect
    {
        public TimeSkipInstance(Entity owner, int duration)
        {
            _owner = owner;

            Duration = duration;

            TimeSkipVisual.Start();
        }

        public Entity Owner()
        {
            return _owner;
        }

        public void Update()
        {
            Duration--;

            if (Duration == 22)
                TimeSkipVisual.Start();
        }

        public int Duration { get; set; }

        private readonly Entity _owner;
    }
}
