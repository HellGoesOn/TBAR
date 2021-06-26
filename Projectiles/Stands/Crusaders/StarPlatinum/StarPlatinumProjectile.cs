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
        Barrage,
        Uppercut
    }

    public class StarPlatinumProjectile : PunchGhostProjectile
    {

        public float Offset { get; set; }
        public bool ReverseOffsetGain { get; set; }

        public StarPlatinumProjectile() : base("Star Platinum")
        {

        }

        // TO-DO: Refactor
        public override void InitializeStates(Projectile projectile)
        {
            AuraColor = new Color(1f, 0f, 1f);
            Opacity = 0f;

            string path = "Projectiles/Stands/Crusaders/StarPlatinum/";

            StandState summon = new StandState(path + "SPSummon", 10, 15);

            summon.OnStateBegin += OnSummon; 
            summon.OnStateUpdate += Summon;
            summon.OnStateEnd += delegate { SetState(SPStates.Idle.ToString()); };
            summon.Duration = 40;

            StandState despawn = new StandState(path + "SPDespawn", 6, 12);
            despawn.OnStateUpdate += Despawn;
            despawn.OnStateEnd += OnDespawnEnd;
            despawn.Duration = 40;

            StandState idle = new StandState(path + "SPIdle", 14, 15, true);
            idle.OnStateUpdate += Idle;

            SpriteAnimation upperCut = new SpriteAnimation(path + "SPDonutPunch", 15, 12);

            StandState upperCutState = new StandState(upperCut);
            upperCutState.OnStateBegin += UpperCutState_OnStateBegin;
            upperCutState.OnStateUpdate += UpperCutState_OnStateUpdate;
            upperCutState.OnStateEnd += UpperCutState_OnStateEnd;
            upperCutState.Duration = 80;

            SpriteAnimation punchMidLeft = new SpriteAnimation(path + "SPPunch_Middle_LeftHand", 3, 10);
            SpriteAnimation punchMidRight = new SpriteAnimation(path + "SPPunch_Middle_RightHand", 3, 10);

            SpriteAnimation punchUpLeft = new SpriteAnimation(path + "SPPunch_Up_LeftHand", 3, 10);
            SpriteAnimation punchUpRight = new SpriteAnimation(path + "SPPunch_Up_RightHand", 3, 10);

            SpriteAnimation punchDownLeft = new SpriteAnimation(path + "SPPunch_Down_LeftHand", 3, 10);
            SpriteAnimation punchDownRight = new SpriteAnimation(path + "SPPunch_Down_RightHand", 3, 10);

            StandState punchState = new StandState
                (punchMidLeft, punchMidRight, punchDownLeft, punchDownRight, punchUpRight, punchUpLeft);

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;
            punchState.Duration = AttackSpeed;

            StandState barrageState = new StandState(path + "SPRush_Middle", 4, 15, true);
            barrageState.OnStateBegin += BarrageState_OnStateBegin;
            barrageState.Duration = 180;

            barrageState.OnStateUpdate += delegate
            {
                if(Barrage != null)
                    SpriteFX = Barrage.Center.X < projectile.Center.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            };

            barrageState.OnStateEnd += delegate
            {
                PunchDirection = Vector2.Zero;
                Barrage = null;
                SetState(SPStates.Idle.ToString());
            };

            States.Add(SPStates.Summon.ToString(), summon);
            States.Add(SPStates.Idle.ToString(), idle);
            States.Add(SPStates.Despawn.ToString(), despawn);
            States.Add(SPStates.Punch.ToString(), punchState);
            States.Add(SPStates.Barrage.ToString(), barrageState);
            States.Add(SPStates.Uppercut.ToString(), upperCutState);

            SetState(SPStates.Summon.ToString());
        }

        private void UpperCutState_OnStateUpdate(StandState sender)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, MousePosition, 0.25f);
        }

        private void UpperCutState_OnStateEnd(StandState sender)
        {
            projectile.damage = 0;
            SetState("Idle");
        }

        private void UpperCutState_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;
            projectile.damage = GetUppercutDamage();
        }

        private void BarrageState_OnStateBegin(StandState sender)
        {
            PunchDirection = Owner.Center.DirectTo(MousePosition, Owner.width + 16 * Range);
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
            base.PostAI();

            if(Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == SPStates.Idle.ToString())
            {
                SetState(SPStates.Despawn.ToString());
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
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        protected override int PunchAnimationIDOffset()
        {
            int offset = 0;

            if (MousePosition.Y > Owner.Center.Y + 120)
                offset = 2;

            if (MousePosition.Y < Owner.Center.Y - 120)
                offset = 4;

            return Main.rand.Next(0, 2) + offset;
        }

        protected override int GetPunchDamage()
        {
            return (int)(12 + BaseDPS * 1.7f);
        }

        public override int GetBarrageDamage() => (int)(12 + BaseDPS * 1.2f);

        public int GetUppercutDamage() => (int)(60 + BaseDPS * 6.66f);

        public override bool CanPunch => State == SPStates.Idle.ToString();

        protected override string PunchState => SPStates.Punch.ToString();
    }
}
