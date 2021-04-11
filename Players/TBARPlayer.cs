using System;
using System.Reflection;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public static TBARPlayer Get(Player player) => player.GetModPlayer<TBARPlayer>();

        public static TBARPlayer Get() => Get(Main.LocalPlayer);

        public override void OnEnterWorld(Player player)
        {
            if (Stand != null)
                Main.NewText(Stand.StandName);
        }

        public override void PostUpdate()
        {
            if (HasActiveStand)
            {
                UpdateInputs();

                if (ActiveStandProjectile.modProjectile == null || !(ActiveStandProjectile.modProjectile is Stand))
                    KillStand();

            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound()
            {
                {"StandName", SaveStand() }
            };

            Stand = null;

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            LoadStand(tag);
        }

        public void KillStand()
        {
            ActiveStandProjectile = null;
        }

        private string SaveStand()
        {
            if (!IsStandUser)
                return "None";

            return Stand.GetType().FullName;
        }

        private void LoadStand(TagCompound tag)
        {
            Stand = null;

            if (tag.GetString("StandName") == "None")
                return;

            if (tag.ContainsKey("StandName"))
            {
                Type type = Assembly.GetAssembly(typeof(Stand)).GetType(tag.GetString("StandName"));

                if (type != null)
                    Stand = (Stand)Activator.CreateInstance(type);
            }
        }

        public Projectile ActiveStandProjectile { get; set; }

        public bool HasActiveStand => ActiveStandProjectile != null;

        public Stand Stand { get; set; }

        public bool IsStandUser => Stand != null;
    }
}
