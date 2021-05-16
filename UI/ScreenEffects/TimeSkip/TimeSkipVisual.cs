using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TBAR.TimeSkip;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class TimeSkipVisual
    {
        /// <summary>
        /// Creates a new instance and adds it to the queue
        /// </summary>
        /// <returns>Reference to that instance</returns>
        public static TimeSkipVisual Start()
        {
            TimeSkipVisual result = new TimeSkipVisual();

            if (Main.netMode != NetmodeID.Server)
                TBAR.TimeSkipManager.VisualEffects.Add(result);

            return result;
        }

        public TimeSkipVisual()
        {
            Animation =
                new SpriteAnimation("Textures/TimeSkipVFX", 22, 30)
                { Active = true };
        }

        public void DrawEffect(SpriteBatch spriteBatch)
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

        public void Update()
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
