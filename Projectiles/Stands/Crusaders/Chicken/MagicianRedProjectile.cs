using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Chicken
{
    public class MagicianRedProjectile : PunchGhostProjectile
    {
        private int flameCounter;

        public MagicianRedProjectile() : base("Magician's Red")
        {
        }

        public override void InitializeStates(Projectile projectile)
        {
            Range = 2f;

            AttackSpeed = 25;

            string path = "Projectiles/Stands/Crusaders/Chicken/";

            SpriteAnimation summon = new SpriteAnimation(path + "Spawn", 23, 18);
            SpriteAnimation despawn = new SpriteAnimation(path + "Spawn", 23, 18) { IsReversed = true };
            SpriteAnimation idle = new SpriteAnimation(path + "Idle", 7, 5, true);

            SpriteAnimation punchMidLeft = new SpriteAnimation(path + "FlameThrow1", 6, 12);
            SpriteAnimation punchMidRight = new SpriteAnimation(path + "FlameThrow2", 8, 12);

            SpriteAnimation punchUpLeft = new SpriteAnimation(path + "FlameUpThrow1", 4, 12);
            SpriteAnimation punchUpRight = new SpriteAnimation(path + "FlameUpThrow2", 4, 12);

            SpriteAnimation punchDownLeft = new SpriteAnimation(path + "DownThrow1", 4, 12);
            SpriteAnimation punchDownRight = new SpriteAnimation(path + "FlameDownThrow2", 4, 12);

            SpriteAnimation falconPunch = new SpriteAnimation(path + "ChargePUNCHFULL", 27, 18);

            StandState summonState = new StandState(MRStates.Spawn.ToString(), summon);
            summonState.OnStateBegin += SummonState_OnStateBegin;
            summonState.OnStateEnd += GoIdle;
            summonState.OnStateUpdate += SummonState_OnStateUpdate;
            summonState.Duration = 70;

            StandState idleState = new StandState(MRStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = new StandState(MRStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };
            despawnState.Duration = 80;

            StandState punchState = new StandState
                (punchMidLeft, punchMidRight, punchDownLeft, punchDownRight, punchUpRight, punchUpLeft)
            { Key = MRStates.Punch.ToString() };

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;
            punchState.OnStateEnd += PunchState_OnStateEnd;
            punchState.Duration = AttackSpeed;

            StandState falconState = new StandState(MRStates.FalconPunch.ToString(), falconPunch);
            falconState.OnStateBegin += FalconState_OnStateBegin;
            falconState.OnStateUpdate += UpdatePunch;
            falconState.OnStateUpdate += FalconState_OnStateUpdate;
            falconState.OnStateEnd += EndPunch;
            falconState.Duration = 100;

            AddStates(summonState, idleState, despawnState, punchState, falconState);

            SetState(MRStates.Spawn.ToString());
        }

        private void FalconState_OnStateUpdate(StandState sender)
        {
            if (sender.Duration == 30)
                Main.PlaySound(SoundID.Item74);

            if (sender.Duration <= 30 && sender.Duration > 15)
                projectile.damage = FalconDamage;
            else if (sender.Duration <= 15)
                projectile.damage = 0;
        }

        private void MagicianRedProjectile_OnHit(PunchGhostProjectile attacker, Entity victim)
        {
            Vector2 dir = new Vector2(Owner.direction * 7.8f, 0);
            for (int i = 0; i < 5; i++)
            {
                Vector2 off = new Vector2(Owner.direction * 8, 0);
                Projectile.NewProjectile(projectile.Center + off, (dir).RotatedByRandom(0.5), ModContent.ProjectileType<FireballFart>(), 0, 0, projectile.owner);
            }
        }

        private void FalconState_OnStateBegin(StandState sender)
        {
            NonTimedAttack = true;

            OnHit += MagicianRedProjectile_OnHit;

            PunchStartPoint = Owner.Center;

            Owner.direction = MousePosition.X < Owner.Center.X ? -1 : 1;

            PunchDirection = PunchStartPoint.DirectTo(MousePosition, Owner.width + 16 * Range);
        }

        private void PunchState_OnStateEnd(StandState sender)
        {
            Main.PlaySound(SoundID.Item45);
            Projectile.NewProjectile(projectile.Center - projectile.Center.ToMouse(24f), Owner.Center.ToMouse(12f), ModContent.ProjectileType<Fireball>(), FireballDamage, 0f, projectile.owner);
        }

        private void SummonState_OnStateBegin(StandState sender)
        {
            projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);
        }

        public override void PostAI()
        {
            base.PostAI();

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && State == MRStates.Idle.ToString())
            {
                SetState(MRStates.Despawn.ToString());
            }

            Vector2 offset = new Vector2(-4, projectile.height * 0.55f);

            Vector2[] vels = new[] { new Vector2(3, -2), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-3, -2) };

            if (++flameCounter > 4)
            {
                flameCounter = 0;
                for (int i = 0; i < 4; i++)
                {
                    int dust = Dust.NewDust(projectile.Center + offset, 0, 0, DustID.Fire, 0, 0, 0, default, 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = vels[i].RotatedByRandom(0.2f);
                }
            }
        }

        private void Idle(StandState sender)
        {
            ClearOnHitEffects();
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
        }

        private void GoIdle(StandState sender)
        {
            SetState(MRStates.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            if (sender.CurrentAnimation.CurrentFrame == 0)
                projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);

            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.12f);
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

        private int FalconDamage => 100 + (int)(BaseDPS * 10f);

        private int FireballDamage => 12 + (int)(BaseDPS * 0.75f);

        protected override string PunchState => MRStates.Punch.ToString();

        protected override int GetPunchDamage() => 12 + (int)(BaseDPS * 1.4f);

        public override bool CanPunch => IsIdle;
    }

    public enum MRStates
    {
        Spawn,
        Idle,
        Despawn,
        Punch,
        FalconPunch
    }
}
