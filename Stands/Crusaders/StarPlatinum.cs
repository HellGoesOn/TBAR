using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Projectiles.Stands.Crusaders.StarPlatinum;
using TBAR.TimeStop;
using Terraria;

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
            TimeStopManager.Instance.TryStopTime(EntityType.Player, player.whoAmI, 300);
        }
    }
}
