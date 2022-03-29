using Terraria.ModLoader;
using Terraria.ID;

namespace TBAR.Items.Vanity.Deliquent
{
    [AutoloadEquip(EquipType.Head)]
    public class DeliquentHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Deliquent's Cap");
            Tooltip.SetDefault("Good grief...");
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
}
