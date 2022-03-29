using Terraria.ModLoader;
using Terraria.ID;

namespace TBAR.Items.Vanity.MilfhunterGear
{
    [AutoloadEquip(EquipType.Head)]
    public class MilfhunterGearHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Student's Haircut");
            Tooltip.SetDefault("Fills you with determination to save someone's relatives");
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
