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
                TimeSkipManager.Instance.Update();
            }

            base.PostDrawTiles();
        }

        public override void PreUpdate()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.dedServ)
            {
                TimeSkipManager.Instance.Update();
            }

            base.PreUpdate();
        }
    }
}
