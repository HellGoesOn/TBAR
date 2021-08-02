using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Extensions;
using TBAR.Helpers;
using TBAR.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Chicken
{
    sealed class FireballFart : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.ToggleModifierDependency();
            projectile.magic = true;
            projectile.width = projectile.height = 2;
            projectile.timeLeft = 20;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.25f;

            //if (projectile.timeLeft % 2 == 0)
                for (int i = 0; i < 3; i++)
                {
                    int dust = Dust.NewDust(projectile.position, 2, 2, DustID.Fire, 0, 0, 0);
                    Main.dust[dust].scale = 1f + (0.1f * i);
                    Main.dust[dust].noGravity = true;
                }
        }

        public override string Texture => Textures.EmptinessPath;
    }

    public class Fireball : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.friendly = true;
            projectile.width = projectile.height = 8;
            projectile.timeLeft = 180;
            fade = 0.8f;
        }

        public override void AI()
        {
            /*for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, 8, 8, DustID.Fire, 0, 0, 0);
                Main.dust[dust].scale = 1.3f;
                Main.dust[dust].noGravity = true;
            }*/

            if (!reverse)
            {
                if((fade += 0.025f) > 1f)
                        reverse = true;
            }    
            else
            {
                if ((fade -= 0.025f) < 0.45f)
                    reverse = false;
            }
        }

        public override void Kill(int timeLeft)
        {
            DrawHelper.CircleDust(projectile.Center - projectile.velocity, projectile.velocity, DustID.Fire, 1, 8);
            for (int i = 0; i < 4; i++)
            {
                Vector2 off = new Vector2(Main.rand.Next(projectile.width + 1), Main.rand.Next(projectile.height + 1));
                Projectile.NewProjectile(projectile.position + off, (projectile.velocity * 0.65f).RotatedByRandom(0.5), ModContent.ProjectileType<FireballFart>(), 0, 0, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(5);
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft >= 179)
                return;

            Texture2D tex = Main.projectileTexture[projectile.type];

            Vector2 origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);

            SpriteEffects sfx1 = Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects sfx2 = Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects sfx3 = Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, projectile.Center - projectile.velocity * 2.5f * fade - Main.screenPosition, null, Color.White * 0.25f * fade, projectile.velocity.ToRotation() - MathHelper.PiOver2, origin, 1.75f, sfx1, 0f);
            spriteBatch.Draw(tex, projectile.Center - projectile.velocity * 1.5f * fade - Main.screenPosition, null, Color.White * 0.5f * fade, projectile.velocity.ToRotation() - MathHelper.PiOver2, origin, 1.5f, sfx2, 0f);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * fade, projectile.velocity.ToRotation() - MathHelper.PiOver2, origin, 1.25f, sfx3, 0f);
        }

        private float fade;
        private bool reverse;
    }
}
