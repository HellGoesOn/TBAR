using TBAR.Enums;
using TBAR.Structs;
using Terraria;

namespace TBAR.Extensions
{
    public static class ItemExtension
    {
        public static DamageType GetDamageType(this Item item)
        {
            if (item.melee)
                return DamageType.Melee;
            if (item.magic)
                return DamageType.Melee;
            if (item.ranged)
                return DamageType.Ranged;
            if (item.summon)
                return DamageType.Summoner;

            return DamageType.Any;
        }

        public static ItemDamageData GetDamageData(this Item item, Player p) => new ItemDamageData(item, p);
    }
}
