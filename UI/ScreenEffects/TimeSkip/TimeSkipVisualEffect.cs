using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TBAR.TimeSkip;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class TimeSkipVisualEffect
    {
        /// <summary>
        /// Creates a new instance and adds it to the queue
        /// </summary>
        public static void Start()
        {
            if(Main.netMode != NetmodeID.Server)
                TBAR.TimeSkipManager.VisualEffects.Add(new TimeSkipVisualEffect());
        }

        public TimeSkipVisualEffect()
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
