using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using Terraria;

namespace TBAR.Projectiles.Stands.Crusaders.StarPlatinum
{
    public class StarPlatinumProjectile : PunchGhostProjectile
    {
        public enum SPStates
        {
            Summon = 1,
            Idle,
            Despawn,
            Punch
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

            string path = "Projectiles/Stands/Crusaders/StarPlatinum/";

            StandState summon = new StandState(path + "SPSummon", 10, 15);

            summon.OnStateBegin += OnSummon; 
            summon.OnStateUpdate += Summon;
            summon.OnStateEnd += delegate { SetState((int)SPStates.Idle); };

            StandState despawn = new StandState(path + "SPDespawn", 6, 12);
            despawn.OnStateUpdate += Despawn;
            despawn.OnStateEnd += OnDespawnEnd;

            StandState idle = new StandState(path + "SPIdle", 14, 15, true);
            idle.OnStateUpdate += Idle;

            SpriteAnimation punchMidLeft = new SpriteAnimation(path + "SPPunch_Middle_LeftHand", 3, 10);
            SpriteAnimation punchMidRight = new SpriteAnimation(path + "SPPunch_Middle_RightHand", 3, 10);

            StandState punchState = new StandState(punchMidLeft, punchMidRight);
            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            States.Add((int)SPStates.Summon, summon);
            States.Add((int)SPStates.Idle, idle);
            States.Add((int)SPStates.Despawn, despawn);
            States.Add((int)SPStates.Punch, punchState);

            SetState((int)SPStates.Summon);
        }

        public override void AI()
        {
            base.AI();

            if (Math.Abs(Offset) > 4)
                ReverseOffsetGain = !ReverseOffsetGain;

            Offset += ReverseOffsetGain ? -0.1f : 0.1f;
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
                SetState((int)SPStates.Despawn);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            return false;
        }

        private void OnSummon(StandState sender)
        {
            TBAR.Instance.PlayVoiceLine("Sounds/StarPlatinum/SP_Call");
            TBAR.Instance.PlaySound("Sounds/StarPlatinum/SP_Call");
        }

        private void Summon(StandState sender)
        {
            if(Opacity < 1f)
                Opacity += 0.05f;

            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        private void Despawn(StandState sender)
        {
            Opacity -= 0.05f;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-12 * Owner.direction, -12), 0.12f);
        }

        private void OnDespawnEnd(StandState sender)
        {
            projectile.Kill();
        }

        private void Idle(StandState sender)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        protected override int PunchAnimationIDOffset()
        {
            return Main.rand.Next(2);
        }

        protected override int PunchState() => (int)SPStates.Punch;
    }
}
