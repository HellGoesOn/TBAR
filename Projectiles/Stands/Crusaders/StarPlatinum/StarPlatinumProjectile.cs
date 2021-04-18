using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using Terraria;

namespace TBAR.Projectiles.Stands.Crusaders.StarPlatinum
{
    public enum SPStates
    {
        Summon = 1,
        Idle,
        Despawn,
        Punch,
        Barrage
    }

    public class StarPlatinumProjectile : PunchGhostProjectile
    {

        public float Offset { get; set; }
        public bool ReverseOffsetGain { get; set; }

        public StarPlatinumProjectile() : base("Star Platinum")
        {

        }

        public override void AddStates(Projectile projectile)
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

            StandState barrageState = new StandState(path + "SPRush_Middle", 4, 15, true, 180);
            barrageState.OnStateBegin += delegate
            {
                PunchDirection = Owner.Center.DirectTo(MousePosition, Owner.width + 16 * Range);
                Barrage = PunchBarrage.CreateBarrage(path + "StarFist", projectile, projectile.Center.DirectTo(MousePosition, 24f), 60, path + "StarFistBack");
            };

            barrageState.OnStateUpdate += delegate
            {
                SpriteFX = Barrage.Center.X < projectile.Center.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            };

            barrageState.OnStateEnd += delegate
            {
                PunchDirection = Vector2.Zero;
                Barrage = null;
            };

            States.Add((int)SPStates.Summon, summon);
            States.Add((int)SPStates.Idle, idle);
            States.Add((int)SPStates.Despawn, despawn);
            States.Add((int)SPStates.Punch, punchState);
            States.Add((int)SPStates.Barrage, barrageState);

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
                DrawDefault(spriteBatch, projectile.Center + offset, AuraColor * 0.5f, SpriteFX);
            }

            DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX);
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
            return false;
        }

        private void OnSummon(StandState sender)
        {
            TBAR.Instance.PlayVoiceLine("Sounds/StarPlatinum/SP_Call");
            TBAR.Instance.PlaySound("Sounds/StarPlatinum/SP_Call");
        }

        private void Summon(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Opacity < 1f)
                Opacity += 0.05f;

            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        private void Despawn(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Opacity -= 0.05f;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-12 * Owner.direction, -12), 0.12f);
        }

        private void OnDespawnEnd(StandState sender)
        {
            projectile.Kill();
        }

        private void Idle(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        protected override int PunchAnimationIDOffset()
        {
            return Main.rand.Next(2);
        }

        public override bool CanPunch => State == (int)SPStates.Idle;

        protected override int PunchState => (int)SPStates.Punch;
    }
}
