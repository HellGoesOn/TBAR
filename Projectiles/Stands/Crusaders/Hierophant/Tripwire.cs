using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Hierophant
{
    public class Tripwire : ModProjectile
    {
        private readonly int maxLength = 32 * 40;

        private int length;

        private Vector2 extendPoint;

        private float moveItMoveIt;

        private bool hasSetExtendPoint;

        public override void SetDefaults()
        {
            projectile.timeLeft = 15 * 60;
            projectile.width = projectile.height = 16;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if(!hasSetExtendPoint)
            {
                extendPoint = projectile.Center + projectile.velocity.SafeNormalize(-Vector2.UnitY) * maxLength;
                hasSetExtendPoint = true;
                projectile.velocity *= 0;
            }

            if(length == maxLength)
            {
                var start = projectile.Center;
                var end = projectile.Center + new Vector2(length, 0).RotatedBy(RotationToExtendPoint);
                foreach (NPC npc in Main.npc.Where(x => x.active && x.friendly == false && x.CanBeChasedBy(projectile)))
                {
                    if(Collision.CheckAABBvLineCollision(npc.position, npc.Hitbox.Size(), start, end))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            var type = ModContent.ProjectileType<SharpEmerald>();

                            var pos = i < 3 ? start : end;
                            Projectile.NewProjectile(pos, pos.DirectTo(npc.Center, 18f).RotatedByRandom(0.035f), type, projectile.damage, 2f, projectile.owner, 0, 1);
                        }

                        projectile.Kill();
                    }
                }
            }

            if (length < maxLength)
            {
                length += 64;
            }

            if (moveItMoveIt <= 1f)
                moveItMoveIt += 0.0075f;
            else
                moveItMoveIt = -1f;

            for (int i = 0; i < 3; i++)
            {
                var pos = projectile.position - new Vector2(16, 0).RotatedBy(RotationToExtendPoint);
                var pos2 = projectile.position + new Vector2(length - 8, 0).RotatedBy(RotationToExtendPoint);
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, 89, 0, -3);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.15f;
                
                dust = Dust.NewDust(pos2, projectile.width, projectile.height, 89, 0, -3);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.15f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Shatter, projectile.Center + new Vector2(length * 0.5f, 0).RotatedBy(RotationToExtendPoint));
            for (int i = 0; i < maxLength / 10; i++)
            {
                var pos = projectile.Center + new Vector2(8 * i, 0).RotatedBy(RotationToExtendPoint);
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, 89, 0, -3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.25f;
            }
        }

        // One day HGO would use vanilla's drawing for his projectiles.
        // This was not the day
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];

            var shader = TBAR.Instance.GetEffect("Effects/HieroShader");

            var rot = RotationToExtendPoint - MathHelper.PiOver2;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            shader.GraphicsDevice.Textures[1] = Textures.HieroMask;
            shader.Parameters["frame"].SetValue(new Vector2(16, 28));
            shader.Parameters["offset"].SetValue(new Vector2(0, moveItMoveIt));
            shader.Parameters["size"].SetValue(new Vector2(250));

            shader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(texture, projectile.Center- Main.screenPosition, new Rectangle(0, 0, 8, length), Color.White, rot, new Vector2(4, 14), 1f, SpriteEffects.None, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override string Texture => "TBAR/Projectiles/Stands/Crusaders/Hierophant/Pattern";

        private float RotationToExtendPoint => projectile.Center.DirectTo(extendPoint).ToRotation();
    }
}
