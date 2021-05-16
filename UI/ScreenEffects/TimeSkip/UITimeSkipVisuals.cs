using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class UITimeSkipVisuals : UIState
    {

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach(TimeSkipVisual vfx in TBAR.TimeSkipManager.VisualEffects)
            {
                vfx.DrawEffect(spriteBatch);
            }
        }

        public bool Visible { get; set; } = true;
    }
}
