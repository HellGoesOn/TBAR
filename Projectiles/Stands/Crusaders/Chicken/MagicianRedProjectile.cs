using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Players;
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

            AddAnimation(MRStates.Spawn,path + "Spawn", 23, 18);

            AddAnimation(MRStates.Despawn, path + "Spawn", 23, 18).AsReversed();

            AddAnimation(MRStates.Idle, path + "Idle", 7, 5, true);

            AddAnimation("PunchMid1", path + "FlameThrow1", 6, 12);
            AddAnimation("PunchMid2", path + "FlameThrow2", 8, 12);

            AddAnimation("PunchUp1", path + "FlameUpThrow1", 4, 12);
            AddAnimation("PunchUp2", path + "FlameUpThrow2", 4, 12);

            AddAnimation("PunchDown1",path + "DownThrow1", 4, 12);
            AddAnimation("PunchDown2",path + "FlameDownThrow2", 4, 12);

            AddAnimation(MRStates.FalconPunch,path + "ChargePUNCHFULL", 27, 18);

            AddAnimation(MRStates.Cursed, path + "SummonPillars2", 16, 18);
            AddAnimation(MRStates.Desruc, path + "SummonPillars", 7, 15);

            StandState summonState = AddState(MRStates.Spawn, 70);
            summonState.OnStateBegin += SummonState_OnStateBegin;
            summonState.OnStateEnd += GoIdle;
            summonState.OnStateUpdate += SummonState_OnStateUpdate;

            StandState idleState = AddState(MRStates.Idle.ToString());
            idleState.OnStateUpdate += Idle;

            StandState despawnState = AddState(MRStates.Despawn.ToString(), 80);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };

            StandState punchState = AddState(MRStates.Punch, AttackSpeed);

            punchState.OnStateBegin += BeginPunch;
            punchState.OnStateUpdate += UpdatePunch;
            punchState.OnStateEnd += EndPunch;
            punchState.OnStateEnd += PunchState_OnStateEnd;

            StandState falconState = AddState(MRStates.FalconPunch.ToString(), 180);
            falconState.OnStateBegin += FalconState_OnStateBegin;
            falconState.OnStateUpdate += UpdatePunch;
            falconState.OnStateUpdate += FalconState_OnStateUpdate;
            falconState.OnStateEnd += EndPunch;
            falconState.OnStateEnd += FalconState_OnStateEnd;

            StandState cursedState = AddState(MRStates.Cursed.ToString(), 51);
            cursedState.OnStateEnd += CursedState_OnStateBegin;
            cursedState.OnStateEnd += GoIdle;
            cursedState.OnStateUpdate += CursedState_OnStateUpdate;

            StandState desrucState = AddState(MRStates.Desruc.ToString(), 24);
            desrucState.OnStateEnd += DesrucState_OnStateBegin;
            desrucState.OnStateEnd += GoIdle;

            SetState(MRStates.Spawn.ToString());
        }

        private void CursedState_OnStateUpdate(StandState sender)
        {
            Owner.heldProj = projectile.whoAmI;
        }

        private void DesrucState_OnStateBegin(StandState sender)
        {
            Projectile.NewProjectile(Owner.Bottom + new Vector2(40, -60), new Vector2(1, 0), ModContent.ProjectileType<FirePillar>(), FirepillarDamage, 5.75f, Owner.whoAmI, 4, 1);
            Projectile.NewProjectile(Owner.Bottom + new Vector2(-40, -60), new Vector2(-1, 0), ModContent.ProjectileType<FirePillar>(), FirepillarDamage, 5.75f, Owner.whoAmI, 4, -1);
        }

        private void CursedState_OnStateBegin(StandState sender)
        {
            Projectile.NewProjectile(Owner.Bottom + new Vector2(40 * Owner.direction, -60), new Vector2(Owner.direction, 0), ModContent.ProjectileType<FirePillar>(), FirepillarDamage, 5.75f, Owner.whoAmI, 15, Owner.direction);
        }

        private void FalconState_OnStateEnd(StandState sender)
        {
        }

        private void FalconState_OnStateUpdate(StandState sender)
        {
            if (sender.TimeLeft == 30)
            {
                Main.PlaySound(SoundID.Item45);
                Projectile.NewProjectile(projectile.Center - projectile.Center.ToMouse(24f), Owner.Center.ToMouse(2f), ModContent.ProjectileType<Falcon>(), BirdDamage, 0f, projectile.owner);
                Main.PlaySound(SoundID.Item74);
            }
            if (sender.TimeLeft <= 30 && sender.TimeLeft > 15)
                projectile.damage = FalconDamage;
            else if (sender.TimeLeft <= 15)
                projectile.damage = 0;
        }

        private void MagicianRedProjectile_OnHit(PunchGhostProjectile attacker, Entity victim)
        {

            TBARPlayer.Get(Owner).AddStylePoints(500);
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
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
        }

        private void GoIdle(StandState sender)
        {
            SetState(MRStates.Idle.ToString());
        }

        private void SummonState_OnStateUpdate(StandState sender)
        {
            if (Animations[CurrentAnimation].CurrentFrame == 0)
                projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -32);

            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -32), 0.15f);
        }

        private int BirdDamage => 50 + (int)(BaseDPS * 1.35f);

        private int FalconDamage => 100 + (int)(BaseDPS * 10f);

        private int FireballDamage => 12 + (int)(BaseDPS * 0.75f);

        private int FirepillarDamage => 20 + (int)(BaseDPS * 2.25f);

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
        FalconPunch,
        Cursed,
        Desruc
    }
}
