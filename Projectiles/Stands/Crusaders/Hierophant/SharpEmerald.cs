using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TBAR.Helpers;
using TBAR.Structs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Hierophant
{
    public class SharpEmerald : ModProjectile
    {
        private float thickness;
        private bool reverseThickness;
        private bool hasSetStartPos;
        private Vector2 startPos;

        private bool shouldDie;
        private bool lastBreath;

        private List<HitEntityData> hitNPCs;

        public override void SetDefaults()
        {
            projectile.timeLeft = 180;
            projectile.height = projectile.width = 8;
            projectile.friendly = true;

            thickness = 2f;

            hitNPCs = new List<HitEntityData>();
        }

        public override bool PreAI()
        {
            if(!hasSetStartPos)
            {
                projectile.extraUpdates = (int)projectile.ai[1];
                EmeraldDusts(-projectile.velocity, 2);

                hasSetStartPos = true;
                startPos = projectile.Center + projectile.velocity * 0.05f;
                Main.PlaySound(SoundID.Item21);
            }

            return base.PreAI();
        }

        private void EmeraldDusts(Vector2 off = default, int count = 7)
        {
            for (int i = 0; i < count; i++)
            {
                var momentum = Main.rand.Next(15, 35) * 0.01f;
                var pos = projectile.position + off;
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, 89, 0, -3);
                Main.dust[dust].velocity = (projectile.velocity * momentum).RotatedByRandom(0.7f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 2.25f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (hitNPCs.Count(x => x.Index == target.whoAmI) > 0)
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[0] <= 0)
            {
                projectile.friendly = false;
                projectile.penetrate++;
                shouldDie = true;
                if (!lastBreath)
                    EmeraldDusts();
                projectile.velocity *= 0f;
            }
            else
            {
                hitNPCs.Add(new HitEntityData(target.whoAmI, 0));
                projectile.penetrate++;
                projectile.ai[0]--;
            }
        }

        public override void AI()
        {
            if(!reverseThickness)
            {
                if ((thickness += 1f) >= 8f)
                    reverseThickness = true;
            }
            else
            {
                thickness -= 0.1f;
            }

            if (shouldDie)
            {
                thickness -= 0.15f;

                if(!lastBreath)
                {
                    Main.PlaySound(SoundID.Shatter, projectile.Center);
                    lastBreath = true;
                }

                if (thickness <= 0)
                    projectile.Kill();
            }
        }

        public override bool PreKill(int timeLeft)
        {
            shouldDie = true;

            return shouldDie && thickness <= 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            projectile.Center -= oldVelocity;
            shouldDie = true;

            if(!lastBreath)
            {
                EmeraldDusts(projectile.velocity);
            }
            return false;
        }

        // Do I even need to elaborate
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var tex = Main.projectileTexture[projectile.type];

            float rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            var emeraldColor = new Color(0.31f, 0.78f, 0.47f);

            if (projectile.timeLeft <= 179 && projectile.active)
            DrawHelper.Line(spriteBatch, startPos, projectile.Center, thickness, emeraldColor * 0.5f);

            if(!shouldDie)
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, rotation, new Vector2(10, 14), 0.5f, SpriteEffects.None, 1f);
        }
    }
}
