using Microsoft.Xna.Framework;
using System;
using TBAR.Enums;
using TBAR.Extensions;
using Terraria;

namespace TBAR.Structs
{
    public struct ItemDamageData
    {
        public DamageType DamageType { get; }

        /// <summary>
        /// Item's DPS. Disregards projectile count
        /// </summary>
        public int DPS { get; }

        /// <summary>
        /// Base Item Damage with no modifiers
        /// </summary>
        public int RawDamage { get; }

        /// <summary>
        /// Damage adjusted with modifiers
        /// </summary>
        public int Damage { get; }

        public ItemDamageData(Item item, Player player)
        {
            DamageType = item.GetDamageType();

            int damage, useAnimation;

            Item tempItem = new Item();
            tempItem.SetDefaults(item.type);

            RawDamage = tempItem.damage;

            useAnimation = item.useAnimation > 0 ? item.useAnimation : 60;

            switch (DamageType)
            {
                case DamageType.Melee:
                    damage = (int)(item.damage * player.meleeDamage * player.allDamage);
                    break;
                case DamageType.Ranged:
                    damage = (int)(item.damage * player.rangedDamage * player.allDamage);
                    break;
                case DamageType.Magic:
                    damage = (int)(item.damage * player.magicDamage * player.allDamage);
                    break;
                case DamageType.Summoner:
                    damage = (int)(item.damage * player.minionDamage * player.allDamage);
                    break;
                default:
                    damage = (int)(item.damage * player.allDamage);
                    break;
            }

            if (item.notAmmo)
                damage = 0;

            Damage = damage;

            DPS = (int)(damage * (60 / useAnimation));
        }
    }
}
