using Microsoft.Xna.Framework;
using TBAR.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Donator.SoulOfCinder
{
    public class SOCExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.damage = 1000;
            projectile.ChangeSize(300);
            projectile.tileCollide = false;
            projectile.timeLeft = 4;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            float yeetFactor = 0.0f;

            if (projectile.timeLeft >= 2)
            {
                for (int i = 0; i < 180; i++)
                {
                    int dust = Dust.NewDust(projectile.Center - new Vector2(25), 50, 50, DustID.Fire);
                    Dust d_ref = Main.dust[dust];

                    d_ref.velocity = new Vector2(15, 0).RotatedBy(yeetFactor);
                    d_ref.noGravity = true;
                    d_ref.scale = 2.25f;

                    dust = Dust.NewDust(projectile.Center, 0, 0, DustID.Fire);
                    d_ref = Main.dust[dust];

                    d_ref.velocity = new Vector2(Main.rand.Next(14, 30), 0).RotatedBy(yeetFactor);
                    d_ref.noGravity = true;
                    d_ref.scale = 1.75f;
                    yeetFactor += MathHelper.Pi * 0.015f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = 1000;
        }

        public override string Texture => Textures.EmptinessPath;
    }
}
