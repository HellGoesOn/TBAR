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

        private float moveItMoveIt;

        private Vector2 twentyMeterSplashSpawnPoint;

        private bool forwardFlip;

        public HierophantProjectile() : base("Hierophant Green")
        {
        }

        protected override string PunchState => HieroAI.NormalAttack.ToString();

        public override void InitializeStates(Projectile projectile)
        {
            Range = 2;

            string path = "Projectiles/Stands/Crusaders/Hierophant/";

            AddAnimation(HieroAI.Summon + "1", path + "Summon", 16, 12);
            AddAnimation(HieroAI.Summon + "2", path + "Summon2", 18, 16);
            AddAnimation(HieroAI.Idle, path + "Idle2", 10, 10, true);
            AddAnimation(HieroAI.NormalAttack + "1", path + "Attack1", 8, 15);
            AddAnimation(HieroAI.NormalAttack + "2", path + "Attack2", 8, 15);
            AddAnimation("MonkeyFlip", path + "MonkeyFlip", 9, 12);
            AddAnimation("MonkeyFlip2", path + "MonkeyFlip2", 12, 15);
            AddAnimation(HieroAI.EmeraldSplash, path + "EmeraldSplash", 10, 10);
            AddAnimation(HieroAI.TwentyMeterSplash, path + "Into20M", 13, 10);

            var summon = AddState(HieroAI.Summon, 80);
            summon.OnStateEnd += Summon_OnStateEnd;
            summon.OnStateUpdate += Summon_OnStateUpdate;

            var despawn = AddState(HieroAI.Despawn, 4);
            despawn.OnStateEnd += Despawn_OnStateEnd;

            var idle = AddState(HieroAI.Idle);
            idle.OnStateUpdate += Idle_OnStateUpdate;

            var emeraldSplash = AddState(HieroAI.EmeraldSplash, 120);
            emeraldSplash.OnStateBegin += EmeraldSplash_OnStateBegin;
            emeraldSplash.OnStateUpdate += EmeraldSplash_OnStateUpdate;
            emeraldSplash.OnStateEnd += EmeraldSplash_OnStateEnd;

            var normalAttack = AddState(HieroAI.NormalAttack, 30);
            normalAttack.OnStateBegin += NormalAttack_OnStateBegin;
            normalAttack.OnStateUpdate += NormalAttack_OnStateUpdate;
            normalAttack.OnStateEnd += NormalAttack_OnStateEnd;

            var hentai = AddState(HieroAI.HentaiAttack, 50);
            hentai.OnStateBegin += Hentai_OnStateBegin;
            hentai.OnStateEnd += Hentai_OnStateEnd;
            hentai.OnStateUpdate += Hentai_OnStateUpdate;

            var twentyMeterSplash = AddState(HieroAI.TwentyMeterSplash, 120);
            twentyMeterSplash.OnStateEnd += TwentyMeterSplash_OnStateEnd;
            twentyMeterSplash.OnStateUpdate += TwentyMeterSplash_OnStateUpdate;
            twentyMeterSplash.OnStateBegin += TwentyMeterSplash_OnStateBegin;

            CurrentAnimation = HieroAI.Summon.ToString() + Main.rand.Next(1, 3);

            SetState(HieroAI.Summon.ToString());
        }

        private void TwentyMeterSplash_OnStateBegin(StandState sender)
        {
            twentyMeterSplashSpawnPoint = MousePosition;
        }

        private void TwentyMeterSplash_OnStateUpdate(StandState sender)
        {
            var type = ModContent.ProjectileType<Tripwire>();

            if (sender.TimeLeft % 12 == 0)
            {
                var x = 640;
                var y = 0;
                var xx = Main.rand.Next(0, 200);
                var yy = 0;
                var spawnOffset = twentyMeterSplashSpawnPoint - new Vector2(x, y).RotatedByRandom(MathHelper.TwoPi);
                var velocity = spawnOffset.DirectTo(twentyMeterSplashSpawnPoint + new Vector2(xx, yy).RotatedByRandom(MathHelper.TwoPi));
                Projectile.NewProjectile(spawnOffset, velocity, type, EmeraldDamage, 0f, projectile.owner);
            }
        }

        private void TwentyMeterSplash_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void Hentai_OnStateUpdate(StandState sender)
        {
            Owner.direction = attackDir;
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center + Owner.Center.DirectTo(MousePosition, Range * 32), 0.35f);

            Owner.heldProj = projectile.whoAmI;

            var frameRequired = forwardFlip ? 5 : 3;

            if (Animations[CurrentAnimation].CurrentFrame == frameRequired && !hasShot)
            {
                hasShot = true;
                var type = ModContent.ProjectileType<MILFHunterTendril>();
                var mult = SpriteFX == SpriteEffects.FlipHorizontally ? 1 : -1;
                Vector2 off = new Vector2(30 * mult, -4);
                var pos = projectile.Center + off;
                int proj = Projectile.NewProjectile(pos, pos.DirectTo(MousePosition, 18f), type, EmeraldDamage * 2, 2f, projectile.owner, MILFHunterTendril.TendrilLength, projectile.whoAmI);


                if (Main.projectile[proj].modProjectile is MILFHunterTendril tendril)
                {
                    var flip = forwardFlip;
                    tendril.flipped = Owner.direction == -1? flip : !flip;
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

            forwardFlip = Main.rand.NextBool();

            if (forwardFlip)
                CurrentAnimation = "MonkeyFlip2";
            else
                CurrentAnimation = "MonkeyFlip";
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
            Owner.direction = emeraldSplashDirection.X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var offset = Owner.Center.DirectTo(MousePosition, Range * 16) + new Vector2(0, 32);
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center - offset, 0.35f);

            if (sender.TimeLeft % 4 == 0 && sender.TimeLeft <= 80)
            {
                hasShot = true;
                var type = ModContent.ProjectileType<SharpEmerald>();
                var mult = SpriteFX == SpriteEffects.FlipHorizontally ? 1 : -1;
                Vector2 off = new Vector2(-12 * mult, 12);
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
            var offset = Owner.Center.DirectTo(MousePosition, Range * 16) + new Vector2(14 * Owner.direction, 16);
            projectile.Center = Vector2.SmoothStep(projectile.Center, Owner.Center - offset, 0.35f);

            if (Animations[CurrentAnimation].CurrentFrame == 4 && !hasShot)
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
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.EmeraldBolt, 0, -3);
            }
            Main.PlaySound(SoundID.Shatter);
            projectile.Kill();
        }

        private void Summon_OnStateUpdate(StandState sender)
        {
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Animations[CurrentAnimation].CurrentFrame == 11 && !cocoonBursted && CurrentAnimation == HieroAI.Summon + "1")
            {
                Main.PlaySound(SoundID.Shatter);
                cocoonBursted = true;
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.EmeraldBolt, 0, -3);
                }
            }

            projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, -24);
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

        public override void PostAI()
        {
            base.PostAI();

            if (moveItMoveIt <= 1f)
                moveItMoveIt += 0.0075f;
            else
                moveItMoveIt = -1f;
        }

        private void Summon_OnStateEnd(StandState sender)
        {
            GoIdle();
        }

        private void GoIdle() => SetState(HieroAI.Idle.ToString());

        protected override string GetPunchVariation()
        {
            return HieroAI.NormalAttack.ToString() + Main.rand.Next(1, 3);
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
                case ImmediateInput.Action2:
                    SetState(HieroAI.TwentyMeterSplash);
                    break;
                default:
                    return;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Scale = Vector2.One;
            // time get shwifty in here
            var shader = TBAR.Instance.GetEffect("Effects/HieroShader2");

            var anim = Animations[CurrentAnimation];

            var frame = new Vector4(anim.FrameRect.X, anim.FrameRect.Y, anim.FrameRect.Width, anim.FrameRect.Height);

            var imageSize = new Vector2(anim.SpriteSheet.Width, anim.SpriteSheet.Height);
            var imageSize2 = new Vector2(86);

            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            shader.GraphicsDevice.Textures[1] = Textures.HieroMask;
            shader.Parameters["frame"].SetValue(frame);
            shader.Parameters["offset"].SetValue(new Vector2(moveItMoveIt));
            shader.Parameters["imageSize"].SetValue(imageSize);
            shader.Parameters["imageSize2"].SetValue(imageSize2 / 4);
            shader.Parameters["pixelation"].SetValue(10);

            shader.CurrentTechnique.Passes[0].Apply();

            DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX); 
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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
        HentaiAttack,
        TwentyMeterSplash,
    }
}
