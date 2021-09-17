using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Donator.SoulOfCinder
{
    public class SoulOfCinderProjectile : PunchGhostProjectile
    {
        public SoulOfCinderProjectile() : base("Soul of Cinder")
        {
        }

        public int swingCounter = 0;

        private readonly int[] swingBaseDamage = new int[] { 90, 95 , 100};

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

            SpriteAnimation barrage = new SpriteAnimation(path + "Barrage", 11, 15, true);

            SpriteAnimation grabPrep = new SpriteAnimation(path + "GrabPrep", 16, 15);
            SpriteAnimation grab = new SpriteAnimation(path + "Grab", 13, 14);

            SpriteAnimation ability2 = new SpriteAnimation(path + "Ability2", 13, 12);

            SpriteAnimation ability3 = new SpriteAnimation(path + "Ability3", 12, 12);

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

            swingState.OnStateBegin += SwingState_OnStateBegin;
            swingState.OnStateUpdate += UpdateSwing;
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

            StandState barrageState = new StandState(SOCStates.Barrage.ToString(), barrage);
            barrageState.OnStateBegin += BarrageState_OnStateBegin;
            barrageState.OnStateUpdate += UpdateSwing;
            barrageState.OnStateEnd += BarrageState_OnStateEnd;
            barrageState.Duration = 240;

            StandState grabPrepState = new StandState(SOCStates.GrabPrep.ToString(), grabPrep);
            grabPrepState.OnStateEnd += delegate { SetState(SOCStates.Grab.ToString()); };
            grabPrepState.Duration = 60;

            StandState grabState = new StandState(SOCStates.Grab.ToString(), grab);
            grabState.OnStateBegin += GrabState_OnStateBegin;
            grabState.OnStateUpdate += GrabState_OnStateUpdate;
            grabState.OnStateEnd += GrabState_OnStateEnd;
            grabState.Duration = 68;

            StandState ability2State = new StandState(SOCStates.Ability2.ToString(), ability2);
            ability2State.OnStateEnd += Ability2State_OnStateEnd;
            ability2State.OnStateBegin += Ability2State_OnStateBegin;
            ability2State.OnStateUpdate += Ability2State_OnStateUpdate;
            ability2State.Duration = 70;

            StandState ability3State = new StandState(SOCStates.Ability3.ToString(), ability3);
            ability3State.OnStateUpdate += Ability3State_OnStateUpdate;
            ability3State.OnStateEnd += GoIdle;
            ability3State.Duration = 120;

            AddStates(spawnState, idleState, swingState, despawnState, ability1State, soulStreamState, barrageState, grabPrepState, grabState, ability2State, ability3State);

            SetState(SOCStates.Spawn.ToString());
        }

        private void Ability3State_OnStateUpdate(StandState sender)
        {
            if (sender.Duration >= 60)
            {
                projectile.Center = Vector2.SmoothStep(projectile.Center, MousePosition - new Vector2(20 * Owner.direction, 0), 0.22f);
                Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
                SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            if (sender.Duration == 60)
                Projectile.NewProjectile(projectile.Center + new Vector2(-20 * Owner.direction, 40), Vector2.Zero, ModContent.ProjectileType<SOCExplosion>(), 1000, 0, projectile.whoAmI);
        }

        private void Ability2State_OnStateUpdate(StandState sender)
        {
            projectile.Center = Vector2.SmoothStep(projectile.Center, MousePosition - new Vector2(20 * Owner.direction, 0), 0.22f);
            Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (sender.Duration == 24)
                for (int i = 0; i < 25; i++)
                {
                    int dust = Dust.NewDust(projectile.Center + new Vector2(10 * Owner.direction, 0), 20, 20, DustID.Fire, 0, 0, 0, default, 1.5f);
                    Main.dust[dust].velocity = new Vector2(Main.rand.Next(2, 6) * Owner.direction, -Main.rand.Next(4));
                }
        }

        private void Ability2State_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;
            projectile.damage = 75;
            projectile.direction = MousePosition.X < Owner.Center.X ? -1 : 1;
            projectile.knockBack = 15f;
        }

        private void Ability2State_OnStateEnd(StandState sender)
        {
            projectile.damage = 0;
            projectile.knockBack = 0f;
            GoIdle(sender);
        }

        private void GrabState_OnStateEnd(StandState sender)
        {
            projectile.damage = 0;
            GoIdle(sender);
        }

        private void GrabState_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;
            projectile.damage = 350;
        }

        private void GrabState_OnStateUpdate(StandState sender)
        {
            projectile.Center = Vector2.SmoothStep(projectile.Center, MousePosition - new Vector2(20 * Owner.direction, 0), 0.22f);
            Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (sender.Duration == 30)
                Main.PlaySound(SoundID.Item74, projectile.position);
        }

        private void SwingState_OnStateBegin(StandState sender)
        {
            if (swingCounter >= 3)
                swingCounter = 0;

            BeginPunch(sender);
        }

        private void BarrageState_OnStateEnd(StandState sender)
        {
            GoIdle(sender);
            AttackSpeed = 30;
            projectile.damage = 0;
        }

        private void BarrageState_OnStateBegin(StandState sender)
        {
            AttackSpeed = 6;
            projectile.damage = 150;
        }

        private void SoulStreamState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(30 * Owner.direction, 10), 0.15f);

            if (sender.Duration == 180)
                Projectile.NewProjectile(projectile.Center, Owner.Center.ToMouse(16), ModContent.ProjectileType<SoulBeam>(), 275, 5.75f, Owner.whoAmI, projectile.whoAmI);

            if (TBARInputs.ComboButton2.Current && Main.myPlayer == Owner.whoAmI && sender.Duration < 74)
                sender.timeLeft = 74;
        }

        private void Ability1State_OnStateBegin(StandState sender)
        {
            projectile.damage = 110;
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
            Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void SwingState_OnStateEnd(StandState sender)
        {
            PunchStartPoint = PunchDirection = Vector2.Zero;
            projectile.damage = 0;

            swingCounter++;

            GoIdle(sender);
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
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SetState(SOCStates.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            projectile.ToggleModifierDependency(false);
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

            if (State == SOCStates.Grab.ToString() && CurrentState.CurrentAnimation.CurrentFrame < 8)
                return false;

            if (State == SOCStates.Ability2.ToString() && CurrentState.CurrentAnimation.CurrentFrame < 8)
                return false;

            return base.CanHitNPC(target);
        }

        public override bool CanPunch => State == SOCStates.Idle.ToString();

        protected override int GetPunchDamage()
        {
            return swingBaseDamage[swingCounter];
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);

            damage = projectile.damage;
        }

        public override void HandleImmediateInputs(ImmediateInput input)
        {
            switch (input)
            {
                case ImmediateInput.LeftClick:
                    if (CanPunch)
                        SetState(PunchState);
                    break;
                case ImmediateInput.RightClick:
                    if (CanPunch)
                        SetState(SOCStates.Ability2.ToString());
                    break;
                default:
                    return;
            }
        }
    }
    public enum SOCStates
    {
        Spawn,
        Idle,
        Despawn,
        Swing,
        Ability1,
        SoulStream,
        Barrage,
        GrabPrep,
        Grab,
        Ability2,
        Ability3
    }
}
