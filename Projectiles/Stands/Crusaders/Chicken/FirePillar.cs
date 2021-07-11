using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Components;
using TBAR.Structs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Chicken
{
    public sealed class FirePillar : ModProjectile
    {
        private HashSet<NPC> npcs;

        private SpriteAnimation anim;

        private bool hasSpawned, hasPlayedSound;

        public override void SetDefaults()
        {
            npcs = new HashSet<NPC>();

            projectile.friendly = true;
            projectile.width = 50;
            projectile.height = 120;
            projectile.tileCollide = false;
            projectile.penetrate = 1;

            hasSpawned = false;

            anim = new SpriteAnimation("Projectiles/Stands/Crusaders/Chicken/FirePillar", 11, 15);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.penetrate++;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !npcs.Contains(target);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.penetrate++;

            npcs.Add(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if(!hasPlayedSound)
            {
                hasPlayedSound = true;
                Main.PlaySound(SoundID.Item45);
            }

            anim.Update();

            if (anim.CurrentFrame == 2 && !hasSpawned)
            {
                for (int i = 0; i < 25; i++)
                {
                    int fire = Dust.NewDust(projectile.BottomLeft, 50, 0, DustID.Fire);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].scale = 1.5f;
                    Main.dust[fire].velocity = new Vector2(0, -(Main.rand.Next(45, 150) * 0.1f));
                }

                if (projectile.ai[0] > 0)
                {
                    Projectile.NewProjectile(projectile.Center + new Vector2(projectile.ai[1] * 40, 0), Vector2.Zero, ModContent.ProjectileType<FirePillar>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0] - 1, projectile.ai[1]);
                }

                hasSpawned = true;
            }
            if (anim.CurrentFrame == anim.FrameCount - 1)
            {
                projectile.Kill();
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Draw(anim.SpriteSheet, projectile.Center - Main.screenPosition, anim.FrameRect, Color.White, 0, anim.DrawOrigin, 1f, SpriteEffects.None, 0f);
        }
        public override string Texture => Textures.EmptinessPath;
    }

}
