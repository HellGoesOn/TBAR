using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Backgrounds
{
    public class SpaceUp : ModSurfaceBgStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
        }

        public override bool ChooseBgStyle()
        {
            return TBAR.TimeSkipManager.IsTimeSkipped;
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            if (TBAR.TimeSkipManager.IsTimeSkipped)
            {
                if(fade < 1f)
                fade += 0.05f;
            }
            else if(fade > 0)
                fade -= 0.05f;

            spriteBatch.Draw(Textures.FancySky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * fade);

            return false;
        }

        public float fade = 0;
    }

    public class SpaceDown : ModUgBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return TBAR.TimeSkipManager.IsTimeSkipped;
        }

        public override void FillTextureArray(int[] textureSlots)
        {
        }
    }
}
