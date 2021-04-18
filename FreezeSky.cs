using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace TBAR
{
    public class FreezeSky : CustomSky
	{
        private bool _isActive;
		public static Color color;

		public override void Update(GameTime gameTime)
		{
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 0f && minDepth < 20f)
			{
				var rect = new Rectangle(0, (int)Math.Ceiling(Main.screenHeight / 50f), Main.screenWidth, (int)Math.Ceiling(Main.screenHeight / 50f));
				spriteBatch.Draw(Main.blackTileTexture, rect, color);
			}
		}

		public override float GetCloudAlpha()
		{
			return 1f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._isActive = true;
		}

		public override void Deactivate(params object[] args)
		{
			this._isActive = false;
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			return this._isActive;
		}
	}
}
