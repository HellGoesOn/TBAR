using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TBAR.Players;
using TBAR.Extensions;

namespace TBAR.Projectiles.Stands.Italy.Aerosmith
{
    public class AerosmithBomb : ModProjectile
    {
        private const int TRIGGER_DELAY = 30;

        public override void SetDefaults()
        {
            projectile.ToggleModifierDependency();
            projectile.width = 12;
            projectile.height = 12;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.friendly = true;
            HitNPC = -999;
        }

        public override void AI()
        {
            if (!HasHitGround) // We only want to rotate the bomb up to the point of where it hits the ground.
            {
                VelocityRotation = projectile.velocity.ToRotation();

                int dustIndex = Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].position = projectile.Center;
                Main.dust[dustIndex].velocity = -projectile.velocity * 0.25f;
                Main.dust[dustIndex].noGravity = true;
            }
            // Wwhen the warhead touches the ground, there is small delay before it goes kaboom


            // I want the bomb to slowly tilt towards dropping down until it goes straight down
            projectile.velocity.X *= 0.99f;

            if (projectile.velocity.Y < 10 && !HasHitGround)
                projectile.velocity.Y += 0.2f;

            if(HasHitGround)
            {
                if(ExplosionDelay < TRIGGER_DELAY)
                {
                    if(HitNPC != -999)
                    {
                        projectile.Center = Main.npc[HitNPC].Center - Offset;
                    }

                    ExplosionDelay++;
                }
                else
                {
                    /// TO DO: Implement KABOOM
                    if (ExplosionDelay < TRIGGER_DELAY + 1)
                    {
                        #region poop code
                        for (int g = 0; g < 4; g++)
                        {
                            int goreIndex = Gore.NewGore(projectile.Center, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 1.5f;
                            goreIndex = Gore.NewGore(projectile.Center, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 1.5f;
                            goreIndex = Gore.NewGore(projectile.Center, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 1.5f;
                            goreIndex = Gore.NewGore(projectile.Center, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 1.5f;
                        }
                        // Smoke Dust spawn
                        for (int i = 0; i < 100; i++)
                        {
                            int dustIndex = Dust.NewDust(projectile.Center, 0, 0, DustID.Smoke, 0f, 0f, 100, default, 2f);
                            Main.dust[dustIndex].velocity *= Main.rand.NextFloat(5f, 9f);
                        }
                        // Fire Dust spawn
                        for (int i = 0; i < 160; i++)
                        {
                            int dustIndex = Dust.NewDust(projectile.Center, 0, 0, DustID.Fire, 0f, 0f, 100, default, 3f);
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].velocity *= Main.rand.NextFloat(3f, 12f);
                            dustIndex = Dust.NewDust(projectile.Center, 0, 0, DustID.Fire, 0f, 0f, 100, default, 2f);
                            Main.dust[dustIndex].velocity *= Main.rand.NextFloat(3f, 12f);
                        }
                        #endregion


                        projectile.damage = ExplosionDamage;

                        projectile.friendly = true;
                        ExplosionDelay += 2;
                        projectile.timeLeft = 2;

                        projectile.ChangeSize(300);
                    }

                    Main.PlaySound(SoundID.Item14, projectile.position);
                }
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HasHitGround = true;
            projectile.netUpdate = true;
            projectile.Center += oldVelocity;
            projectile.velocity = Vector2.Zero;
            projectile.tileCollide = false;

            return false;
        }



        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasHitGround);
            writer.Write(HitNPC);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasHitGround = reader.ReadBoolean();
            HitNPC = reader.ReadInt32();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if(!HasHitGround)
            {
                HasHitGround = true;
                projectile.netUpdate = true;
                projectile.velocity = Vector2.Zero;
                projectile.tileCollide = false;

                projectile.friendly = false;

                HitNPC = target.whoAmI;

                Offset = target.Center - projectile.Center;
            }
        }

        // Can't be bothered to use dumb vanilla drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get(Main.LocalPlayer).IsStandUser || ExplosionDelay >= TRIGGER_DELAY)
                return;

            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 position = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(texture, position, null, Color.White, VelocityRotation, new Vector2(9, 11), 0.75f, SpriteEffects.None, 1f);
        }

        public bool HasHitGround { get; private set; }

        public int ExplosionDelay { get; private set; }

        public float VelocityRotation { get; private set; }

        public int HitNPC { get; private set; }

        public Vector2 Offset { get; private set; }

        public int ExplosionDamage { get; set; }

        public override string Texture => "TBAR/Projectiles/Stands/Italy/Aerosmith/Bomb";
    }
}
