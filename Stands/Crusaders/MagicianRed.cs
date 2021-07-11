﻿using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.Projectiles.Stands.Crusaders.Chicken;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Stands.Crusaders
{
    public sealed class MagicianRed : SingleEntityStand<MagicianRedProjectile>
    {
        public MagicianRed() : base("Magician's Red")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/Chicken/Idle", 7, 15, true);
        }

        public override void InitializeCombos()
        {
            StandCombo falconPunch = new StandCombo("Fist of Flaming Fury", ComboInput.Action1, ComboInput.Action1, ComboInput.Action2);

            falconPunch.OnActivate += FalconPunch_OnActivate;

            StandCombo zxcursed = new StandCombo("Fireraze", ComboInput.Action1, ComboInput.Action2, ComboInput.Action3);
            zxcursed.Description = "Releases Pillars of Flames in the direction you are facing";

            zxcursed.OnActivate += Zxcursed_OnActivate;

            StandCombo desrucxz = new StandCombo("Firequake", ComboInput.Action1, ComboInput.Action2, ComboInput.Action1);
            desrucxz.Description = "Releases Pillars of Flames in both directions";

            desrucxz.OnActivate += Desrucxz_OnActivate;

            AddNormalCombos(falconPunch, zxcursed, desrucxz);
        }

        private void Desrucxz_OnActivate(Player player)
        {
            Projectile.NewProjectile(player.Bottom + new Vector2(40, -60), new Vector2(1, 0), ModContent.ProjectileType<FirePillar>(), 60, 5.75f, player.whoAmI, 4, 1);
            Projectile.NewProjectile(player.Bottom + new Vector2(-40, -60), new Vector2(-1, 0), ModContent.ProjectileType<FirePillar>(), 60, 5.75f, player.whoAmI, 4, -1);
        }

        private void Zxcursed_OnActivate(Player player)
        {
            Projectile.NewProjectile(player.Bottom + new Vector2(40 * player.direction, -60), new Vector2(player.direction, 0), ModContent.ProjectileType<FirePillar>(), 60, 5.75f, player.whoAmI, 9, player.direction);
        }

        private void FalconPunch_OnActivate(Player player)
        {
            ActiveInstance?.SetState(MRStates.FalconPunch.ToString());
        }

        public override bool CanAcquire(TBARPlayer player)
        {
            return true;
        }

        public override DamageType StandDamageType => DamageType.Magic;

        public override string GetDamageScalingText => "12 + 140%(75%) DPS";

        public override string GetEffectiveRangeText => "2m(melee)";
    }
}
