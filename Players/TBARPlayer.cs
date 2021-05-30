using System;
using System.Collections.Generic;
using System.Reflection;
using TBAR.Input;
using TBAR.Stands;
using TBAR.TimeStop;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public static TBARPlayer Get(Player player) => player.GetModPlayer<TBARPlayer>();

        public static TBARPlayer Get() => Get(Main.LocalPlayer);

        public override void Initialize()
        {
            BeamVisuals = new List<Visuals.BeamVisual>();
            InputBlockers = new List<InputBlocker>();
            CurrentComboInputs = new List<ComboInput>(10);
        }

        public override void OnEnterWorld(Player player)
        {
            if (PlayerStand != null)
                Main.NewText(PlayerStand.StandName);
        }

        public override void ResetEffects()
        {
            UpdateArrowUseProgress();

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
            if(TBAR.TimeStopManager.IsTimeStopped && !TBAR.TimeStopManager.HaveITimeStopped(player))
            {
                player.velocity *= 0;
                player.position = player.oldPosition;
            }

            if(IsStandUser)
            {
                if(ComboTimeExpired)
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
            if(IsStandUser)
            {
                PlayerStand.KillStand();
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound()
            {
                {"StandName", SaveStand() }
            };

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            LoadStand(tag);
        }

        private string SaveStand()
        {
            if (!IsStandUser)
                return "None";

            return PlayerStand.GetType().FullName;
        }

        private void LoadStand(TagCompound tag)
        {
            PlayerStand = null;

            if (tag.GetString("StandName") == "None")
                return;

            if (tag.ContainsKey("StandName"))
            {
                Type type = Assembly.GetAssembly(typeof(Stand)).GetType(tag.GetString("StandName"));

                if (type != null)
                    PlayerStand = (Stand)Activator.CreateInstance(type);
            }
        }

        public Stand PlayerStand { get; set; }

        public bool IsStandUser => PlayerStand != null;
    }
}
