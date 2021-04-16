using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using Terraria;

namespace TBAR.Projectiles.Stands.Crusaders.StarPlatinum
{
    public class StarPlatinumProjectile : StandProjectile
    {
        public enum SPStates
        {
            None,
            Summon,
            Idle,
            Despawn
        }

        public float Offset { get; set; }
        public bool ReverseOffsetGain { get; set; }

        public StarPlatinumProjectile() : base("Star Platinum")
        {

        }

        public override void AddStates()
        {
            AuraColor = new Color(1f, 0f, 1f);
            Opacity = 0f;
            State = (int)SPStates.Summon;

            string path = "Projectiles/Stands/Crusaders/StarPlatinum/";

            SpriteAnimation summon = new SpriteAnimation(path + "SPSummon", 10, 15);
            summon.AnimationPlay += OnSummon; 
            summon.AnimationPlay += Summon;
            summon.OnAnimationEnd += delegate { State = (int)SPStates.Idle; };

            SpriteAnimation despawn = new SpriteAnimation(path + "SPDespawn", 6, 12);
            despawn.AnimationPlay += Despawn;
            despawn.OnAnimationEnd += OnDespawnEnd;

            SpriteAnimation idle = new SpriteAnimation(path + "SPIdle", 14, 15, true);
            idle.AnimationPlay += Idle;

            States.Add((int)SPStates.Summon, summon);
            States.Add((int)SPStates.Idle, idle);
            States.Add((int)SPStates.Despawn, despawn);
        }

        public override void AI()
        {
            base.AI();

            if (!ReverseOffsetGain)
            {
                if ((Offset += 0.1f) > 4)
                    ReverseOffsetGain = true;
            }
            else
            {
                if ((Offset -= 0.1f) <= 0)
                    ReverseOffsetGain = false;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get().IsStandUser)
                return;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(Offset, 0).RotatedBy(MathHelper.PiOver2 * i);
                DefaultDrawStand(spriteBatch, projectile.Center + offset, AuraColor * 0.5f, SpriteFX);
            }

            DefaultDrawStand(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public override void PostAI()
        {
            if(Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == (int)SPStates.Idle)
            {
                State = (int)SPStates.Despawn;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            return false;
        }

        private void OnSummon(SpriteAnimation sender)
        {
            TBAR.Instance.PlayVoiceLine("Sounds/StarPlatinum/SP_Call");
            TBAR.Instance.PlaySound("Sounds/StarPlatinum/SP_Call");

            sender.AnimationPlay -= OnSummon;
        }

        private void Summon(SpriteAnimation sender)
        {
            if(Opacity < 1f)
                Opacity += 0.05f;

            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        private void Despawn(SpriteAnimation sender)
        {
            Opacity -= 0.05f;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-12 * Owner.direction, -12), 0.12f);
        }

        private void OnDespawnEnd(SpriteAnimation sender)
        {
            projectile.Kill();
        }

        private void Idle(SpriteAnimation sender)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }
    }
}
