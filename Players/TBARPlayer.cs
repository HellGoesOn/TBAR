using System.Collections.Generic;
using TBAR.Enums;
using TBAR.Input;
using TBAR.ScreenModifiers;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public TBARPlayer()
        {
        }

        internal int[] damageCaps;

        public bool ShatteredTime { get; set; }

        /// <summary>
        /// Easier access to TBARPlayer
        /// </summary>
        /// <returns>TBARPlayer instance for specified</returns>
        public static TBARPlayer Get(Player player) => player.GetModPlayer<TBARPlayer>();

        /// <summary>
        /// Easier access to TBARPlayer
        /// </summary>
        /// <returns>TBARPlayer instance for local player</returns>
        public static TBARPlayer Get() => Get(Main.LocalPlayer);

        public override void Initialize()
        {
            damageCaps = new int[] { 10, 10, 10, 10, 10 };
            BeamVisuals = new List<Visuals.BeamVisual>();
            InputBlockers = new List<InputBlocker>();
            CurrentComboInputs = new List<ComboInput>(10);
            ScreenModifiers = new List<ScreenModifier>();
        }

        public override void OnEnterWorld(Player player)
        {
            if (PlayerStand != null)
                Main.NewText(PlayerStand.StandName);
        }

        public override void ResetEffects()
        {
            ResetRepeatCount();
            UpdateArrowUseProgress();

            ShatteredTime = false;

            if (StyleHitCounterResetTimer > 0)
                StyleHitCounterResetTimer--;
            else
                StyleHitCounter = 0;

            if (StyleDecayTimer > 0)
                StyleDecayTimer--;
            else
            {
                RepeatCount = 0;
                StylePoints = 0;
            }

            if (IsStandUser)
            {
                if (ComboTime > 0)
                    ComboTime--;

                PlayerStand.Update();
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (TBAR.TimeStopManager.IsTimeStopped && !TBAR.TimeStopManager.HaveITimeStopped(npc))
                return false;

            if (IsUsingArrow)
                return false;

            if (TBAR.TimeSkipManager.IsTimeSkipped && TBAR.TimeSkipManager.GetEffect(x => x.Owner() == npc) == null)
                return false;

            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (TBAR.TimeStopManager.IsTimeStopped
                && !TBAR.TimeStopManager.HaveITimeStopped(proj)
                && !TBAR.TimeStopManager.HaveITimeStopped(Main.player[proj.owner]))
                return false;

            if (IsUsingArrow)
                return false;

            if (TBAR.TimeSkipManager.IsTimeSkipped && TBAR.TimeSkipManager.GetEffect(x => x.Owner() == proj) == null)
                return false;

            return base.CanBeHitByProjectile(proj);
        }

        public override void PostUpdate()
        {
            if (TBAR.TimeStopManager.IsTimeStopped && !TBAR.TimeStopManager.HaveITimeStopped(player) && !TBAR.TimeStopManager.IsMyTeamImmune(player))
            {
                player.velocity *= 0;
                player.position = player.oldPosition;
            }

            if (IsStandUser)
            {
                if (ComboTimeExpired)
                {
                    PlayerStand.HandleInputs(player, CurrentComboInputs);

                    CurrentComboInputs.Clear();
                }
            }


            foreach (InputBlocker blocker in InputBlockers)
                blocker.Duration--;

            InputBlockers.RemoveAll(x => x.Duration == 0);
        }

        public override void UpdateDead()
        {
            if (IsStandUser)
            {
                PlayerStand.KillStand();
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound()
            {
                {"StandName", SaveStand() },
                {"DamageCaps", damageCaps }
            };

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            LoadStand(tag);
            var retardedFuckingArrayBullshit = tag.GetIntArray("DamageCaps");

            if (retardedFuckingArrayBullshit.Length == 5)
                for (int i = 0; i < 5; i++)
                    damageCaps[i] = retardedFuckingArrayBullshit[i];
        }

        private string SaveStand()
        {
            if (!IsStandUser)
                return "None";

            return PlayerStand.StandName;
        }

        private void LoadStand(TagCompound tag)
        {
            PlayerStand = null;

            string loadedName = tag.GetString("StandName");

            if (loadedName == "None")
                return;

            Stand attempt = StandLoader.Instance.Get(loadedName);

            if (attempt != null)
                PlayerStand = attempt;
        }

        public Stand PlayerStand { get; set; }

        public bool IsStandUser => PlayerStand != null;
    }
}
