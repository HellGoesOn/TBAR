﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Players;

namespace TBAR.UI
{
    public class FallingLetter
    {
        public Vector2 velocity;

        private float rotation;

        public float Opacity { get; set; }

        public Vector2 Position { get; set; }
        public StyleRank Rank { get; set; }

        public FallingLetter(StyleRank rank)
        {
            Opacity = 1.75f;
            Rank = rank;
        }

        public void Update(GameTime gameTime)
        {
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity.X -= (velocity.X * 0.995f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += 200f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            rotation += (velocity.X > 0 ? 6.28f : -6.28f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Opacity -= 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle rect = new Rectangle(0, 42 * (int)Rank, 99, 42);
            sb.Draw(Textures.StyleRanks, Position + new Vector2(49.5f, 21), rect, Color.White * Opacity, rotation, new Vector2(49.5f, 21), 1f, SpriteEffects.None, 1);
        }
    }
}
