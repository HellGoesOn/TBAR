using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
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

            GlobalCombos.Add(timeStop);
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
    }
}
