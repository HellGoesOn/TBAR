using TBAR.Players;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Items
{
    public sealed class TBARGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.damage > 0 && TBARConfig.standOnly && TBARPlayer.Get(player).PlayerStand != null && TBARPlayer.Get(player).PlayerStand.IsActive)
                return false;

            return base.CanUseItem(item, player);
        }
    }
}
