using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Items.Vanity.VinegarDisguise
{
    [AutoloadEquip(EquipType.Body)]
    public class VinegarShirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Vinegar's Disguise");
            Tooltip.SetDefault("Holds a terrible secret within...");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = -12;
            item.vanity = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(TBAR.Instance);
            recipe.AddIngredient(ItemID.PinkThread, 2);
            recipe.AddIngredient(ItemID.Silk, 20);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
    public class DiavoloBody : EquipTexture
    {
    }
}
