using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands.Crusaders.Hierophant
{
    // otherwise known as Green Dickhead
    public class HierophantProjectile : PunchGhostProjectile
    {
        private bool cocoonBursted;
        private bool hasShot;

        private Vector2 emeraldSplashDirection;

        private int attackDir;


        public HierophantProjectile() : base("Hierophant Green")
        {
        }

        protected override string PunchState => HieroAI.NormalAttack.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 2;

            string path = "Projectiles/Stands/Crusaders/Hierophant/";

            AddAnimation(HieroAI.Summon, path + "Summon", 15);
            AddAnimation(HieroAI.Idle, path + "Idle", 6, 10, true);
            AddAnimation(HieroAI.NormalAttack, path + "NormalAttack", 9, 20);
            AddAnimation(HieroAI.EmeraldSplash, path + "EmeraldSplash", 6, 20, true);

            var summon = AddState(HieroAI.Summon, 80);
            summon.OnStateEnd += Summon_OnStateEnd;
            summon.OnStateUpdate += Summon_OnStateUpdate;

            var despawn = AddState(HieroAI.Despawn, 4);
            despawn.OnStateEnd += Despawn_OnStateEnd;

            var idle = AddState(HieroAI.Idle);
            idle.OnStateUpdate += Idle_OnStateUpdate;

            var emeraldSplash = AddState(HieroAI.EmeraldSplash, 80);
            emeraldSplash.OnStateBegin += EmeraldSplash_OnStateBegin;
            emeraldSplash.OnStateUpdate += EmeraldSplash_OnStateUpdate;
            emeraldSplash.OnStateEnd += EmeraldSplash_OnStateEnd;

            var normalAttack = AddState(HieroAI.NormalAttack, 30);
            normalAttack.OnStateBegin += NormalAttack_OnStateBegin;
            normalAttack.OnStateUpdate += NormalAttack_OnStateUpdate;
            normalAttack.OnStateEnd += NormalAttack_OnStateEnd;

            var hentai = AddState(HieroAI.HentaiAttack, 25);
            hentai.OnStateBegin += Hentai_OnStateBegin;
            hentai.OnStateEnd += Hentai_OnStateEnd;
            hentai.OnStateUpdate += Hentai_OnStateUpdate;

            SetState(HieroAI.Summon.ToString());
        }

        private void Hentai_OnStateUpdate(StandState sender)
        {
            Owner.direction = attackDir;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + Owner.Center.DirectTo(MousePosition, Range * 32), 0.35f);

            Owner.heldProj = projectile.whoAmI;
            if (Animations[CurrentAnimation].CurrentFrame == 3 && !hasShot)
            {
                hasShot = true;
                var type = ModContent.ProjectileType<MILFHunterTendril>();
                var mult = SpriteFX == SpriteEffects.FlipHorizontally ? 1 : -1;
                Vector2 off = new Vector2(30 * mult, -4);
                var pos = projectile.Center + off;
                int proj = Projectile.NewProjectile(pos, pos.DirectTo(MousePosition, 18f), type, EmeraldDamage * 2, 2f, projectile.owner, MILFHunterTendril.TendrilLength, projectile.whoAmI);


                if (Main.projectile[proj].modProjectile is MILFHunterTendril tendril)
                {
                    tendril.flipped = Main.rand.NextBool();
                }
            }
        }

        private void Hentai_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void Hentai_OnStateBegin(StandState sender)
        {
            hasShot = false;
            attackDir = MousePosition.X < Owner.Center.X ? -1 : 1;

            Owner.direction = attackDir;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void EmeraldSplash_OnStateBegin(StandState sender)
        {
            emeraldSplashDirection = MousePosition;
            hasShot = false;
            attackDir = MousePosition.X < Owner.Center.X ? -1 : 1;

            Owner.direction = attackDir;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void EmeraldSplash_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void EmeraldSplash_OnStateUpdate(StandState sender)
        {
            Owner.heldProj = projectile.whoAmI;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var offset = Owner.Center.DirectTo(MousePosition, Range * 16) + new Vector2(0, 12);
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center - offset, 0.35f);

            if (CurrentState.TimeLeft % 4 == 0)
            {
                hasShot = true;
                var type = ModContent.ProjectileType<SharpEmerald>();
                var mult = SpriteFX == SpriteEffects.FlipHorizontally ? 1 : -1;
                Vector2 off = new Vector2(12 * mult, -4);
                var pos = projectile.Center + off;

                Projectile.NewProjectile(pos, pos.DirectTo(emeraldSplashDirection, 22f).RotatedByRandom(0.027f), type, (int)(EmeraldDamage * 0.75f), 2f, projectile.owner, 3, 2);
            }
        }

        private void NormalAttack_OnStateBegin(StandState sender)
        {
            attackDir = MousePosition.X < Owner.Center.X ? -1 : 1;
            hasShot = false;
            Owner.direction = attackDir;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void NormalAttack_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void NormalAttack_OnStateUpdate(StandState sender)
        {
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = attackDir;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var offset = Owner.Center.DirectTo(MousePosition, Range * 16) + new Vector2(0, 12);
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center - offset, 0.35f);

            if (Animations[CurrentAnimation].CurrentFrame == 5 && !hasShot)
            {
                hasShot = true;
                var type = ModContent.ProjectileType<SharpEmerald>();
                var mult = SpriteFX == SpriteEffects.FlipHorizontally ? 1 : -1;
                Vector2 off = new Vector2(18 * mult, -4);
                var pos = projectile.Center + off;
                for(int i = 0; i < 2; i++)
                Projectile.NewProjectile(pos, pos.DirectTo(MousePosition, 18f).RotatedByRandom(0.07f), type, EmeraldDamage, 2f, projectile.owner);
            }
        }

        private void Despawn_OnStateEnd(StandState sender)
        {
            for(int i = 0; i < 100; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0, -3);
            }
            Main.PlaySound(SoundID.Shatter);
            projectile.Kill();
        }

        private void Summon_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Animations[CurrentAnimation].CurrentFrame == 8 && !cocoonBursted)
            {
                Main.PlaySound(SoundID.Shatter);
                cocoonBursted = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0, -3);
                }
            }

            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -24), 0.15f);
        }

        private void Idle_OnStateUpdate(StandState sender)
        {
            ClearOnHitEffects();
            NonTimedAttack = false;
            HitNPCs.RemoveAll(x => !x.IsTimed);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + new Vector2(-30 * Owner.direction, -24), 0.15f);

            if (Owner.whoAmI == Main.myPlayer && TBARInputs.SummonStand.JustPressed && IsIdle)
            {
                SetState(HieroAI.Despawn.ToString());
            }

            if (Owner.controlUseTile && Owner.whoAmI == Main.myPlayer)
                SetState(HieroAI.HentaiAttack, GetPunchVariation());
        }

        private void Summon_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void GoIdle() => SetState(HieroAI.Idle.ToString());

        protected override string GetPunchVariation()
        {
            return HieroAI.NormalAttack.ToString();
        }

        public override void HandleImmediateInputs(ImmediateInput input)
        {
            switch (input)
            {
                case ImmediateInput.LeftClick:
                    if (CanPunch)
                        SetState(PunchState, GetPunchVariation());
                    break;
                case ImmediateInput.RightClick:
                    if (CanPunch)
                        SetState(HieroAI.HentaiAttack, GetPunchVariation());
                    break;
                case ImmediateInput.Action1:
                    SetState(HieroAI.EmeraldSplash);
                    break;
                default:
                    return;
            }
        }

        private int EmeraldDamage => 12 + (int)(BaseDPS * 1.75f);
    }

    public enum HieroAI
    {
        Summon,
        Idle,
        Despawn,
        NormalAttack,
        EmeraldSplash,
        HentaiAttack
    }
}
