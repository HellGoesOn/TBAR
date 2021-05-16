using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TBAR.ScreenModifiers;
using Microsoft.Xna.Framework;

namespace TBAR.Items.Tools
{
    public class StandArrow : ModItem
    {
        public const int CUTSCENE_DURATION = 500;

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
            item.useAnimation = item.useTime = CUTSCENE_DURATION;
            item.rare = ItemRarityID.Quest;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            return true; //!TBARPlayer.Get(player).IsStandUser;
        }

        public override bool UseItem(Player player)
        {
            TBARPlayer tBAR = TBARPlayer.Get(player);

            InputBlocker.BlockInputs(player, CUTSCENE_DURATION);

            player.mount.Dismount(player);

            tBAR.ArrowProgress = tBAR.ArrowProgressMax;

            tBAR.PlayerStand = StandLoader.Instance.GetNewRandom(tBAR);

            tBAR.ScreenModifiers.Add(new SmoothStepScreenModifier(player.Center, player.Center - new Vector2(0, 64), 0.025f));
            tBAR.ScreenModifiers.Add(new ScreenModifier(player.Center - new Vector2(0, 64)));

            tBAR.UsePosition = player.Center + new Vector2(0, player.height * 0.5f);

            player.velocity = Vector2.Zero;

            Main.NewText("You've acquired a stand: " + tBAR.PlayerStand.StandName);

            return true;
        }
    }
}
