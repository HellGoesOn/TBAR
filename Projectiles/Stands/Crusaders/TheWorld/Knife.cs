using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Extensions;
using TBAR.Players;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.TheWorld
{
    public class Knife : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.ToggleModifierDependency();
            projectile.width = projectile.height = 8;
            projectile.friendly = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(2);
            TBARPlayer.Get(Main.player[projectile.owner]).AddStamina(1);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            TBARPlayer.Get(Main.player[projectile.owner]).AddStylePoints(2);
            TBARPlayer.Get(Main.player[projectile.owner]).AddStamina(1);
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            if (ShouldStop)
                projectile.timeLeft++;

            if (TBAR.TimeStopManager.IsTimeStopped && !ShouldStop)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active || !npc.CanBeChasedBy(this))
                        continue;

                    if (VelocityAdjustedHitbox.Intersects(npc.Hitbox))
                        ShouldStop = true;
                }
            }
        }

        public override bool ShouldUpdatePosition() => !ShouldStop || !TBAR.TimeStopManager.IsTimeStopped;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects fx = Reversed ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.Pi / 2, new Vector2(4, 16), 1f, fx, 1f);
        }

        public Rectangle VelocityAdjustedHitbox =>
            new Rectangle(projectile.Hitbox.X + (int)projectile.velocity.X * 2,
                projectile.Hitbox.Y + (int)projectile.velocity.Y * 2,
                projectile.Hitbox.Width, projectile.Hitbox.Height);

        public bool Reversed { get; } = Main.rand.NextBool();

        public bool ShouldStop { get; set; }
    }
}
