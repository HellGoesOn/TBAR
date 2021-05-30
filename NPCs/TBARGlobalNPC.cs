using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Components;
using TBAR.TimeSkip;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.NPCs
{
    public partial class TBARGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            CustomBuffs = new List<TBARBuff>();
            TimeSkipStates = new List<TimeSkipData>();
        }

        public static TBARGlobalNPC Get(NPC npc) => npc.GetGlobalNPC<TBARGlobalNPC>();

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            PostKingDraw(npc, spriteBatch);
        }

        public override bool PreAI(NPC npc)
        {
            foreach (TBARBuff buff in CustomBuffs)
                buff.UpdateNPC(npc);

            CustomBuffs.RemoveAll(x => x.Duration <= 0);

            PreTimeSkipAI(npc);

            return base.PreAI(npc);
        }

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

        public List<TBARBuff> CustomBuffs { get; private set; }

        public override bool InstancePerEntity => true;
    }
}
