using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
using TBAR.Players.Visuals;
using TBAR.Projectiles.Stands;
using TBAR.Projectiles.Stands.Crusaders.StarPlatinum;
using TBAR.Projectiles.Visual;
using TBAR.TimeStop;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Stands.Crusaders
{
    public class StarPlatinum : SingleEntityStand
    {
        public StarPlatinum() : base(new StarPlatinumProjectile(), "Star Platinum")
        {
        }

        public override void AddCombos()
        {
            StandCombo timeStop = new StandCombo("Time Stop", ComboInput.Action1, ComboInput.Action1, ComboInput.Action2);
            timeStop.OnActivate += StopTime;

            StandCombo upperCut = new StandCombo("Upper Cut", ComboInput.Up, ComboInput.Up, ComboInput.Action1);
            upperCut.OnActivate += UpperCut_OnActivate;

            StandCombo barrage = new StandCombo("Barrage", ComboInput.Action2, ComboInput.Action1, ComboInput.Action2);
            barrage.OnActivate += Barrage;

            StandCombo offensiveTimeStop = new StandCombo("Offensive Time Stop", ComboInput.Up, ComboInput.Action1, ComboInput.Up, ComboInput.Action2);
            offensiveTimeStop.OnActivate += OffensiveTimeStop;
            offensiveTimeStop.Description = "Allows you to prepare a Quick Time Stop.\nAfter use, Right-Click on a location to teleport there.\nClicking on an Entity will cause you to teleport behind it.";

            GlobalCombos.Add(timeStop);
            GlobalCombos.Add(offensiveTimeStop);
            NormalCombos.Add(barrage);
            NormalCombos.Add(upperCut);
        }

        private void UpperCut_OnActivate(Player player)
        {
            StarPlatinumProjectile sp = ActiveStandProjectile as StarPlatinumProjectile;

            if (sp?.State == "Idle")
            {
                sp?.SetState(SPStates.Uppercut.ToString());
            }
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/StarPlatinum/SPIdle", 14, 15, true);
        }

        private void StopTime(Player player)
        {
            bool isTimeStopped = TimeStopManager.Instance.IsTimeStopped;
            string path = isTimeStopped ? "" : "Sounds/StarPlatinum/SP_TimeStopSignal";

            TimeStopInstance ts = new TimeStopInstance(player, 600, path) { EndSoundEffect = "Sounds/StarPlatinum/SP_TimeRestore" };

            if (!isTimeStopped)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TimeStopVFX>(), 0, 0, player.whoAmI);
                TBAR.Instance.PlayVoiceLine("Sounds/StarPlatinum/SP_TimeStopCall");
            }

            TimeStopManager.Instance.TryStopTime(ts);
        }

        private void Barrage(Player player)
        {
            StarPlatinumProjectile sp = ActiveStandProjectile as StarPlatinumProjectile;

            if (sp?.State == "Idle")
            {
                sp?.SetState(SPStates.Barrage.ToString());

                string path = "Projectiles/Stands/Crusaders/StarPlatinum/";
                Projectile projectile = sp.projectile;
                int i = PunchBarrage.CreateBarrage(path + "StarFist", projectile, projectile.Center.DirectTo(ActiveStandProjectile.MousePosition, 24f), sp.GetBarrageDamage(), path + "StarFistBack");
                sp.Barrage = Main.projectile[i];
            }
        }

        private void OffensiveTimeStop(Player player)
        {
            TBARPlayer plr = TBARPlayer.Get(player);

            plr.OnRightClick -= Plr_OnRightClick;
            plr.OnRightClick += Plr_OnRightClick;

            BeamVisual.AddBeamVisual(player, 30, 1200, 45, 15, Color.Yellow);
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
    }
}
