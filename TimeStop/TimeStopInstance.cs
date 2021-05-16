using System;
using TBAR.Components;
using Terraria;

namespace TBAR.TimeStop
{
    public class TimeStopInstance : IGlobalEffect
    {
        public TimeStopInstance(Entity _owner, int duration, string soundPath = "")
        {
            owner = _owner;
            Duration = duration;

            if (soundPath != "")
                TBAR.Instance.PlaySound(soundPath);
        }

        public override string ToString()
        {
            return "{ O: " + owner + "; D: " + Duration + " }";
        }

        public void Update()
        {
            Duration--;
        }

        public Entity Owner()
        {
            return owner;
        }

        public string EndSoundEffect { get; set; }

        public int Duration { get; set; }

        private readonly Entity owner;
    }
}
