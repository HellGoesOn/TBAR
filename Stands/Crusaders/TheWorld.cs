using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
using TBAR.Players.Visuals;
using TBAR.Projectiles.Stands;
using TBAR.Projectiles.Stands.Crusaders.TheWorld;
using TBAR.Projectiles.Stands.Crusaders.TheWorld.RoadRoller;
using TBAR.Projectiles.Visual;
using TBAR.ScreenModifiers;
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

            StandCombo slamDunk = new StandCombo("Road Roller", ComboInput.Up, ComboInput.Action1, ComboInput.Action2, ComboInput.Action3, ComboInput.Up);
            slamDunk.OnActivate += SlamDunk_OnActivate;
            slamDunk.Description = "Drops a Road Roller from hammerspace onto your Mouse Position.\nRoad Roller deals damage on impact and after it explodes";
            
            StandCombo barrage = new StandCombo("Barrage", ComboInput.Action2, ComboInput.Action1, ComboInput.Action2);
            barrage.OnActivate += Barrage;

            AddNormalCombos(slamDunk, barrage);

        }

        private void SlamDunk_OnActivate(Player player)
        {
            if (ActiveStandProjectile != null && ActiveStandProjectile is TheWorldProjectile tw)
            {
                tw.SetState("FlyUp");

                ScreenModifier holdPos = new ScreenModifier(player.Center, 15);
                ScreenModifier holdPos2 = new ScreenModifier(new Vector2(tw.MousePosition.X, player.Center.Y), 160);
                SmoothStepScreenModifier smoothStep = new SmoothStepScreenModifier(player.Center, new Vector2(tw.MousePosition.X, player.Center.Y), 0.25f, 120);

                ScreenModifier.AddModifiersToPlayer(player, holdPos, smoothStep, holdPos2);
            }

            TBARMusic.AddTrackToQueue("Sounds/Music/TWTheme", Global.SecondsToTicks(21));

            bool isTimeStopped = TBAR.TimeStopManager.IsTimeStopped;
            string path = isTimeStopped ? "" : "Sounds/TheWorld/TheWorld_ZaWarudoSFX";

            TimeStopInstance ts = new TimeStopInstance(player, Global.SecondsToTicks(20), path) { EndSoundEffect = "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX" };

            Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, player.whoAmI);

            TBAR.TimeStopManager.ForceStop(ts);
        }

        private void Barrage(Player player)
        {
            TheWorldProjectile tw = ActiveStandProjectile as TheWorldProjectile;

            if (tw?.State == "Idle")
            {
                tw?.SetState(TWStates.Barrage.ToString());

                string path = "Projectiles/Stands/Crusaders/TheWorld/";
                Projectile projectile = tw.projectile;
                int i = PunchBarrage.CreateBarrage(path + "TWRush", projectile, projectile.Center.DirectTo(ActiveStandProjectile.MousePosition, 24f), tw.GetBarrageDamage(), path + "TWRushBack");
                tw.Barrage = Main.projectile[i];
            }
        }

        private void TimeStop(Player player)
        {
            bool isTimeStopped = TBAR.TimeStopManager.IsTimeStopped;
            string path = isTimeStopped ? "" : "Sounds/TheWorld/TheWorld_ZaWarudoSFX";

            TimeStopInstance ts = new TimeStopInstance(player, Global.SecondsToTicks(10), path) { EndSoundEffect = "Sounds/TheWorld/TheWorld_ZaWarudoReleaseSFX" };

            if (!isTimeStopped)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, player.whoAmI);
                TBAR.Instance.PlaySound("Sounds/TheWorld/TimeStop");
            }
            else
                TBAR.Instance.PlaySound("Sounds/TheWorld/TimeResume");

            TBAR.TimeStopManager.TryStopTime(ts);
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

            if(ActiveStandProjectile is TheWorldProjectile world)
            {
                world.SetState("Idle");
            }

            plr.OnRightClick -= Plr_OnRightClick;
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/TheWorld/TheWorldIdle", 8, 10, true);
        }

        public override string GetDamageScalingText => "30 + 120% DPS";

        public override string GetEffectiveRangeText => "10m";

        public override DamageType StandDamageType => DamageType.Melee;
    }
}
