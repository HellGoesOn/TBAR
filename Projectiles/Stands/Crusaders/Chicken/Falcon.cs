using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Chicken
{
    public class Falcon : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.ToggleModifierDependency();
            projectile.magic = true;
            projectile.width = projectile.height = 158;
            projectile.timeLeft = 500;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            fade = 0.8f;

            anim = new SpriteAnimation("Projectiles/Stands/Crusaders/Chicken/Falcon", 9, 12);
        }

        public override void AI()
        {
            /*for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, 8, 8, DustID.Fire, 0, 0, 0);
                Main.dust[dust].scale = 1.3f;
                Main.dust[dust].noGravity = true;
            }*/

            anim.Update();

            if (anim.CurrentFrame == anim.FrameCount - 1)
            {
                if (!spedUp)
                {
                    projectile.friendly = true;
                    spedUp = true;
                    projectile.velocity *= 1.5f;
                }

                Vector2 pepega = new Vector2(60, -78).RotatedBy(projectile.velocity.ToRotation());

                int upper = Dust.NewDust(projectile.Center + pepega, 8, 8, DustID.Fire, 0, 0, 0);
                Main.dust[upper].scale = 1.3f;
                Main.dust[upper].noGravity = true;
                Main.dust[upper].velocity = -projectile.velocity * 0.5f;


                Vector2 pepega2 = new Vector2(60, 78).RotatedBy(projectile.velocity.ToRotation());

                int lower = Dust.NewDust(projectile.Center + pepega2, 8, 8, DustID.Fire, 0, 0, 0);
                Main.dust[lower].scale = 1.3f;
                Main.dust[lower].noGravity = true;
                Main.dust[lower].velocity = -projectile.velocity * 0.5f;
            }

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

            if (projectile.timeLeft <= 110)
            {
                // cring inc
                for(int i = 0; i < 40; i++)
                {

                    Vector2 pos = projectile.Center + new Vector2(80, -80 + i * 4).RotatedBy(projectile.velocity.ToRotation());

                    int dust = Dust.NewDust(pos, 8, 8, DustID.Fire, 0, 0, 0);
                    Main.dust[dust].scale = 1.3f;
                    Main.dust[dust].noGravity = true;

                }

                decay++;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 10;

            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(20);

            if (!target.boss)
            {
                target.velocity = projectile.velocity * 1.5f;
            }

            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle rect = new Rectangle(decay, (int)anim.FrameSize.Y * (int)anim.CurrentFrame, (int)anim.FrameSize.X, (int)anim.FrameSize.Y);
            if (projectile.timeLeft > 110)
                rect = anim.FrameRect;

            spriteBatch.Draw(anim.SpriteSheet, projectile.Center - Main.screenPosition, rect, Color.White * fade, projectile.velocity.ToRotation() + MathHelper.Pi, anim.DrawOrigin, 1f, SpriteEffects.None, 0f);
        }

        private bool spedUp;

        private int decay;

        private float fade;
        private bool reverse;

        private SpriteAnimation anim;
    }
}
