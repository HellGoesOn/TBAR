using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Input;
using TBAR.NPCs;
using TBAR.Players;
using TBAR.Stands;
using TBAR.TBARBuffs;
using Terraria;

namespace TBAR.Projectiles.Stands.Italy.KingCrimson
{
    public class KingCrimsonProjectile : PunchGhostProjectile
    {
        private float offset;
        private bool reverseOffsetGain;
        public KingCrimsonProjectile() : base("King Crimson")
        {
        }

        protected override string PunchState => KCStates.Punch.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            AttackSpeed = 30;

            string path = "Projectiles/Stands/Italy/KingCrimson/";
            SpriteAnimation spawn = new SpriteAnimation(path + "KCSpawn", 7, 20);
            SpriteAnimation despawn = new SpriteAnimation(path + "KCSpawn", 7, 20)
            {
                IsReversed = true
            };
            despawn.Reset();

            SpriteAnimation punchMidLeft = new SpriteAnimation(path + "KCPunchRight", 4, 10);
            SpriteAnimation punchMidRight = new SpriteAnimation(path + "KCPunchLeft", 4, 10);

            SpriteAnimation punchUpLeft = new SpriteAnimation(path + "KCPunchRightU", 4, 10);
            SpriteAnimation punchUpRight = new SpriteAnimation(path + "KCPunchLeftU", 4, 10);

            SpriteAnimation punchDownLeft = new SpriteAnimation(path + "KCPunchRightD", 4, 10);
            SpriteAnimation punchDownRight = new SpriteAnimation(path + "KCPunchLeftD", 4, 10);

            SpriteAnimation idle = new SpriteAnimation(path + "KCIdle", 5, 5, true);

            SpriteAnimation cut = new SpriteAnimation(path + "KCYeet", 13, 12);

            StandState spawnState = new StandState(KCStates.Spawn.ToString(), spawn);
            spawnState.OnStateUpdate += SpawnState_OnStateUpdate;
            spawnState.OnStateEnd += GoIdle;

            StandState idleState = new StandState(KCStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = new StandState(KCStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };

            StandState punchState = new StandState
                (punchMidLeft, punchMidRight, punchDownLeft, punchDownRight, punchUpRight, punchUpLeft)
            { Key = KCStates.Punch.ToString() };

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            StandState cutState = new StandState(KCStates.Slice.ToString(), cut);
            cutState.OnStateBegin += CutState_OnStateBegin;
            cutState.OnStateUpdate += UpdatePunch;
            cutState.OnStateEnd += EndPunch;

            AddStates(spawnState, idleState, despawnState, punchState, cutState);
            
            SetState(KCStates.Spawn.ToString());
        }

        private void CutState_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;
            projectile.damage = CutDamage;
            Owner.direction = MousePosition.X < Owner.Center.X ? -1 : 1;
        }

        private void SpawnState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        public override void PostAI()
        {
            base.PostAI();

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && IsIdle)
            {
                SetState(KCStates.Despawn.ToString());
            }
        }

        private void Idle(StandState sender)
        {
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        public void GoIdle(StandState sender)
        {
            SetState(KCStates.Idle.ToString());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get().IsStandUser)
                return;

            for (int i = 0; i < 4; i++)
            {
                Vector2 voffset = new Vector2(offset, 0).RotatedBy(MathHelper.PiOver2 * i);
                DrawDefault(spriteBatch, projectile.Center + voffset, AuraColor * 0.5f, SpriteFX);
            }

            DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public override void AI()
        {
            base.AI();

            if (Math.Abs(offset) > 4)
                reverseOffsetGain = !reverseOffsetGain;

            offset += reverseOffsetGain ? -0.1f : 0.1f;
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
            return (int)(12 + BaseDPS * 1.2f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);

            if (State == KCStates.Slice.ToString())
                TBARBuff.AddToNpcIf<Laceration>(target, 6 * Global.TPS, x => x.Owner == projectile, projectile, BuffAddStyle.Replace);
            else
            {
                Laceration buff = (Laceration)TBARBuff.ExtendForNpcIf<Laceration>(target, 2 * Global.TPS, x => x.Owner == projectile);

                if (buff != null)
                    buff.Initialize();
            }
        }

        protected int CutDamage => (int)(30 + BaseDPS * 2);

        public int LacerationDamage => (int)(15 + BaseDPS * 0.75);

        public override int GetBarrageDamage() => (int)(12 + BaseDPS * 1.2f);

        public override bool CanPunch => State == KCStates.Idle.ToString();
    }

    public enum KCStates
    {
        Spawn,
        Idle,
        Despawn,
        Punch,
        Slice
    }
}
