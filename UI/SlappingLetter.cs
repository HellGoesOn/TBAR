using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Players;

namespace TBAR.UI
{
    public class SlappingLetter : ILetter
    {
        public float scale;

        public float Opacity { get; set; }

        public Vector2 Position { get; set; }
        public StyleRank Rank { get; set; }

        public SlappingLetter(StyleRank rank)
        {
            scale = 14f;
            Opacity = -0.25f;
            Rank = rank;
        }

        public void Update(GameTime gameTime)
        {
            if (scale > 1f)
                scale -= 8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                scale = 1f;

            Opacity += 0.4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb)
        {
            if (scale <= 1f)
                return;

            Rectangle rect = new Rectangle(0, 54 * (int)Rank, 99, 54);
            sb.Draw(Textures.StyleRanks, Position + new Vector2(49.5f, 27), rect, Color.White * Opacity, 0f, new Vector2(49.5f, 27), scale, SpriteEffects.None, 1);
        }
    }
}
