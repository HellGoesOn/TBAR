using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class TimeSkipVFX : UIVisualEffect
    {
        public TimeSkipVFX()
        {
            Animation =
                new SpriteAnimation("Textures/TimeSkipVFX", 22, 30)
                { Active = false };
        }

        public override void DrawEffect(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                   Animation.SpriteSheet,
                   new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                   Animation.FrameRect,
                   Color.White,
                   0,
                   Vector2.Zero,
                   SpriteEffects.None,
                   1f);
        }

        public override void Update()
        {
            if (Animation.Active)
            {
                Animation.Update();
                Animation.UpdateEvent();
            }
        }

        public SpriteAnimation Animation { get; }
    }
}
