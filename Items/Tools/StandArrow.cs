using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Items.Tools
{
    public class StandArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bizarre Arrow");
            Tooltip.SetDefault("Use to unlock hidden powers\nIt looks almost as if it can move");
        }

        public override void SetDefaults()
        {
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.width = item.height = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 15;
            item.rare = ItemRarityID.Quest;
        }

        public override bool CanUseItem(Player player)
        {
            return !TBARPlayer.Get(player).IsStandUser;
        }

        public override bool UseItem(Player player)
        {
            TBARPlayer tBAR = TBARPlayer.Get(player);

            tBAR.PlayerStand = StandFactory.Instance.GetNewRandom(tBAR);

            Main.NewText("You've acquired a stand: " + tBAR.PlayerStand.StandName);

            return true;
        }
    }
}
