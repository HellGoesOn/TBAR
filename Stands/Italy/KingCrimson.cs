using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
using TBAR.Players.Visuals;
using TBAR.Projectiles.Stands;
using TBAR.Projectiles.Stands.Italy.KingCrimson;
using TBAR.Projectiles.Visual;
using TBAR.TimeSkip;
using TBAR.UI.ScreenEffects.TimeSkip;
using Terraria;

namespace TBAR.Stands.Italy
{
    public class KingCrimson : SingleEntityStand
    {
        public KingCrimson() : base(new KingCrimsonProjectile(), "King Crimson")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Italy/KingCrimson/KCIdle", 5, 5, true);
        }

        public override void InitializeCombos()
        {
            StandCombo timeErase = new StandCombo("Court of the Crimson King", ComboInput.Action1, ComboInput.Action2, ComboInput.Action2, ComboInput.Action1);
            timeErase.OnActivate += TimeErase_OnActivate;
            timeErase.Description = "'Rips time apart, allowing you to slip out of it.'\nImmune to damage during this effect.";

            StandCombo cut = new StandCombo("Evisceration", ComboInput.Action1, ComboInput.Up, ComboInput.Down);
            cut.OnActivate += Cut_OnActivate;
            cut.Description = "'Cuts a deep wound onto it's victims.'\nInflicts Laceration on hit targets.\nLaceration deals damage every 2 seconds.\nHitting Eviscerated enemies extends duration.";

            StandCombo donut = new StandCombo("Heart Ripper", ComboInput.Action1, ComboInput.Action2, ComboInput.Action2);
            donut.OnActivate += Donut_OnActivate;

            StandCombo offensiveSkip = new StandCombo("Time Rift", ComboInput.Up, ComboInput.Up, ComboInput.Action1);
            offensiveSkip.OnActivate += OffensiveTimeSkip;
            offensiveSkip.Description = "'Briefly cuts a rift in time, allowing you to relocate yourself instantly'.\nAfter activation, right click to teleport to the target location.";

            StandCombo barrage = new StandCombo("Barrage", ComboInput.Action2, ComboInput.Action1, ComboInput.Action2);
            barrage.OnActivate += Barrage;

            AddGlobalCombos(timeErase, offensiveSkip);

            AddNormalCombos(cut, donut, barrage);
        }

        private void Barrage(Player player)
        {
            KingCrimsonProjectile kc = ActiveStandProjectile as KingCrimsonProjectile;

            if (kc?.State == "Idle")
            {
                kc?.SetState(KCStates.Barrage.ToString());

                string path = "Projectiles/Stands/Italy/KingCrimson/";
                Projectile projectile = kc.projectile;
                int i = PunchBarrage.CreateBarrage(path + "KCFistFront", projectile, projectile.Center.DirectTo(ActiveStandProjectile.MousePosition, 24f), kc.GetBarrageDamage(), path + "KCFistBack");
                kc.Barrage = Main.projectile[i];
            }
        }

        private void Donut_OnActivate(Player player)
        {
            if (ActiveStandProjectile is KingCrimsonProjectile kc)
            {
                kc.SetState(KCStates.Donut.ToString());
            }
        }

        private void Cut_OnActivate(Player player)
        {
            if(ActiveStandProjectile is KingCrimsonProjectile kc)
            {
                kc.SetState(KCStates.Slice.ToString());
            }
        }

        private void TimeErase_OnActivate(Player player)
        {
            TBARMusic.AddTrackToQueue("Sounds/Music/KingCrimsonMusic", Global.SecondsToTicks(10));
            FakeTilesProjectile.Create(player.Center);
            TimeSkipVisual vs = TimeSkipVisual.Start();
            vs.Animation.AnimationPlay += Animation_AnimationPlay;
            TBAR.TimeSkipManager.AddEffect(new TimeSkipInstance(player, Global.SecondsToTicks(10)));
        }

        private void Animation_AnimationPlay(SpriteAnimation sender)
        {
            if (sender.CurrentFrame == sender.FrameCount / 3)
                TBAR.Instance.PlaySound("Sounds/StandAbilityEffects/BigTimeSkip");
        }

        private void OffensiveTimeSkip(Player player)
        {
            TBARPlayer plr = TBARPlayer.Get(player);

            plr.OnRightClick -= Plr_OnRightClick;
            plr.OnRightClick += Plr_OnRightClick;

            BeamVisual.AddBeamVisual(player, 20, 1200, 30, 15, Color.Crimson);
        }

        private void Plr_OnRightClick(Player sender)
        {
            TBARPlayer plr = TBARPlayer.Get(sender);

            TBAR.Instance.PlaySound("Sounds/StandAbilityEffects/TimeSkip");
            TimeSkipVisual.Start();

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

        public override DamageType StandDamageType => DamageType.Melee;

        public override string GetDamageScalingText => "12 + 120% DPS";

        public override string GetEffectiveRangeText => "2m";
    }
}
