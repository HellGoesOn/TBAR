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
        private int stopSignHitCount;

        public TheWorldProjectile() : base("theworld")
        {
        }

        protected override string PunchState => TheWorldAI.Punch.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 5f;

            AttackSpeed = 15;

            string path = "Projectiles/Stands/Crusaders/TheWorld/";

            AddAnimation(TheWorldAI.Summon, path + "Spawn", 22, 24);
            AddAnimation(TheWorldAI.Despawn, path + "Spawn", 22, 24).AsReversed();
            AddAnimation(TheWorldAI.Idle, path + "TheWorldIdle", 8, 10, true);
            AddAnimation(TheWorldAI.FlyUp, path + "TheWorldIdle", 8, 10, true);
            AddAnimation(TheWorldAI.SlamDunk, path + "TheWorldSlamDunk", 1, 5, true);
            AddAnimation(TheWorldAI.KnifeThrow, path + "TheWorldKnifeThrow", 14, 15);

            AddAnimation("PunchMid1", path + "TheWorldPunchMiddle", 7, 20);
            AddAnimation("PunchMid2", path + "TheWorldPunchMiddleAlt", 8, 20);

            AddAnimation("PunchUp1", path + "TheWorldPunchUp", 8, 20);
            AddAnimation("PunchUp2", path + "TheWorldPunchUpAlt", 8, 20);

            AddAnimation("PunchDown1", path + "TheWorldPunchDown", 7, 20);
            AddAnimation("PunchDown2", path + "TheWorldPunchDownAlt", 8, 20);
            AddAnimation(TheWorldAI.Barrage, path + "TheWorldRushMiddle", 4, 15, true);

            AddAnimation("Stop_Idle0", path + "Stop_Idle", 8, 15, true);
            AddAnimation("Stop_Idle1", path + "Stop_IdleDamaged1", 8, 15, true);
            AddAnimation("Stop_Idle2", path + "Stop_IdleDamaged2", 8, 15, true);

            AddAnimation("Stop_Attack0", path + "Stop_Attack1", 9, 15);
            AddAnimation("Stop_Attack1", path + "Stop_Attack2", 10, 15);
            AddAnimation("Stop_Attack2", path + "Stop_Attack3", 11, 15);

            AddAnimation(TheWorldAI.GrabSign, path + "Stop_GetGrounded", 20, 15);

            StandState summonState = AddState(TheWorldAI.Summon.ToString(), 60);
            summonState.OnStateBegin += SummonState_OnStateBegin;
            summonState.OnStateEnd += GoIdle;
            summonState.OnStateUpdate += SummonState_OnStateUpdate;

            StandState idleState = AddState(TheWorldAI.Idle.ToString());
            idleState.OnStateUpdate += Idle;

            StandState despawnState = AddState(TheWorldAI.Despawn.ToString(), 45);
            despawnState.OnStateUpdate += DespawnState_OnStateUpdate;
            despawnState.OnStateEnd += delegate { projectile.Kill(); };

            StandState punchState = AddState(TheWorldAI.Punch, AttackSpeed + 5);

            punchState.OnStateBegin += PunchState_OnStateBegin;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            StandState signAttack = AddState(TheWorldAI.StopSignAttack, 60);
            signAttack.OnStateBegin += SignAttack_OnStateBegin;
            signAttack.OnStateUpdate += UpdatePunch;
            signAttack.OnStateEnd += SignAttack_OnStateEnd;

            StandState flyUpState = AddState(TheWorldAI.FlyUp, 90);
            flyUpState.OnStateUpdate += FlyUpState_OnStateUpdate;
            flyUpState.OnStateBegin += FlyUpState_OnStateBegin;
            flyUpState.OnStateEnd += delegate { SetState(TheWorldAI.SlamDunk.ToString()); };

            StandState slamDunkState = AddState(TheWorldAI.SlamDunk, 90);
            slamDunkState.OnStateEnd += SlamDunkState_OnStateEnd;
            slamDunkState.OnStateBegin += SlamDunkState_OnStateBegin;
            slamDunkState.OnStateUpdate += SlamDunkState_OnStateUpdate;

            StandState knifeThrowState = AddState(TheWorldAI.KnifeThrow, 60);
            knifeThrowState.OnStateBegin += KnifeThrowState_OnStateBegin;
            knifeThrowState.OnStateEnd += GoIdle;
            knifeThrowState.OnStateEnd += delegate { knifeThrowState.OnStateUpdate += ThrowingKnives; };
            knifeThrowState.OnStateUpdate += ThrowingKnives;

            StandState barrageState = AddState(TheWorldAI.Barrage.ToString(), 180);
            barrageState.OnStateBegin += BarrageState_OnStateBegin;

            StandState signGrab = AddState(TheWorldAI.GrabSign, 90);
            signGrab.OnStateEnd += SignGrab_OnStateEnd;
            signGrab.OnStateUpdate += SignGrab_OnStateUpdate;

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
                SetState(TheWorldAI.Idle.ToString());
            };

            SetState(TheWorldAI.Summon.ToString());
        }

        private void PunchState_OnStateBegin(StandState sender)
        {
            AttackSpeed = 15;
            BeginPunch(sender);
        }

        private void SignAttack_OnStateEnd(StandState sender)
        {
            EndPunch(sender);
            stopSignHitCount--;

            if (stopSignHitCount <= 0)
            {
                projectile.width = 60;
                projectile.height = 60;
            }

            if(stopSignHitCount > 0)
            CurrentAnimation = "Stop_Idle" + (3 - stopSignHitCount).ToString();
        }

        private void SignAttack_OnStateBegin(StandState sender)
        {
            AttackSpeed = 60;
            BeginPunch(sender);
            CurrentAnimation = "Stop_Attack" + (3 - stopSignHitCount).ToString();
        }

        private void DespawnState_OnStateUpdate(StandState sender)
        {
            if(Animations[CurrentAnimation].CurrentFrame == 17)
            TBAR.Instance.PlayVoiceLine("Sounds/TheWorld/Teleport");
        }

        private void SignGrab_OnStateUpdate(StandState sender)
        {
            Owner.heldProj = projectile.whoAmI;
        }

        private void SignGrab_OnStateEnd(StandState sender)
        {
            stopSignHitCount = 3;
            projectile.width = 160;
            projectile.height = 160;
            GoIdle(sender);
            CurrentAnimation = "Stop_Idle0";
        }

        private void SummonState_OnStateBegin(StandState sender)
        {
            TBAR.Instance.PlayVoiceLine("Sounds/TheWorld/Teleport");
            TBAR.Instance.PlayVoiceLine("Sounds/TheWorld/Call");

            projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);
        }

        private void KnifeThrowState_OnStateBegin(StandState sender)
        {
            projectile.damage = 0;
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
                case ImmediateInput.LeftClick:
                    if (CanPunch)
                    {
                        if (stopSignHitCount > 0)
                            SetState(TheWorldAI.StopSignAttack, "Stop_Attack" + (3 - stopSignHitCount).ToString());
                        else
                            SetState(PunchState, GetPunchVariation());
                    }
                    break;
                case ImmediateInput.RightClick:
                        SetState(TheWorldAI.KnifeThrow.ToString());
                        break;
                case ImmediateInput.Action3:
                    if (CanPunch && stopSignHitCount <= 0)
                        SetState(TheWorldAI.GrabSign);
                    break;
                default:
                    return;
            }
        }

        private void ThrowingKnives(StandState sender)
        {
            if (Animations[CurrentAnimation].CurrentFrame == 10)
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
            SetState(TheWorldAI.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);
        }

        public override void PostAI()
        {
            if (CanPunch && Owner.controlUseItem && Owner.whoAmI == Main.myPlayer)
            {
                if (stopSignHitCount > 0)
                    SetState(TheWorldAI.StopSignAttack, "Stop_Attack" + (3 - stopSignHitCount).ToString());
                else
                    SetState(PunchState, GetPunchVariation());
            }

            base.PostAI();

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == TheWorldAI.Idle.ToString())
            {
                SetState(TheWorldAI.Despawn.ToString());
            }
        }
        private void Idle(StandState sender)
        {
            projectile.GetGlobal().HitRoadRollerInLifeTime = false;
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.18f);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get().IsStandUser)
                return;

            DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public int KnifeDamage => (int)(8 + BaseDPS * 0.25f);

        public Vector2 SlamDunkPosition { get; set; }

        public Projectile MyRoller { get; set; }

        protected override int GetPunchDamage() => (int)(30 + BaseDPS * 1.2f);

        public override int GetBarrageDamage() => (int)(20 + BaseDPS * 0.8f);

        public override bool CanPunch => State == TheWorldAI.Idle.ToString();
    }

    public enum TheWorldAI
    {
        Summon,
        Despawn,
        Idle,
        Punch,
        KnifeThrow,
        Barrage,
        SlamDunk,
        FlyUp,
        StopSignAttack,
        GrabSign
    }
}
