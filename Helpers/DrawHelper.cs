using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace TBAR.Helpers
{
    public static class DrawHelper
    {
        public static void DrawInputButtonKeyboard(string buttonName, SpriteBatch spriteBatch, Vector2 position, string description = "", float opacity = 1f)
        {
            spriteBatch.Draw(Textures.KeyboardInput, position, null, Color.White * opacity, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            Utils.DrawBorderString(spriteBatch, buttonName, position + new Vector2(16, 6), Color.Yellow * opacity, .5f);
            Utils.DrawBorderString(spriteBatch, description, position + new Vector2(50, 10), Color.Yellow * opacity);
        }

        public static void CircleDust(Vector2 pos, Vector2 vel, int dustID, float width = 2, float height = 8, float scale = 1.55f, float count = 25.0f)
        {
            for (int k = 0; (double)k < (double)count; k++)
            {
                Vector2 vector2 = (Vector2.UnitX * 0.0f + -Vector2.UnitY.RotatedBy((double)k * (6.22 / (double)count), new Vector2()) * new Vector2(width, height)).RotatedBy((double)vel.ToRotation(), new Vector2());
                int dust = Dust.NewDust(pos - new Vector2(0f, 4f), 1, 1, dustID, 0f, 0f, 200, Scale: scale);
                Main.dust[dust].scale = scale;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = pos + vector2;
                Main.dust[dust].velocity = vel * 0.0f + vector2.SafeNormalize(Vector2.UnitY) * 1.0f;
            }
        }

        public static void DrawBorderedRectangle(Vector2 position, int width, int height, Color color, Color borderColor, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
                 (
                     Main.magicPixel,
                     position,
                     new Rectangle(0, 0, width, height),
                     color,
                     0f,
                     Vector2.Zero,
                     1f,
                     SpriteEffects.None,
                     1
                 );

            #region Draw Borders
            spriteBatch.Draw
                     (
                         Main.magicPixel,
                         position,
                         new Rectangle(0, 0, 2, height),
                         borderColor,
                         0f,
                         Vector2.Zero,
                         1f,
                         SpriteEffects.None,
                         1
                     );
            spriteBatch.Draw
                     (
                         Main.magicPixel,
                         position,
                         new Rectangle(0, 0, width, 2),
                         borderColor,
                         0f,
                         Vector2.Zero,
                         1f,
                         SpriteEffects.None,
                         1
                     );
            spriteBatch.Draw
                     (
                         Main.magicPixel,
                         position + new Vector2(width - 2, 0),
                         new Rectangle(0, 0, 2, height),
                         borderColor,
                         0f,
                         Vector2.Zero,
                         1f,
                         SpriteEffects.None,
                         1
                     );
            spriteBatch.Draw
                     (
                         Main.magicPixel,
                         position + new Vector2(0, height - 2),
                         new Rectangle(0, 0, width, 2),
                         borderColor,
                         0f,
                         Vector2.Zero,
                         1f,
                         SpriteEffects.None,
                         1
                     );
            #endregion
        }

        public static void DrawRectangle(Vector2 position, int width, int height, Color color, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
                 (
                     Main.magicPixel,
                     position,
                     new Rectangle(0, 0, width, height),
                     color,
                     0f,
                     Vector2.Zero,
                     1f,
                     SpriteEffects.None,
                     1
                 );
        }

        public static void Line(SpriteBatch sb, Vector2 a, Vector2 b, float thickness, Color cl)
        {
            Vector2 tan = (b - a);
            float rot = (float)Math.Atan2(tan.Y, tan.X);
            Vector2 scale = new Vector2(tan.Length(), thickness);
            Vector2 middleOrigin = new Vector2(0, Textures.Laser.Height / 2f);

            SpriteEffects sprfx = SpriteEffects.None;

            sb.Draw(Textures.Laser, a - Main.screenPosition, null, cl, rot, middleOrigin, scale, sprfx, 0f);
        }
    }
}
