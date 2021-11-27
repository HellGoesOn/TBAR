using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Helpers;
using TBAR.TimeSkip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.NPCs
{
    public partial class TBARGlobalNPC : GlobalNPC
    {
        public bool Shocked { get; set; }

        public bool RedBinded { get; set; }
        
        public float[] DefaultAI { get; set; }

        public override void SetDefaults(NPC npc)
        {
            NPCID.Sets.UsesNewTargetting[npc.type] = true;
            CustomBuffs = new List<TBARBuff>();
            TimeSkipStates = new List<TimeSkipData>();
            DefaultAI = new float[npc.ai.Length];

            for (int i = 0; i < npc.ai.Length; i++)
                DefaultAI[i] = npc.ai[i];
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (TBAR.TimeStopManager.IsTimeStopped)
                damage = (int)(damage * 1.4f);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (TBAR.TimeStopManager.IsTimeStopped)
                damage = (int)(damage * 1.4f);
        }

        public static TBARGlobalNPC Get(NPC npc) => npc.GetGlobalNPC<TBARGlobalNPC>();

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            PostKingDraw(npc, spriteBatch);
        }

        public override void ResetEffects(NPC npc)
        {
            Shocked = false;
            RedBinded = false;
        }

        public override bool PreAI(NPC npc)
        {
            foreach (TBARBuff buff in CustomBuffs)
                buff.UpdateNPC(npc);

            CustomBuffs.RemoveAll(x => x.Duration <= 0);

            PreTimeSkipAI(npc);

            if(RedBinded)
            {
                npc.velocity *= 0f;
                npc.oldVelocity *= 0f;;

                for (int i = 0; i < DefaultAI.Length; i++)
                    npc.ai[i] = DefaultAI[i];

                CoolVFXMethodEcksDee(npc);
            }

            return base.PreAI(npc);
        }

        public static void RestoreAction(NPC npc)
        {
            if (Get(npc).AccumulatedDamage <= 0)
                return;

            Main.LocalPlayer.ApplyDamageToNPC(npc, Get(npc).AccumulatedDamage, 0, 0, false);
            Get(npc).AccumulatedDamage = 0;
        }


        private void CoolVFXMethodEcksDee(NPC npc)
        {
            var pos1 = npc.BottomLeft + new Vector2(npc.width * 0.4f, -(npc.height * 0.4f));
            var pos2 = npc.BottomRight - new Vector2(npc.width * 0.4f, npc.height * 0.4f);
            var pos3 = npc.Top + new Vector2(0, npc.height * 0.4f);

            int val = (int)((npc.height + npc.width) * 0.33f);
            int height = (int)MathHelper.Clamp(val, 60, int.MaxValue);

            int width = (int)(height * 0.33f);

            DrawHelper.CircleDust(pos1, npc.BottomLeft.DirectTo(npc.Center), DustID.Fire, height, width, .7f, 60);
            DrawHelper.CircleDust(pos2, npc.BottomRight.DirectTo(npc.Center), DustID.Fire, height, width, .7f, 60);
            DrawHelper.CircleDust(pos3, npc.Top.DirectTo(npc.Center), DustID.Fire, height, width, .7f, 60);
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
