using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TBAR.Projectiles.Stands
{
    public class FakePunchData
    {
        public FakePunchData(string path, string altPath)
        {
            TexturePath = path;
            AltTexturePath = altPath;

            float x = -Main.rand.NextFloat(36f, 58f);
            float y = Main.rand.NextFloat(-24f, 24f);

            RelativePosition = new Vector2(x, y);

            Speed = (float)(Main.rand.Next(45, 76) / 10);

            IsFlipped = Main.rand.NextBool();

            UseAltTexture = Main.rand.NextBool();

            Opacity = 1f;
            Active = true;
        }

        public void Update()
        {
            RelativePosition += new Vector2(Speed, 0);
            Opacity -= 0.05f;

            if (RelativePosition.X > 32)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 startPoint, Vector2 destination)
        {
            if (TexturePath == null || TexturePath == "")
                return;

            Texture2D texture = TBAR.Instance.GetTexture(TexturePath);

            if (AltTexturePath != "" && AltTexturePath != null && UseAltTexture)
                texture = TBAR.Instance.GetTexture(AltTexturePath);

            int textureOffset = IsFlipped ? 1 : 0;

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 4);

            Rectangle sourceRect = new Rectangle(0, (int)(texture.Height * 0.5f) * textureOffset, (int)texture.Width, (int)texture.Height / 2);
            spriteBatch.Draw(texture, startPoint + RelativePosition.RotatedBy(destination.ToRotation()) - Main.screenPosition, sourceRect, Color.White * Opacity, destination.ToRotation(), origin, 1.1f, SpriteEffects.FlipHorizontally, 1f);

        }

        public float Opacity { get; set; }

        public bool Active { get; set; }

        public float Speed { get; }

        public bool UseAltTexture { get; }

        public bool IsFlipped { get; }

        public Vector2 RelativePosition { get; set; }

        public string TexturePath { get; }

        public string AltTexturePath { get; }
    }
}
