using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Extensions;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace TBAR.Projectiles.Stands.Crusaders.Hierophant
{
    public class MILFHunterTendril : ModProjectile
    {
        public const int TendrilLength = 15;

        private int childeIndex;

        private bool hasExtended;

        private float drawAngle;

        private float angle;

        private float baseAngle;

        private float turnRate;

        private float destinationAngle;

        public bool flipped;

        private int deadAss;

        private int previousSegmentIndex;

        public override void SetDefaults()
        {
            projectile.timeLeft = 40;

            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;

            projectile.tileCollide = false;

            projectile.penetrate = -1;
            childeIndex = -1;
            previousSegmentIndex = -1;
            deadAss = 0;
        }

        // Btw no fucking clue if this will work
        public override void AI()
        {
            projectile.netUpdate = true;

            if (projectile.timeLeft <= 10)
                deadAss--;
            else if (projectile.timeLeft >= 30)
                deadAss++;

            var magicTrick = (float)(deadAss / 10.0f);

            if (!hasExtended)
            {
                baseAngle = projectile.velocity.ToRotation();

                if (!flipped)
                {
                    angle = baseAngle - MathHelper.PiOver2 * (0.09f * ((TendrilLength + 1) - projectile.ai[0]));
                    destinationAngle = baseAngle + MathHelper.PiOver2 * 1.15f + (0.0085f * (TendrilLength + 1 - projectile.ai[0]));
                }
                else
                {
                    angle = baseAngle + MathHelper.PiOver2 * (0.09f * ((TendrilLength + 1) - projectile.ai[0]));
                    destinationAngle = baseAngle - MathHelper.PiOver2 * 1.15f - (0.0085f * (TendrilLength + 1 - projectile.ai[0]));
                }

                hasExtended = true;

                if (projectile.ai[0] > 0)
                    childeIndex = Projectile.NewProjectile(projectile.Center + projectile.velocity, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0] - 1, projectile.ai[1]);

                if (childeIndex != -1 && Childe.modProjectile is MILFHunterTendril tendril && tendril != null)
                {
                    tendril.flipped = flipped;
                    tendril.previousSegmentIndex = projectile.whoAmI;
                }
            }

            angle = MathHelper.Lerp(angle, destinationAngle, turnRate * ((TendrilLength + 1) - projectile.ai[0]));

            /*if (turnRate <= 0.009f)
                turnRate += 0.0008f;*/

            if (projectile.timeLeft <= 10 || projectile.timeLeft >= 30)
                turnRate = 0.0008f;
            else
                turnRate = 0.012f;

            var offset = new Vector2((12 * ((TendrilLength + 1) - projectile.ai[0])) * magicTrick, 0).RotatedBy(angle);

            if (Parent != null)
                projectile.Center = Parent.Center + offset;
            /*else if (Childe != null)
                projectile.Center = Childe.Center - offset;*/

            projectile.velocity *= 0f;
        }

        public override void Kill(int timeLeft)
        {
            for(int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.EmeraldBolt, 0, 0);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var tex = Textures.HierophantWhip;

            if (childeIndex != -1)
            {
                drawAngle = projectile.Center.DirectTo(Childe.Center).ToRotation();
            }
            else if(previousSegmentIndex != -1)
            {
                drawAngle = projectile.Center.DirectTo(PreviousSegment.Center).ToRotation();
            }

            float rotation = drawAngle + MathHelper.PiOver2;

            if (projectile.ai[0] == 0)
            {
                rotation += MathHelper.TwoPi;
                tex = Textures.HierophantWhipEnd;
            }

            var shader = TBAR.Instance.GetEffect("Effects/HieroShader2");

            var imageSize2 = new Vector2(86);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            shader.GraphicsDevice.Textures[1] = Textures.HieroMask;
            shader.Parameters["frame"].SetValue(new Vector4(0, 0, 5, 28));
            shader.Parameters["offset"].SetValue(new Vector2(0, 0));
            shader.Parameters["imageSize"].SetValue(new Vector2(5, 28));
            shader.Parameters["imageSize2"].SetValue(imageSize2 / 5);
            shader.Parameters["pixelation"].SetValue(100);

            shader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, rotation, new Vector2(2.5f, 14f), 1f, SpriteEffects.None, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override string Texture => Textures.EmptinessPath;

        public Projectile Parent
        {
            get
            {
                return Main.projectile[(int)projectile.ai[1]];
            }
        }

        public Projectile Childe
        {
            get
            {
                if (childeIndex == -1)
                    return null;

                return Main.projectile[childeIndex];
            }
        }

        private Projectile PreviousSegment => Main.projectile[previousSegmentIndex];
    }
}
