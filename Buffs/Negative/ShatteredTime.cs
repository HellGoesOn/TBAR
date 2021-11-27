using Terraria.ModLoader;
using Terraria;
using TBAR.Players;

namespace TBAR.Buffs.Negative
{
    public class ShatteredTime : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shattered Time");
			Description.SetDefault("Cannot stop time flow");
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			TBARPlayer.Get(player).ShatteredTime = true;
		}
	}
}
