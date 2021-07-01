using TBAR.Extensions;
using TBAR.Players;
using TBAR.Stands;
using TBAR.UI.Elements;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace TBAR.UI
{
    public class StandStatPanel : UIPanel
    {
        public StandStatPanel()
        {
            Width.Set(200, 0);
            Height.Set(80, 0);
        }

        public void UpdateStats()
        {
            this.RemoveAllChildren();

            Stand stand = TBARPlayer.Get().PlayerStand;

            if (stand == null)
                return;

            /*UIDamageStat damage = new UIDamageStat(stand.StandDamageType, false);
            int dmg = Main.LocalPlayer.HeldItem.GetDamageData(Main.LocalPlayer).DPS;
            damage.DamageScaleText.SetText(dmg.ToString());

            damage.Left.Set(10, 0);
            damage.Top.Set(10, 0);
            this.Append(damage);*/
        }
    }
}
