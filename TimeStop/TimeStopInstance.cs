using Terraria;

namespace TBAR.TimeStop
{
    public class TimeStopInstance
    {
        public TimeStopInstance(Entity owner, int duration)
        {
            Owner = owner;
            Duration = duration;
        }

        public override string ToString()
        {
            return "{ O: " + Owner + "; D: " + Duration + " }";
        }

        public int Duration { get; set; }

        public Entity Owner { get; set; }
    }
}
