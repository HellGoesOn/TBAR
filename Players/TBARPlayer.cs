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
            CurrentComboInputs = new List<ComboInput>(10);
        }

        public override void OnEnterWorld(Player player)
        {
            if (PlayerStand != null)
                Main.NewText(PlayerStand.StandName);
        }

        public override void ResetEffects()
        {
            if (IsStandUser)
            {
                if (ComboTime > 0)
                    ComboTime--;

                PlayerStand.Update();
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (TimeStopManager.Instance.IsTimeStopped && !TimeStopManager.Instance.HaveITimeStopped(npc))
                return false;

            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (TimeStopManager.Instance.IsTimeStopped
                && !TimeStopManager.Instance.HaveITimeStopped(proj)
                && !TimeStopManager.Instance.HaveITimeStopped(Main.player[proj.owner]))
                return false;

            return base.CanBeHitByProjectile(proj);
        }

        public override void PostUpdate()
        {
            if(TimeStopManager.Instance.IsTimeStopped && !TimeStopManager.Instance.HaveITimeStopped(player))
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
