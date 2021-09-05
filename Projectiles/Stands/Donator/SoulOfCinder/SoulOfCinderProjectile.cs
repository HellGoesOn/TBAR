using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Donator.SoulOfCinder
{
    public class SoulOfCinderProjectile : PunchGhostProjectile
    {
        public SoulOfCinderProjectile() : base("Soul of Cinder")
        {
        }

        public int swingCounter = 0;

        private readonly int[] swingBaseDamage = new int[] { 180, 190, 200 };

        private readonly Vector2[] swingsOffsets = new Vector2[] { new Vector2(0, -20), new Vector2(0, -20), new Vector2(0, -40) };

        protected override string PunchState => SOCStates.Swing.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 3.5f;

            AttackSpeed = 30;

            string path = "Projectiles/Stands/Donator/SoulOfCinder/";
            SpriteAnimation spawn = new SpriteAnimation(path + "Spawn", 20, 12);
            SpriteAnimation despawn = new SpriteAnimation(path + "Spawn", 20, 12) { IsReversed = true };
            SpriteAnimation idle = new SpriteAnimation(path + "Idle", 10, 15, true);
            SpriteAnimation ability1 = new SpriteAnimation(path + "Ability1", 18, 15);

            SpriteAnimation swingOne = new SpriteAnimation(path + "Attack1", 8, 10);
            SpriteAnimation swingTwo = new SpriteAnimation(path + "Attack2", 7, 10);
            SpriteAnimation swingThree = new SpriteAnimation(path + "Attack3", 10, 14);

            SpriteAnimation soulMagic = new SpriteAnimation(path + "SoulMagic", 9, 10);

            StandState spawnState = new StandState(SOCStates.Spawn.ToString(), spawn);
            spawnState.OnStateBegin += SummonState_OnStateBegin;
            spawnState.OnStateEnd += GoIdle;
            spawnState.OnStateUpdate += SummonState_OnStateUpdate;
            spawnState.Duration = 104;

            StandState despawnState = new StandState(SOCStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };
            despawnState.Duration = 80;

            StandState swingState = new StandState(swingOne, swingTwo, swingThree)
            {
                Key = SOCStates.Swing.ToString()
            };

            swingState.OnStateBegin += BeginPunch;
            swingState.OnStateUpdate += UpdateSwing;
            swingState.OnStateEnd += EndPunch;
            swingState.OnStateEnd += SwingState_OnStateEnd;
            swingState.Duration = 50;

            StandState idleState = new StandState(SOCStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState ability1State = new StandState(SOCStates.Ability1.ToString(), ability1);
            ability1State.OnStateEnd += GoIdle;
            ability1State.OnStateEnd += Ability1State_OnStateEnd;
            ability1State.OnStateUpdate += Ability1State_OnStateUpdate;
            ability1State.OnStateBegin += Ability1State_OnStateBegin;
            ability1State.Duration = 68;

            StandState soulStreamState = new StandState(SOCStates.SoulStream.ToString(), soulMagic);
            soulStreamState.OnStateEnd += GoIdle;
            soulStreamState.OnStateUpdate += SoulStreamState_OnStateUpdate;
            soulStreamState.Duration = 240;

            AddStates(spawnState, idleState, swingState, despawnState, ability1State, soulStreamState);

            SetState(SOCStates.Spawn.ToString());
        }

        private void SoulStreamState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(30 * Owner.direction, 10), 0.15f);

            if (sender.Duration == 180)
                Projectile.NewProjectile(projectile.Center, Owner.Center.ToMouse(16), ModContent.ProjectileType<SoulBeam>(), 150, 5.75f, Owner.whoAmI, projectile.whoAmI);
        }

        private void Ability1State_OnStateBegin(StandState sender)
        {
            projectile.damage = 600;
        }

        private void Ability1State_OnStateEnd(StandState sender)
        {
            projectile.damage = 0;
            Projectile.NewProjectile(projectile.Bottom + new Vector2(40 * Owner.direction, -46), new Vector2(Owner.direction, 0), ModContent.ProjectileType<GroundFire>(), 150, 5.75f, Owner.whoAmI, 15, Owner.direction);
        }

        private void Ability1State_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(64 * Owner.direction, -44), 0.22f);
        }

        public override void PostAI()
        {
            base.PostAI();

            projectile.width = (int)CurrentState.CurrentAnimation.FrameSize.X;
            projectile.height = (int)CurrentState.CurrentAnimation.FrameSize.Y;

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == SOCStates.Idle.ToString())
            {
                SetState(SOCStates.Despawn.ToString());
            }
        }

        private void UpdateSwing(StandState _)
        {
            projectile.Center = Vector2.SmoothStep(projectile.Center, MousePosition + swingsOffsets[swingCounter], 0.22f);
            Owner.direction = (Owner.Center + PunchDirection).X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void SwingState_OnStateEnd(StandState sender)
        {
            if (++swingCounter > 2)
                swingCounter = 0;
        }

        private void SummonState_OnStateBegin(StandState sender)
        {
            projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);
        }

        private void Idle(StandState sender)
        {
            ClearOnHitEffects();
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
        }

        private void GoIdle(StandState sender)
        {
            SetState(SOCStates.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            Owner.heldProj = projectile.whoAmI;

            if (sender.CurrentAnimation.CurrentFrame == 0)
                projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);

            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
        }

        protected override int PunchAnimationIDOffset()
        {
            return swingCounter;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if(State == SOCStates.Swing.ToString())
            {
                if (CurrentState.CurrentAnimationID == 0 && CurrentState.CurrentAnimation.CurrentFrame < 5)
                    return false;
                if (CurrentState.CurrentAnimationID == 1 && CurrentState.CurrentAnimation.CurrentFrame < 5)
                    return false;
                if (CurrentState.CurrentAnimationID == 2 && CurrentState.CurrentAnimation.CurrentFrame < 8)
                    return false;
            }

            if(State == SOCStates.Ability1.ToString())
            {
                if (CurrentState.CurrentAnimation.CurrentFrame < 14)
                    return false;
            }

            return base.CanHitNPC(target);
        }

        public override bool CanPunch => State == SOCStates.Idle.ToString();

        protected override int GetPunchDamage()
        {
            return swingBaseDamage[swingCounter];
        }
    }
    public enum SOCStates
    {
        Spawn,
        Idle,
        Despawn,
        Swing,
        Ability1,
        SoulStream
    }
}
