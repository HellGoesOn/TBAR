using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ID;

namespace TBAR.Projectiles.Stands.Crusaders.Chicken
{
    public class MagicianRedProjectile : PunchGhostProjectile
    {
        public MagicianRedProjectile() : base("Magician's Red")
        {
        }

        protected override string PunchState => MRStates.Punch.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 2f;

            string path = "Projectiles/Stands/Crusaders/Chicken/";

            SpriteAnimation summon = new SpriteAnimation(path + "Spawn", 23, 18);
            SpriteAnimation despawn = new SpriteAnimation(path + "Spawn", 23, 18) { IsReversed = true };
            SpriteAnimation idle = new SpriteAnimation(path + "Idle", 7, 10, true);

            StandState summonState = new StandState(MRStates.Spawn.ToString(), summon);
            summonState.OnStateBegin += SummonState_OnStateBegin;
            summonState.OnStateEnd += GoIdle;
            summonState.OnStateUpdate += SummonState_OnStateUpdate;

            StandState idleState = new StandState(MRStates.Idle.ToString(), idle);
            idleState.OnStateUpdate += Idle;

            StandState despawnState = new StandState(MRStates.Despawn.ToString(), despawn);
            despawnState.OnStateEnd += delegate { projectile.Kill(); };


            AddStates(summonState, idleState, despawnState);

            SetState(MRStates.Spawn.ToString());
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

        private int flameCounter;
    }

    public enum MRStates
    {
        Spawn,
        Idle,
        Despawn,
        Punch
    }
}
