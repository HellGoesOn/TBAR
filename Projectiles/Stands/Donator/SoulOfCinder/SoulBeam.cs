using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using TBAR.Helpers;
using TBAR.Input;
using TBAR.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Donator.SoulOfCinder
{
    public class SoulBeam : ModProjectile
    {
        public bool heldButton;

        private float range = 0;

        private float ballScale = 0.6f;

        private float angle = 0;

        private float thickness = 18;

        private readonly int _timeLeft = 200;

        public override void SetDefaults()
        {
            projectile.height = projectile.width = 18;
            projectile.timeLeft = _timeLeft;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.immune[projectile.owner] = 8;
            TBARPlayer.Get(Owner).AddStylePoints(8);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            heldButton = false;
            projectile.netUpdate = true;

            if (range < 100)
                range += 2;

            if (TBARInputs.ComboButton2.Current && Main.myPlayer == Owner.whoAmI)
            {
                heldButton = true;
                projectile.timeLeft = 78;
            }

            int id = (int)projectile.ai[0];

            projectile.Center = Main.projectile[id].Center - new Vector2(Owner.direction * -40, 36);

            if(projectile.timeLeft == _timeLeft)
                angle = projectile.velocity.ToRotation();

            if (Main.myPlayer == Owner.whoAmI)
            {
                angle = Utils.AngleLerp(angle, (Main.MouseWorld - projectile.Center).ToRotation(), 0.05f);

                projectile.velocity = new Vector2(range, 0).RotatedBy(angle);
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(projectile.Center, 0, 0, DustID.AncientLight, 0, 0);
                Main.dust[dust].position = projectile.Center + new Vector2(-80, Main.rand.Next(-14, 15)).RotatedBy(angle);
                Main.dust[dust].velocity = (projectile.velocity / range) * Main.rand.Next(75, 200);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1f;
            }

            if (projectile.timeLeft <= 75)
                thickness -= 0.25f;
            else if (thickness < 18)
                thickness += 0.25f;

            if ((ballScale += 0.08f) > 1f)
                ballScale = 0.6f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var point = 0.0f;
            if (thickness <= 0)
                return false;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * range, thickness, ref point);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (thickness <= 0)
                return;

            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.CornflowerBlue * 0.5f, projectile.rotation, new Vector2(16), ballScale + 0.6f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.Cyan * 0.6f, projectile.rotation, new Vector2(16), ballScale, SpriteEffects.None, 1f);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.White * 1f, projectile.rotation, new Vector2(16), ballScale - 0.6f, SpriteEffects.None, 1f); ;

            Vector2 end = projectile.Center + projectile.velocity * range;
            DrawHelper.Line(spriteBatch, projectile.Center, end, thickness * 1.6f, Color.CornflowerBlue * 0.4f);
            DrawHelper.Line(spriteBatch, projectile.Center, end, thickness, Color.Cyan * 0.5f);
            DrawHelper.Line(spriteBatch, projectile.Center, end, thickness * 0.4f, Color.White * 0.9f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(angle);
            writer.Write(heldButton);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            angle = reader.ReadSingle();
            heldButton = reader.ReadBoolean();
        }

        public Player Owner => Main.player[projectile.owner];
    }
}
