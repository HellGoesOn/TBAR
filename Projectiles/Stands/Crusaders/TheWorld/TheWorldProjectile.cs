using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.TheWorld
{
    public class TheWorldProjectile : PunchGhostProjectile
    {
        public TheWorldProjectile() : base("theworld")
        {
        }

        protected override string PunchState => TWStates.Punch.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 4f;

            string path = "Projectiles/Stands/Crusaders/TheWorld/";

            SpriteAnimation summon = new SpriteAnimation(path + "TheWorldSpawn", 7, 15);
            SpriteAnimation despawn = new SpriteAnimation(path + "TheWorldSpawn", 7, 15) { IsReversed = true };
            SpriteAnimation idle = new SpriteAnimation(path + "TheWorldIdle", 8, 10, true);
            SpriteAnimation throwAnimation = new SpriteAnimation(path + "TheWorldKnifeThrow", 14, 15);

            SpriteAnimation punchMidLeft = new SpriteAnimation(path + "TheWorldPunchMiddle", 7, 15);
            SpriteAnimation punchMidRight = new SpriteAnimation(path + "TheWorldPunchMiddleAlt", 8, 15);

            SpriteAnimation punchUpLeft = new SpriteAnimation(path + "TheWorldPunchUp", 8, 15);
            SpriteAnimation punchUpRight = new SpriteAnimation(path + "TheWorldPunchUpAlt", 8, 15);

            SpriteAnimation punchDownLeft = new SpriteAnimation(path + "TheWorldPunchDown", 7, 15);
            SpriteAnimation punchDownRight = new SpriteAnimation(path + "TheWorldPunchDownAlt", 8, 15);

            StandState summonState = new StandState(TWStates.Summon.ToString(), summon);
            summonState.OnStateBegin += delegate { TBAR.Instance.PlayVoiceLine("Sounds/TheWorld/Call"); };
            summonState.OnStateEnd += GoIdle;
            summonState.OnStateUpdate += SummonState_OnStateUpdate;

            StandState idleState = new StandState(TWStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = new StandState(TWStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };

            StandState punchState = new StandState
                (punchMidLeft, punchMidRight, punchDownLeft, punchDownRight, punchUpRight, punchUpLeft)
            { Key = TWStates.Punch.ToString() } ;

            StandState knifeThrowState = new StandState(throwAnimation) { Key = TWStates.KnifeThrow.ToString() };
            knifeThrowState.OnStateEnd += GoIdle;
            knifeThrowState.OnStateEnd += delegate { knifeThrowState.OnStateUpdate += ThrowingKnives; };
            knifeThrowState.OnStateUpdate += ThrowingKnives;

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            AddStates(summonState, despawnState, idleState, punchState, knifeThrowState);

            SetState(TWStates.Summon.ToString());
        }

        public override void HandleImmediateInputs(ImmediateInput input)
        {
            switch (input)
            {
                case ImmediateInput.Action1:
                case ImmediateInput.Action2:
                case ImmediateInput.Action3:
                case ImmediateInput.LeftClick:
                    if (CanPunch)
                        SetState(PunchState);
                    break;
                case ImmediateInput.RightClick:
                    if (CanPunch)
                        SetState(TWStates.KnifeThrow.ToString());
                        break;
                default:
                    return;
            }
        }

        private void ThrowingKnives(StandState sender)
        {
            if (sender.CurrentAnimation.CurrentFrame == 10)
            {
                int knifeCount = 4;

                float spread = 4 * knifeCount;

                for (int i = 0; i < knifeCount; i++)
                {
                    Vector2 direction = projectile.Center.DirectTo(MousePosition, 14f);

                    float offY = spread - ((spread * 0.5f) * i);

                    Vector2 position = projectile.Center + direction * 2.2f + new Vector2(Main.rand.Next(-5, 5), -offY).RotatedBy(direction.ToRotation());

                    Projectile.NewProjectile(position, direction, ModContent.ProjectileType<Knife>(), KnifeDamage, 3.5f, Owner.whoAmI);
                }

                sender.OnStateUpdate -= ThrowingKnives;
            }
        }

        private void GoIdle(StandState sender)
        {
            SetState(TWStates.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        public override void PostAI()
        {
            base.PostAI();

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == TWStates.Idle.ToString())
            {
                SetState(TWStates.Despawn.ToString());
            }
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

        private void Idle(StandState sender)
        {
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        public int KnifeDamage => (int)(8 + BaseDPS * 0.25f);
    }

    public enum TWStates
    {
        Summon,
        Despawn,
        Idle,
        Punch,
        KnifeThrow
    }
}
