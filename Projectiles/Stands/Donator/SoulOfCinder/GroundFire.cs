using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Components;
using TBAR.Players;
using TBAR.Structs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Donator.SoulOfCinder
{
    public sealed class GroundFire : ModProjectile
    {
        private HashSet<NPC> npcs;

        private SpriteAnimation anim;

        private bool hasSpawned, hasPlayedSound;

        public override void SetDefaults()
        {
            npcs = new HashSet<NPC>();

            projectile.friendly = true;
            projectile.width = 30;
            projectile.height = 80;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;

            hasSpawned = false;

            anim = new SpriteAnimation("Projectiles/Stands/Donator/SoulOfCinder/GroundFire", 5, 10);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(5);
            projectile.penetrate++;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !npcs.Contains(target);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(5);
            projectile.penetrate++;

            npcs.Add(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (!hasPlayedSound)
            {
                hasPlayedSound = true;
                Main.PlaySound(SoundID.Item45);
            }

            anim.Update();

            if (projectile.timeLeft <= 56 && !hasSpawned)
            {
                for (int i = 0; i < 25; i++)
                {
                    int fire = Dust.NewDust(projectile.BottomLeft, 30, 0, DustID.Fire);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].scale = 1.5f;
                    Main.dust[fire].velocity = new Vector2(0, -(Main.rand.Next(45, 150) * 0.1f));
                }

                if (projectile.ai[0] > 0)
                {
                    Projectile.NewProjectile(projectile.Center + new Vector2(projectile.ai[1] * 32, 0), Vector2.Zero, ModContent.ProjectileType<GroundFire>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0] - 1, projectile.ai[1]);
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
            spriteBatch.Draw(anim.SpriteSheet, projectile.Center + new Vector2(0, anim.DrawOrigin.Y - 6) - Main.screenPosition, anim.FrameRect, Color.White, 0, anim.DrawOrigin, 1f, SpriteEffects.None, 0f);
        }

        public override string Texture => Textures.EmptinessPath;

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);

            damage = projectile.damage;
        }
    }
}
