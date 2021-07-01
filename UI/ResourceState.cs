using Microsoft.Xna.Framework;
using TBAR.Players;
using TBAR.UI.Elements;
using Terraria;
using Terraria.UI;

namespace TBAR.UI
{
    public class ResourceState : UIState
    {
        //private StandStatPanel panel;

        public override void OnInitialize()
        {
            ComboTimeRunner runner = new ComboTimeRunner();
            /*
            panel = new StandStatPanel();
            panel.Top.Set(Main.screenHeight - panel.Height.Pixels - 40, 0);
            panel.Left.Set(40, 0);*/

            runner.Top.Set(40, 0);
            runner.Left.Set(Main.screenWidth / 2, 0);

            base.Append(runner);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Visible = TBARPlayer.Get().IsStandUser && TBARPlayer.Get().PlayerStand.IsActive;

            /*if (Visible)
            {
                if (!HasChild(panel))
                {
                    panel.UpdateStats();
                    this.Append(panel);
                }
            }
            else
            {
                if (HasChild(panel))
                    this.RemoveChild(panel);
            }*/
        }

        public bool Visible { get; set; }
    }
}
