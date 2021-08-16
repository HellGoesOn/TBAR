using TBAR.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Buffs.MagicianRed
{
    public class RedBindDebuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Red Bind");
            Description.SetDefault("Unable to move");
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            TBARGlobalNPC.Get(npc).RedBinded = true;
            TBARGlobalNPC.Get(npc).Shocked = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // TO-DO: Add PvP shit here;
        }
    }
}
