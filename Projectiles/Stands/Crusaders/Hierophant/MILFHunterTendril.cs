using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Extensions;
using Terraria;
using Terraria.ModLoader;

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

        public override void SetDefaults()
        {
            projectile.timeLeft = 40;

            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;

            projectile.tileCollide = false;

            projectile.penetrate = -1;
            childeIndex = -1;
            deadAss = 0;
        }

        // Btw no fucking clue if this will work
        public override void AI()
        {
            projectile.netUpdate = true;

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
                }
            }

            if (Childe != null)
            {
                drawAngle = projectile.Center.DirectTo(Childe.Center).ToRotation();
            }

            if (Childe == null && Parent != null)
            {
                drawAngle = projectile.Center.DirectTo(Parent.Center).ToRotation();
            }

            angle = MathHelper.Lerp(angle, destinationAngle, turnRate * ((TendrilLength + 1) - projectile.ai[0]));

            /*if (turnRate <= 0.009f)
                turnRate += 0.0008f;*/

            if (projectile.timeLeft <= 10 || projectile.timeLeft >= 30)
                turnRate = 0.0008f;
            else
                turnRate = 0.009f;

            if (projectile.timeLeft <= 10)
                deadAss--;
            else if(projectile.timeLeft >= 30)
                deadAss++;

            var magicTrick = (float)(deadAss / 10.0f);

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
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0, 0);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var tex = Main.projectileTexture[ModContent.ProjectileType<SharpEmerald>()];

            float angleOff = projectile.ai[0] > 0 ? MathHelper.PiOver2 : MathHelper.Pi + MathHelper.PiOver2;

            float rotation = drawAngle + angleOff;

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, rotation, new Vector2(10, 14), 0.5f, SpriteEffects.None, 1f);
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
    }
}
