using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.TheWorld
{
    public class Knife : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 8;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects fx = Reversed ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.Pi / 2, new Vector2(4, 16), 1f, fx, 1f);
        }

        public bool Reversed { get; } = Main.rand.NextBool();
    }
}
