﻿using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using TBAR.Players.Visuals;
using TBAR.Projectiles.Stands.Crusaders.TheWorld;
using TBAR.Projectiles.Visual;
using TBAR.TimeStop;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Stands.Crusaders
{
    public class TheWorld : SingleEntityStand
    {
        public TheWorld() : base(new TheWorldProjectile(), "The World")
        {
        }

        public override void InitializeCombos()
        {
            StandCombo tsCombo = new StandCombo("Time Stop", ComboInput.Action1, ComboInput.Action2, ComboInput.Action1);
            tsCombo.OnActivate += TimeStop;

            StandCombo offensiveTimeStop = new StandCombo("Offensive Time Stop", ComboInput.Up, ComboInput.Action1, ComboInput.Up, ComboInput.Action2);
            offensiveTimeStop.OnActivate += OffensiveTimeStop;
            offensiveTimeStop.Description = "Allows you to prepare a Quick Time Stop.\nAfter use, Right-Click on a location to teleport there.\nClicking on an Entity will cause you to teleport behind it.";

            AddGlobalCombos(tsCombo, offensiveTimeStop);
        }

        private void TimeStop(Player player)
        {
            bool isTimeStopped = TimeStopManager.Instance.IsTimeStopped;
            string path = isTimeStopped ? "" : "Sounds/TheWorld/TheWorld_ZaWarudoSFX";

            TimeStopInstance ts = new TimeStopInstance(player, 600, path) { EndSoundEffect = "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX" };

            if (!isTimeStopped)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, player.whoAmI);
                TBAR.Instance.PlaySound("Sounds/TheWorld/TimeStop");
            }
            else
                TBAR.Instance.PlaySound("Sounds/TheWorld/TimeResume");

            TimeStopManager.Instance.TryStopTime(ts);
        }

        private void OffensiveTimeStop(Player player)
        {
            TBARPlayer plr = TBARPlayer.Get(player);

            plr.OnRightClick -= Plr_OnRightClick;
            plr.OnRightClick += Plr_OnRightClick;

            BeamVisual.AddBeamVisual(player, 20, 1200, 30, 15, Color.PaleVioletRed);
        }

        private void Plr_OnRightClick(Player sender)
        {
            TBARPlayer plr = TBARPlayer.Get(sender);

            Entity target = null;

            if (plr.TargetedNPC() != null)
                target = (Entity)plr.TargetedNPC();

            if (plr.TargetedPlayer() != null)
                target = (Entity)plr.TargetedPlayer();

            if (target != null)
            {
                Vector2 destination = target.Center - new Vector2(((target.width / 2) + (sender.width * 2)) * (target.direction), sender.height);
                if (Collision.SolidCollision(destination, sender.width, sender.height))
                    return;

                sender.direction = target.direction;
                sender.Teleport(destination, 1);
            }
            else
            {
                if (Collision.SolidCollision(Main.MouseWorld, sender.width, sender.height))
                    return;

                sender.Teleport(Main.MouseWorld, 1);
            }

            plr.OnRightClick -= Plr_OnRightClick;
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/TheWorld/TheWorldIdle", 8, 10, true);
        }
    }
}
