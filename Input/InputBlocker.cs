using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.Players;
using Terraria;

namespace TBAR.Input
{
    public class InputBlocker
    {
        /// <summary>
        /// Blocks Input for specified Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="duration">How long (in ticks) should Input Blocker exist</param>
        /// <param name="id">Custom ID</param>
        /// <returns>ID of the input blocker instance</returns>
        public static int BlockInputs(Player player, int duration, int id = -1)
        {
            TBARPlayer plr = TBARPlayer.Get(player);
            var whoAmI = id;

            if (whoAmI == -1)
                whoAmI = plr.InputBlockers.Count;

            var block = new InputBlocker(duration, whoAmI);

            plr.InputBlockers.Add(block);

            return id;
        }

        public static void UnblockInputs(Player player, int id)
        {
            TBARPlayer plr = TBARPlayer.Get(player);

            var blocker = plr.InputBlockers.Find(x => x.ID == id);

            if(blocker != null)
            {
                plr.InputBlockers.Remove(blocker);
            }
        }

        public InputBlocker(int duration, int id)
        {
            Duration = duration;
            ID = id;
        }

        public int Duration { get; set; }

        public int ID { get; set; }
    }
}
