using Terraria;

namespace TBAR.TimeStop
{
    public class TimeStopInstance
    {
        public TimeStopInstance(Entity owner, int duration, string soundPath = "")
        {
            Owner = owner;
            Duration = duration;

            if (soundPath != "")
                TBAR.Instance.PlaySound(soundPath);
        }

        public override string ToString()
        {
            return "{ O: " + Owner + "; D: " + Duration + " }";
        }

        public string EndSoundEffect { get; set; }

        public int Duration { get; set; }

        public Entity Owner { get; set; }
    }
}
