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

            AttackSpeed = 12;

            string path = "Projectiles/Stands/Crusaders/StarPlatinum/";
            AddAnimation(SPStates.Summon.ToString(), path + "SPSummon", 10, 15);
            AddAnimation(SPStates.Despawn.ToString(), path + "SPDespawn", 6, 12);
            AddAnimation(SPStates.Idle.ToString(), path + "SPIdle", 14, 15, true);
            AddAnimation(SPStates.Barrage.ToString(), path + "SPRush_Middle", 4, 15, true);
            AddAnimation(SPStates.Uppercut.ToString(), path + "SPDonutPunch", 15, 12);
            AddAnimation("PunchUp1", path + "SPPunch_Up_LeftHand", 3, 10);
            AddAnimation("PunchUp2", path + "SPPunch_Up_RightHand", 3, 10);
            AddAnimation("PunchMid1", path + "SPPunch_Middle_LeftHand", 3, 10);
            AddAnimation("PunchMid2", path + "SPPunch_Middle_RightHand", 3, 10);
            AddAnimation("PunchDown1", path + "SPPunch_Down_LeftHand", 3, 10);
            AddAnimation("PunchDown2", path + "SPPunch_Down_RightHand", 3, 10);

            StandState summon = AddState(SPStates.Summon.ToString(), 40);

            summon.OnStateBegin += OnSummon; 
            summon.OnStateUpdate += Summon;
            summon.OnStateEnd += delegate { SetState(SPStates.Idle.ToString()); };

            StandState despawn = AddState(SPStates.Despawn.ToString(), 40);
            despawn.OnStateUpdate += Despawn;
            despawn.OnStateEnd += OnDespawnEnd;

            StandState idle = AddState(SPStates.Idle.ToString());
            idle.OnStateUpdate += Idle;

            StandState upperCutState = AddState(SPStates.Uppercut.ToString(), 80);
            upperCutState.OnStateBegin += UpperCutState_OnStateBegin;
            upperCutState.OnStateUpdate += UpperCutState_OnStateUpdate;
            upperCutState.OnStateEnd += UpperCutState_OnStateEnd;

            StandState punchState = AddState(SPStates.Punch.ToString(), AttackSpeed + 3);

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            StandState barrageState = AddState(SPStates.Barrage.ToString(), 180);
            barrageState.OnStateBegin += BarrageState_OnStateBegin;

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
            OnHit += StarPlatinumProjectile_OnHit;
            NonTimedAttack = true;
            projectile.damage = GetUppercutDamage();
        }

        private void StarPlatinumProjectile_OnHit(PunchGhostProjectile attacker, Entity victim)
        {
            TBARPlayer.Get(Owner).AddStylePoints(1000);
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

            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
        }

        private void Despawn(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Opacity -= 0.05f;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-12 * Owner.direction, -12), 0.15f);
        }

        private void OnDespawnEnd(StandState sender)
        {
            projectile.Kill();
        }

        private void Idle(StandState sender)
        {
            ClearOnHitEffects();
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
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
