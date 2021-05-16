using TBAR.TimeStop;
using TBAR.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Worlds
{
    public class TBARWorld : ModWorld
    {
        public override void PostDrawTiles()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                TBAR.TimeStopManager.Update();
                TBAR.TimeSkipManager.UpdateVisuals();
                TBAR.TimeSkipManager.Update(x => x.Duration <= 0);
                UIManager.Instance.StandAlbumLayer?.State?.Card?.Idle?.Update();
            }
        }

        public override void PreUpdate()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.dedServ)
            {
                TBAR.TimeStopManager.Update();
                TBAR.TimeSkipManager.UpdateVisuals();
                TBAR.TimeSkipManager.Update(x => x.Duration <= 0);
                UIManager.Instance.StandAlbumLayer?.State?.Card?.Idle?.Update();
            }
        }
    }
}
