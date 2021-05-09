using System.Collections.Generic;
using System.Linq;
using TBAR.Components;
using TBAR.Players;
using TBAR.Projectiles.Stands.Crusaders.TheWorld;
using Terraria;
using Terraria.ModLoader;

namespace TBAR
{
    public partial class TBAR : Mod
    {
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }

            TBARPlayer plr = TBARPlayer.Get();

            if(Tracks.Count > 0)
            {
                Tracks[0].TimeLeft--;

                music = GetSoundSlot(SoundType.Music, Tracks[0].Path);
                priority = Tracks[0].Priority;

                Tracks.RemoveAll(x => x.TimeLeft <= 0);
            }

            if (plr.IsUsingArrow)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/StandObtain2");
                priority = MusicPriority.BossHigh;
            }
        }

        public List<TBARMusic> Tracks { get; }
    }
}
