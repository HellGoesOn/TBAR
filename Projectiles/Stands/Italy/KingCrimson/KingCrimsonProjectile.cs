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
            AttackSpeed = 18;

            string path = "Projectiles/Stands/Italy/KingCrimson/";

            AddAnimation(KCStates.Spawn.ToString(), path + "KCSpawn", 7, 20);
            AddAnimation(KCStates.Despawn.ToString(), path + "KCSpawn", 7, 20).AsReversed();
            AddAnimation("PunchMid1", path + "KCPunchRight", 4, 10);
            AddAnimation("PunchMid2", path + "KCPunchLeft", 4, 10);
            AddAnimation("PunchUp1", path + "KCPunchRightU", 4, 10);
            AddAnimation("PunchUp2", path + "KCPunchLeftU", 4, 10);
            AddAnimation("PunchDown1", path + "KCPunchRightD", 4, 10);
            AddAnimation("PunchDown2", path + "KCPunchLeftD", 4, 10);
            AddAnimation(KCStates.Idle.ToString(), path + "KCIdle", 5, 5, true);
            AddAnimation(KCStates.Slice.ToString(), path + "KCYeet", 13, 12);
            AddAnimation(KCStates.Donut, path + "KCDonutCommit", 6, 10);
            AddAnimation("DonutEnd", path + "KCDonutUndo", 12, 12);
            AddAnimation("DonutMiss", path + "KCDonutMiss", 7, 12);
            AddAnimation(KCStates.Barrage, path + "KCRush", 4, 15, true);

            StandState spawnState = AddState(KCStates.Spawn, 20);
            spawnState.OnStateUpdate += SpawnState_OnStateUpdate;
            spawnState.OnStateEnd += GoIdle;

            StandState idleState = AddState(KCStates.Idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = AddState(KCStates.Despawn.ToString(), 14);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };

            StandState punchState = AddState(KCStates.Punch, AttackSpeed);
            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;

            StandState cutState = AddState(KCStates.Slice.ToString(), 60);
            cutState.OnStateBegin += CutState_OnStateBegin;
            cutState.OnStateUpdate += UpdatePunch;
            cutState.OnStateEnd += EndPunch;

            StandState donutState = AddState(KCStates.Donut, 40);
            donutState.OnStateUpdate += DonutState_OnStateUpdate;
            donutState.OnStateEnd += DonutState_OnStateEnd;
            donutState.OnStateBegin += DonutState_OnStateBegin;

            StandState donutEndState = AddState(KCStates.DonutEnd, 60);
            donutEndState.OnStateEnd += GoIdle;

            StandState barrageState = AddState(KCStates.Barrage, 180);

            barrageState.OnStateBegin += BarrageState_OnStateBegin;

            barrageState.OnStateUpdate += delegate
            {
                if (Barrage != null)
                    SpriteFX = Barrage.Center.X < projectile.Center.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            };

            barrageState.OnStateEnd += delegate
            {
                Barrage.Kill();
                PunchDirection = Vector2.Zero;
                Barrage = null;
                SetState(KCStates.Idle.ToString());
            };
            
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
            MyDonutPunch = null;
            SetState(KCStates.DonutEnd, "Donut" + (HasMissedDonut ? "Miss" : "End"));
        }

        private void DonutState_OnStateUpdate(StandState sender)
        {
            projectile.Center = Vector2.SmoothStep(projectile.Center, MousePosition, 0.22f);
            Owner.direction = projectile.Center.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // bad code; state shouldnt care about which animation is playing
            if (Animations[CurrentAnimation].CurrentFrame == 3 && MyDonutPunch == null)
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
            OnHit += KingCrimsonProjectile_OnHit;
            NonTimedAttack = true;
            PunchStartPoint = Owner.Center;
            PunchDirection = PunchStartPoint.DirectTo(MousePosition, Owner.width + 16 * Range);
            projectile.damage = CutDamage;
            Owner.direction = MousePosition.X < Owner.Center.X ? -1 : 1;
        }

        private void KingCrimsonProjectile_OnHit(PunchGhostProjectile attacker, Entity victim)
        {
            TBARPlayer.Get(Owner).AddStylePoints(1000);
        }

        private void SpawnState_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
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
            ClearOnHitEffects();
            HasMissedDonut = false;
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
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
