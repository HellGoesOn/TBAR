using Microsoft.Xna.Framework;
using System;
using TBAR.Enums;
using TBAR.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles
{
    // TO-DO: Rename to something less retarded
    public class SliceProjectile : ModProjectile
    {
        public static void Create(Entity target, EntityType type, int damage, float speed = 0.06f)
        {
            Projectile.NewProjectile(target.Center, new Vector2(speed, 0), ModContent.ProjectileType<SliceProjectile>(), damage, 0, Main.myPlayer, (int)target.whoAmI, (int)type);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laceration");
        }

        public override void SetDefaults()
        {
            hasHit = false;
            angle = 0;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.netUpdate = true;
            emitterPosition = projectile.Center - new Vector2(0, Target.height);
            projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !hasHit && angle >= MathHelper.PiOver2 * 0.9f && target == Target;
        }

        public override bool CanHitPvp(Player target)
        {
            return !hasHit && angle >= MathHelper.PiOver2 * 0.9f && target == Target;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.penetrate++;
            hasHit = true;
            damage = projectile.damage;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            projectile.penetrate++;
            hasHit = true;
            damage = projectile.damage;
        }

        public override void AI()
        {
            if (Target != null)
            {
                projectile.width = Target.width;
                projectile.height = Target.height;

                if (angle >= MathHelper.PiOver2 || !Target.active)
                    projectile.Kill();

                this.projectile.Center = Target.Center;

                float radius = (float)Math.Sqrt(Target.width * Target.width + Target.height * Target.height);

                emitterPosition = projectile.BottomRight - new Vector2(0, radius * 0.75f).RotatedBy(-angle);

                angle += Speed;

                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(emitterPosition, 0, 0, DustID.Blood, 0, 0);

                    Vector2 oldPos = projectile.BottomRight - new Vector2(0, radius * 0.75f).RotatedBy(-(angle - Speed));
                    Vector2 olderPos = projectile.BottomRight - new Vector2(0, radius * 0.75f).RotatedBy(-(angle - Speed * 2));

                    if (i < 3)
                    {
                        Main.dust[dust].scale = 1.2f;
                        Main.dust[dust].velocity = oldPos.DirectTo(olderPos, 5).RotatedByRandom(1.2f);
                    }
                    else
                    {
                        Main.dust[dust].scale = 1.5f;
                        Main.dust[dust].velocity = emitterPosition.DirectTo(oldPos, 4);
                    }
                    Main.dust[dust].velocity += Target.velocity;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        private Entity Target
        {
            get
            {
                switch(EType)
                {
                    case EntityType.Item:
                        return Main.item[Index];
                    case EntityType.Npc:
                        return Main.npc[Index];
                    case EntityType.Player:
                        return Main.player[Index];
                    case EntityType.Projectile:
                        return Main.projectile[Index];
                    default:
                        throw new System.Exception("Why the fuck did you parse non-existant value as a entity type?");
                }
            }
        }

        private bool hasHit;

        private float angle;

        private float Speed => projectile.velocity.X;

        private Vector2 emitterPosition;

        private int Index => (int)projectile.ai[0];

        private EntityType EType => (EntityType)(projectile.ai[1]);

        public sealed override string Texture => Textures.EmptinessPath;
    }
}
