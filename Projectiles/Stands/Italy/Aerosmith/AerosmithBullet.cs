using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TBAR.Players;

namespace TBAR.Projectiles.Stands.Italy.Aerosmith
{
    public class AerosmithBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            aiType = ProjectileID.Bullet;

            projectile.width = 4;
            projectile.height = 4;
            projectile.timeLeft = 120;

            projectile.extraUpdates = 1;
            projectile.friendly = true;
        }

        // Can't be bothered to use dumb vanilla drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get(Main.LocalPlayer).IsStandUser)
                return;


            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 position = projectile.Center - Main.screenPosition;

            float rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2;
            float opacity = projectile.timeLeft > 116 ? 0f : 1f;

            spriteBatch.Draw(texture, position, null, Color.White * opacity, rotation, new Vector2(1), 1f, SpriteEffects.None, 1f);
        }

        public override string Texture => "TBAR/Projectiles/Stands/Italy/Aerosmith/Bullet";
    }
}
