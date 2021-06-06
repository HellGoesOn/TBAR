using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;
using TBAR.Projectiles.Stands.Crusaders.TheWorld.RoadRoller;
using TBAR.Players;

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
            Range = 5f;

            string path = "Projectiles/Stands/Crusaders/TheWorld/";

            SpriteAnimation summon = new SpriteAnimation(path + "TheWorldSpawn", 7, 15);
            SpriteAnimation despawn = new SpriteAnimation(path + "TheWorldSpawn", 7, 15) { IsReversed = true };
            SpriteAnimation idle = new SpriteAnimation(path + "TheWorldIdle", 8, 10, true);
            SpriteAnimation flyUp = new SpriteAnimation(path + "TheWorldIdle", 8, 10, true, 90);
            SpriteAnimation slamDunk = new SpriteAnimation(path + "TheWorldSlamDunk", 1, 5, true, 90);
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
            { Key = TWStates.Punch.ToString() };

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            StandState flyUpState = new StandState("FlyUp", flyUp);
            flyUpState.OnStateUpdate += FlyUpState_OnStateUpdate;
            flyUpState.OnStateBegin += FlyUpState_OnStateBegin;
            flyUpState.OnStateEnd += delegate { SetState("SlamDunk"); };

            StandState slamDunkState = new StandState("SlamDunk", slamDunk);
            slamDunkState.OnStateEnd += SlamDunkState_OnStateEnd;
            slamDunkState.OnStateBegin += SlamDunkState_OnStateBegin;
            slamDunkState.OnStateUpdate += SlamDunkState_OnStateUpdate;

            StandState knifeThrowState = new StandState(throwAnimation) { Key = TWStates.KnifeThrow.ToString() };
            knifeThrowState.OnStateEnd += GoIdle;
            knifeThrowState.OnStateEnd += delegate { knifeThrowState.OnStateUpdate += ThrowingKnives; };
            knifeThrowState.OnStateUpdate += ThrowingKnives; 

            StandState barrageState = new StandState(TWStates.Barrage.ToString(), path + "TheWorldRushMiddle", 4, 15, true, 180);
            barrageState.OnStateBegin += BarrageState_OnStateBegin;

            barrageState.OnStateUpdate += delegate
            {
                if (Barrage != null)
                    SpriteFX = Barrage.Center.X < projectile.Center.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            };

            barrageState.OnStateEnd += delegate
            {
                PunchDirection = Vector2.Zero;
                Barrage = null;
                SetState(TWStates.Idle.ToString());
            };

            AddStates(summonState, despawnState, idleState, punchState, knifeThrowState, flyUpState, slamDunkState, barrageState);

            SetState(TWStates.Summon.ToString());
        }

        private void FlyUpState_OnStateBegin(StandState sender)
        {
            InputBlocker.BlockInputs(Owner, 90);
            SlamDunkPosition = new Vector2(MousePosition.X, MousePosition.Y - 400);
        }

        private void SlamDunkState_OnStateEnd(StandState sender)
        {
            GoIdle(sender);

            MyRoller = null;
        }

        private void BarrageState_OnStateBegin(StandState sender)
        {
            PunchDirection = Owner.Center.DirectTo(MousePosition, Owner.width + 16 * Range);
        }

        private void SlamDunkState_OnStateUpdate(StandState sender)
        {
            Owner.noFallDmg = true;
            if (MyRoller != null && MyRoller.modProjectile is RoadRollerProjectile roller && !roller.HasHitSomething)
            {
                projectile.Center = MyRoller.Center + new Vector2(60, -30);
                SpriteFX = SpriteEffects.None;
                Owner.direction = -1;
                Owner.Center = projectile.Center - new Vector2(0, 60);
            }
        }

        private void SlamDunkState_OnStateBegin(StandState sender)
        {
            Owner.Center = SlamDunkPosition;
            MyRoller = RoadRollerProjectile.CreateRoller(SlamDunkPosition, (projectile.modProjectile as TheWorldProjectile));
        }

        private void FlyUpState_OnStateUpdate(StandState sender)
        {
            Owner.noFallDmg = true;
            projectile.Center -= new Vector2(0, 16);
            Owner.velocity = Vector2.Zero;
            Owner.Center = projectile.Center + new Vector2(0, 32);
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
            projectile.GetGlobal().HitRoadRollerInLifeTime = false;
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get().IsStandUser)
                return;

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(4, 0).RotatedBy(MathHelper.PiOver2 * i);
                DrawDefault(spriteBatch, projectile.Center + offset, AuraColor * 0.5f, SpriteFX);
            }

            DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public int KnifeDamage => (int)(8 + BaseDPS * 0.25f);

        public Vector2 SlamDunkPosition { get; set; }

        public Projectile MyRoller { get; set; }

        protected override int GetPunchDamage() => (int)(30 + BaseDPS * 1.2f);

        public override int GetBarrageDamage() => (int)(20 + BaseDPS * 0.8f);

        public override bool CanPunch => State == TWStates.Idle.ToString() || State == TWStates.KnifeThrow.ToString();
    }

    public enum TWStates
    {
        Summon,
        Despawn,
        Idle,
        Punch,
        KnifeThrow,
        Barrage
    }
}
