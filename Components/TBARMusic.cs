using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Components
{
    public class TBARMusic
    {
        public static int AddTrackToQueue(string path, int duration, MusicPriority priority = MusicPriority.BossHigh)
        {
            if (Main.netMode == NetmodeID.Server)
                return 0;

            TBARMusic i = new TBARMusic(path, duration, priority);

            TBAR.Instance.Tracks.Add(i);

            return TBAR.Instance.Tracks.Count - 1;
        }

        public static void RemoveTrackFromQueue(int index)
        {
            TBAR.Instance.Tracks.RemoveAt(index);
        }

        protected TBARMusic(string path, int duration, MusicPriority priority)
        {
            Path = path;
            TimeLeft = duration;
            Priority = priority;
        }

        public int TimeLeft { get; set; }

        public MusicPriority Priority { get; }

        public string Path { get; }
    }
}
