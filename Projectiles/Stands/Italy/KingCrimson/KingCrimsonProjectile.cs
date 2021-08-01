using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.NPCs;
using TBAR.Players;
using TBAR.Stands;
using TBAR.Stands.GoldenWind.KingCrimson;
using TBAR.TBARBuffs;
using Terraria;
using Terraria.ModLoader;

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

            SpriteAnimation donut = new SpriteAnimation(path + "KCDonutCommit", 6, 10);
            SpriteAnimation donutUndo = new SpriteAnimation(path + "KCDonutUndo", 12, 12);
            SpriteAnimation donutMiss = new SpriteAnimation(path + "KCDonutMiss", 7, 12);

            StandState spawnState = new StandState(KCStates.Spawn.ToString(), spawn);
            spawnState.OnStateUpdate += SpawnState_OnStateUpdate;
            spawnState.OnStateEnd += GoIdle;
            spawnState.Duration = 20;

            StandState idleState = new StandState(KCStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = new StandState(KCStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };
            despawnState.Duration = 20;
            StandState punchState = new StandState
                (punchMidLeft, punchMidRight, punchDownLeft, punchDownRight, punchUpRight, punchUpLeft)
            { Key = KCStates.Punch.ToString() };

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;
            punchState.Duration = 20;

            StandState cutState = new StandState(KCStates.Slice.ToString(), cut);
            cutState.OnStateBegin += CutState_OnStateBegin;
            cutState.OnStateUpdate += UpdatePunch;
            cutState.OnStateEnd += EndPunch;
            cutState.Duration = 60;

            StandState donutState = new StandState(donut) { Key = KCStates.Donut.ToString()};
            donutState.OnStateUpdate += DonutState_OnStateUpdate;
            donutState.OnStateEnd += DonutState_OnStateEnd;
            donutState.OnStateBegin += DonutState_OnStateBegin;
            donutState.Duration = 40;

            StandState donutEndState = new StandState(donutUndo, donutMiss) { Key = KCStates.DonutEnd.ToString() };
            donutEndState.Duration = 60;
            donutEndState.OnStateEnd += GoIdle;
            StandState barrageState = new StandState(path + "KCRush", 4, 15, true)
            {
                Key = KCStates.Barrage.ToString()
            };

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
                SetState(KCStates.Idle.ToString());
            };

            barrageState.Duration = 180;

            AddStates(spawnState, idleState, despawnState, punchState, cutState, donutState, donutEndState, barrageState);
            
            SetState(KCStates.Spawn.ToString());
        }

        private void BarrageState_OnStateBegin(StandState sender)
        {
            PunchDirection = Owner.Center.DirectTo(MousePosition, Owner.width + 16 * Range);
        }

        private void DonutState_OnStateBegin(StandState sender)
        {
            projectile.damage = 0;
            HasMissedDonut = true;
        }

        private void DonutState_OnStateEnd(StandState sender)
        {
            if (sender.CurrentAnimationID == 0)
            {
                if (!HasMissedDonut)
                {
                    States[KCStates.DonutEnd.ToString()].CurrentAnimationID = 0;
                    SetState(KCStates.DonutEnd.ToString());
                    return;
                }
                else if (HasMissedDonut)
                {
                    States[KCStates.DonutEnd.ToString()].CurrentAnimationID = 1;
                    SetState(KCStates.DonutEnd.ToString());
                    return;
                }
            }

            MyDonutPunch = null;

            if (sender.CurrentAnimationID != 0)
            {
                sender.CurrentAnimationID = 0;
                GoIdle(sender);
            }
        }

        private void DonutState_OnStateUpdate(StandState sender)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, MousePosition, 0.25f);
            Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (sender.CurrentAnimationID == 0 && sender.CurrentAnimation.CurrentFrame == 3 && MyDonutPunch == null)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DonutPunch>(), DonutImpactDamage, 0, Owner.whoAmI, projectile.whoAmI, -1);
                MyDonutPunch = Main.projectile[proj].modProjectile as DonutPunch;
                MyDonutPunch.UnpullDamage = DonutUnpullDamage;
            }

            if (MyDonutPunch != null && !MyDonutPunch.projectile.active)
                MyDonutPunch = null;
        }

        private void CutState_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;
            PunchStartPoint = Owner.Center;
            PunchDirection = PunchStartPoint.DirectTo(MousePosition, Owner.width + 16 * Range);
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
            HasMissedDonut = false;
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

        public bool HasMissedDonut { get; set; }

        public int DonutImpactDamage => 25 + (int)(BaseDPS * 1.2);
        public int DonutUnpullDamage => 25 + (int)(BaseDPS * 7.0);

        public DonutPunch MyDonutPunch { get; set; }
    }

    public enum KCStates
    {
        Spawn,
        Idle,
        Despawn,
        Punch,
        Slice,
        Donut,
        DonutEnd,
        Barrage
    }
}
