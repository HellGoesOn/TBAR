﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
using TBAR.ScreenModifiers;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Italy.Aerosmith
{
    public class AerosmithProjectile : StandProjectile
    {
        public AerosmithProjectile() : base("Aerosmith")
        {
        }

        public override void InitializeStates(Projectile projectile)
        {
            string path = "Projectiles/Stands/Italy/Aerosmith/";

            BaseDPS = -1;

            Speed = 4f;

            Opacity = 0f;

            IsEngineOn = true;

            // Wait, its all "Idle"?
            SpriteAnimation spawn = new SpriteAnimation(path + "Idle", 18, 12);
            SpriteAnimation idle = new SpriteAnimation(path + "Idle", 18, 12, true);
            SpriteAnimation returnAnimation = new SpriteAnimation(path + "Idle", 18, 12, true);
            SpriteAnimation despawn = new SpriteAnimation(path + "Idle", 18, 12);
            SpriteAnimation barrage = new SpriteAnimation(path + "Idle", 18, 12, true, 12);
            // Always has been *cocks gun*

            StandState spawnState = new StandState(ASStates.Spawn.ToString(), spawn);
            spawnState.OnStateBegin += SpawnState_OnStateBegin;
            spawnState.OnStateUpdate += SpawnState_OnStateUpdate;
            spawnState.OnStateEnd += SpawnState_OnStateEnd;

            StandState idleState = new StandState(ASStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += IdleState_OnStateUpdate;

            StandState despawnState = new StandState(ASStates.Despawn.ToString(), despawn);
            despawnState.OnStateUpdate += DespawnState_OnStateUpdate;
            despawnState.OnStateEnd += DespawnState_OnStateEnd;
            despawnState.OnStateBegin += DespawnState_OnStateBegin;

            StandState barrageState = new StandState(ASStates.Barrage.ToString(), barrage);
            barrageState.OnStateUpdate += IdleState_OnStateUpdate;
            barrageState.OnStateUpdate += BarrageState_OnStateUpdate;
            barrageState.OnStateEnd += BarrageState_OnStateEnd;

            StandState returnState = new StandState(ASStates.Return.ToString(), returnAnimation);
            returnState.OnStateUpdate += ReturnState_OnStateUpdate;
            returnState.OnStateBegin += DespawnState_OnStateBegin;

            AddStates(spawnState, idleState, despawnState, barrageState, returnState);

            SetState(ASStates.Spawn.ToString());
        }

        private void BarrageState_OnStateEnd(StandState sender)
        {
            SetState(ASStates.Idle.ToString());
        }

        public override void HandleImmediateInputs(ImmediateInput input)
        {
            switch (input)
            {
                case ImmediateInput.LeftClick:
                    if (State == ASStates.Idle.ToString())
                        SetState(ASStates.Barrage.ToString());
                    break;

                case ImmediateInput.Action1:
                    IsEngineOn = !IsEngineOn;
                    break;
            }
        }

        private void BarrageState_OnStateUpdate(StandState sender)
        {
            if (CurrentState.CurrentAnimation.LoopTimer % 4 == 0)
            {
                Vector2 position = projectile.Center;
                Vector2 velocity = new Vector2(16, 0).RotatedBy(Angle);
                int type = ModContent.ProjectileType<AerosmithBullet>();
                int damage = BulletDamage;

                float offX = -24;
                float offY = 4;

                int multiplier = (projectile.velocity.X > 0 ? 1 : -1);

                Vector2 offset1 = new Vector2(offX, offY * multiplier).RotatedBy(Angle);
                Vector2 offset2 = new Vector2(offX, (offY + 2) * multiplier).RotatedBy(Angle);

                Main.PlaySound(SoundID.Item31, projectile.position);

                Projectile.NewProjectile(position + offset1, velocity.RotatedByRandom(.035f), type, damage, 0, Owner.whoAmI);

                Projectile.NewProjectile(position + offset2, velocity.RotatedByRandom(.035f), type, damage, 0, Owner.whoAmI);
            }

        }

        private void SpawnState_OnStateBegin(StandState sender)
        {
            if (Owner.direction == -1)
                Angle = (float)MathHelper.Pi;
        }

        private void DespawnState_OnStateBegin(StandState sender)
        {
            TBARPlayer.Get(Owner).ScreenModifiers.RemoveAll(x => x is AerosmithScreenModifier || x is PlayerChaseScreenModifier);
            TBARPlayer.Get(Owner).ScreenModifiers.Add(new PlayerChaseScreenModifier(projectile.Center, Owner.Center, 0.2f));
        }

        private void ReturnState_OnStateUpdate(StandState sender)
        {
            if (Vector2.Distance(Owner.Center, projectile.Center) <= 16 * 10)
                SetState(ASStates.Despawn.ToString());

            Angle = (Owner.Center - projectile.Center).SafeNormalize(-Vector2.UnitY).ToRotation();

            IsEngineOn = true;

            projectile.velocity = new Vector2(Speed, 0).RotatedBy(Angle);
        }

        private void DespawnState_OnStateUpdate(StandState sender)
        {
            Opacity -= 0.02f;

            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) + (projectile.velocity * 4.5f), 0, 0, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity = -(projectile.velocity * 0.5f).RotatedByRandom(.45f);
            }
        }

        private void SpawnState_OnStateUpdate(StandState sender)
        {
            FlightVector = new Vector2(1, 0).RotatedBy(Angle);

            projectile.velocity = FlightVector * Speed + new Vector2(0, YSpeed);

            if (Opacity < 1f)
                Opacity += 0.04f;

            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) + (projectile.velocity * 4.5f), 0, 0, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity = -(projectile.velocity * 0.5f).RotatedByRandom(.45f);
            }
        }

        private void DespawnState_OnStateEnd(StandState sender)
        {
            projectile.Kill();
        }

        private void SpawnState_OnStateEnd(StandState sender)
        {
            TBARPlayer.Get(Owner).ScreenModifiers.Add(new AerosmithScreenModifier(Owner.Center));
            SetState(ASStates.Idle.ToString());
        }

        private void IdleState_OnStateUpdate(StandState sender)
        {
            if (Vector2.Distance(projectile.Center, Owner.Center) >= Global.TILE_SIZE * 200)
            {
                SetState(ASStates.Return.ToString());
            }

            float angle = (MousePosition - projectile.Center).SafeNormalize(-Vector2.UnitY).ToRotation();

            Angle = Utils.AngleLerp(Angle, angle, 0.07f);

            //float omegaAngle = Utils.AngleLerp(Angle, angle, 0.5f);

            if (IsEngineOn)
            {
                FlightVector = new Vector2(1, 0).RotatedBy(Angle);

                if (Speed < 8.0f)
                    Speed += 0.2f;

                if (YSpeed > 0)
                    YSpeed -= 0.12f;
            }
            else
            {
                if (Speed > 0)
                    Speed -= 0.025f;

                if (YSpeed < 12f)
                    YSpeed += 0.06f;
            }

            projectile.velocity = FlightVector * Speed + new Vector2(0, YSpeed);
            /*
            if (!IsReturning && !IsDespawning)
                Velocity = Collision.TileCollision(Center - new Vector2(10), Velocity, 20, 20, false, false, 1);
            */

            if (projectile.velocity.Y == 0 && !IsEngineOn)
            {
                if (Speed > 0)
                    Speed = MathHelper.Clamp(Speed - 0.15f, 0, 8f);
            }

            SpriteFX = projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public override void PostAI()
        {
            base.PostAI();

            if (BaseDPS == -1)
            {
                if (Owner.HeldItem.GetDamageType() == DamageType.Ranged)
                    BaseDPS = Owner.HeldItem.GetDamageData(Owner).DPS;
                else
                    BaseDPS = 5;
            }

            if (TBARInputs.SummonStand.JustPressed && State == ASStates.Idle.ToString() && Main.myPlayer == projectile.owner)
            {
                if (Vector2.Distance(Owner.Center, projectile.Center) <= Global.TILE_SIZE * 10)
                    SetState(ASStates.Despawn.ToString());
                else
                    SetState(ASStates.Return.ToString());
            }
        }

        public int BulletDamage => 8 + (int)(BaseDPS * 1.2);

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteAnimation animation = CurrentState.AssignedAnimations[CurrentState.CurrentAnimationID];

            float angleDerp = SpriteFX == SpriteEffects.FlipHorizontally ? 0f : MathHelper.Pi;
            spriteBatch.Draw(animation.SpriteSheet, projectile.Center - Main.screenPosition, animation.FrameRect, Color.White * Opacity, Angle + angleDerp, animation.DrawOrigin, Scale, SpriteFX, 1f);
        }

        public Vector2 FlightVector { get; set; }

        public float Angle { get; set; }

        public int BaseDPS { get; set; }

        public float Speed { get; set; }

        public float YSpeed { get; set; }

        public bool IsEngineOn { get; set; }
    }

    public enum ASStates
    {
        Spawn,
        Idle,
        Despawn,
        Barrage,
        Return
    }
}