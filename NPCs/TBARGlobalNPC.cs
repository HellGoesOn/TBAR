using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.NPCs
{
    public class TBARGlobalNPC : GlobalNPC
    {
        public static TBARGlobalNPC Get(NPC npc) => npc.GetGlobalNPC<TBARGlobalNPC>();

        public static void RestoreAction(NPC npc)
        {
            if (Get(npc).AccumulatedDamage <= 0)
                return;

            Main.LocalPlayer.ApplyDamageToNPC(npc, Get(npc).AccumulatedDamage, 0, 0, false);
            Get(npc).AccumulatedDamage = 0;
        }

        private int accumulatedDamage;
        public int AccumulatedDamage
        { 
            get => accumulatedDamage;
            set => accumulatedDamage = (int)MathHelper.Clamp(value, 0, int.MaxValue); 
        }

        public override bool InstancePerEntity => true;
    }
}
