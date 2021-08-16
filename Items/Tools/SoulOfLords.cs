using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace TBAR.Items.Tools
{
    public class SoulOfLords : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A powerful being has willed this very item into existance");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
        }

        public override void SetDefaults()
        {
            item.useTime = item.useAnimation = 15;
            item.noUseGraphic = true;
            item.expert = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = 12;
        }

        public override bool UseItem(Player player)
        {
            TBARPlayer tBAR = TBARPlayer.Get(player);

            tBAR.PlayerStand = StandLoader.Instance.Get("Soul of Cinder");

            Main.NewText("You feel the Lords soul burn within you");

            return true;
        }
    }
}
