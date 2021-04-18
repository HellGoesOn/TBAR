using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace TBAR
{
    public class PerfectlyNormalSky : CustomSky
    {
        public override void Update(GameTime gameTime)
        {
            if (Active && Intensity < 1f)
            {
                Intensity += 0.01f;
            }
            else if (!Active && Intensity > 0f)
            {
                Intensity -= 0.01f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                for (int i = 0; i < 200; i++)
                {
                    var rect = new Rectangle(0, (int)Math.Ceiling(Main.screenHeight / 200f) * i, Main.screenWidth, (int)Math.Ceiling(Main.screenHeight / 200f));
                    spriteBatch.Draw(Main.blackTileTexture, rect, Color);
                }
            }

        }

        public override float GetCloudAlpha()
        {
            return 1f - Intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Active = false;
        }

        public override void Reset()
        {
            Active = false;
        }

        public override bool IsActive()
        {
            return Active || Intensity > 0f;
        }

        public bool Active { get; private set; }

        public float Intensity { get; private set; }

        public static Color Color { get; set; }
    }
}
