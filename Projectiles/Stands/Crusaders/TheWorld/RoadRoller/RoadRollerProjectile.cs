using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TBAR.Extensions;
using TBAR.Players;
using TBAR.Structs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.TheWorld.RoadRoller
{
    public class RoadRollerProjectile : ModProjectile
    {
        private bool triedToAdjustTimeLeft;

        private int genshinImpactDamage;
        private int explosiveBurstDamage;

        public static Projectile CreateRoller(Vector2 position, TheWorldProjectile parent)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(0, 10), ModContent.ProjectileType<RoadRollerProjectile>(), 0, 0, parent.projectile.owner, parent.projectile.whoAmI);

            Projectile p = Main.projectile[proj];

            if(p.modProjectile is RoadRollerProjectile roller)
            {
                roller.SetDamage(parent);

                return p;
            }

            return p;
        }

        public override void SetDefaults()
        {
            projectile.ToggleModifierDependency();
            projectile.timeLeft = 300;
            ExplodeScaling = 0f;
            projectile.width = 134;
            projectile.height = 72;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.netUpdate = true;
            projectile.velocity = new Vector2(0, 26);
            projectile.tileCollide = false;
            HitEntities = new List<HitEntityData>();
        }

        public override void AI()
        {
            projectile.netUpdate = true;

            if (!triedToAdjustTimeLeft)
            {
                if (TBAR.TimeStopManager.HaveITimeStopped(Main.player[projectile.owner]))
                {
                    projectile.timeLeft = TBAR.TimeStopManager.FindInstance(x => x.Owner() == Main.player[projectile.owner]).Duration + 2;
                }

                triedToAdjustTimeLeft = true;
            }

            HandleProjectileCollision();

            if (!HasHitSomething)
            {
                projectile.velocity = CollisionVector;
                projectile.damage = genshinImpactDamage;
            }
            else
            {
                if (Target != null)
                    projectile.Center = Target.Center;

                projectile.damage = (int)(900 + (explosiveBurstDamage * ExplodeScaling));
            }

            if (CollisionVector.Y == 0)
                HasHitSomething = true;

            if (projectile.timeLeft == 2)
            {
                GarbageExplosionPlaceholderMethod();
                projectile.ChangeSize(300);
                HitEntities.Clear();
            }
        }

        private void GarbageExplosionPlaceholderMethod()
        {
            // Play explosion sound
            Main.PlaySound(SoundID.Item15, projectile.position);
            // Smoke Dust spawn
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                int goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                goreIndex = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
            }
        }

        private void HandleProjectileCollision()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (!p.active || p.owner != projectile.owner || !p.friendly || p == projectile)
                    continue;

                if (p.Hitbox.Intersects(projectile.Hitbox) && !p.GetGlobal().HitRoadRollerInLifeTime)
                {
                    if (p.modProjectile is TheWorldProjectile tw)
                    {
                        if (tw.State == "Punch")
                        {
                            ExplodeScaling += 1f;
                            p.GetGlobal().HitRoadRollerInLifeTime = true;
                            continue;
                        }
                        else
                            continue;
                    }

                    if (p.modProjectile is PunchBarrage)
                    {
                        ExplodeScaling += 0.4f;
                        p.GetGlobal().HitRoadRollerInLifeTime = true;
                        continue;
                    }

                    ExplodeScaling += 0.1f;
                    p.GetGlobal().HitRoadRollerInLifeTime = true;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            HitTarget(target);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            HitTarget(target);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitEntities.Count(x => x.Index == target.whoAmI) > 0)
                return false;

            return base.CanHitNPC(target);
        }

        public override bool CanHitPvp(Player target)
        {
            if (HitEntities.Count(x => x.Index == target.whoAmI) > 0)
                return false;

            return base.CanHitPvp(target);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return projectile.timeLeft > 2;
        }

        private void HitTarget(Entity target)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(500);
            HasHitSomething = true;
            projectile.penetrate++;

            if(Target == null)
                Target = target;

            projectile.netUpdate = true;

            HitEntities.Add(new HitEntityData(target.whoAmI, 0));
        }

        public override void Kill(int timeLeft)
        {
            Target = null;
            HitEntities.Clear();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(explosiveBurstDamage);
            writer.Write(genshinImpactDamage);
            writer.Write(HasHitSomething);
            writer.Write(ExplodeScaling);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            explosiveBurstDamage = reader.ReadInt32();
            genshinImpactDamage = reader.ReadInt32();
            HasHitSomething = reader.ReadBoolean();
            ExplodeScaling = reader.ReadSingle();
        }

        public void SetDamage(TheWorldProjectile tw)
        {
            genshinImpactDamage = (int)(100 + (tw.BaseDPS * 0.4f));
            explosiveBurstDamage = tw.BaseDPS;
            projectile.netUpdate = true;
        }

        public float ExplodeScaling { get; set; }

        public bool HasHitSomething { get; private set; }

        public List<HitEntityData> HitEntities { get; set; }

        public Entity Target { get; set; }

        public Projectile Parent => Main.projectile[(int)projectile.ai[0]];

        public Vector2 CollisionVector => Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
    }
}
