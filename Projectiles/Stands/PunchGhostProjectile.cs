using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using Terraria;

namespace TBAR.Projectiles.Stands
{
    public abstract class PunchGhostProjectile : StandProjectile
    {
        protected PunchGhostProjectile(string name) : base(name)
        {
            PunchDirection = Vector2.Zero;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
        }

        protected abstract int PunchState { get; }

        protected void BeginPunch(StandState sender)
        {
            sender.CurrentAnimationID = PunchAnimationIDOffset();

            PunchStartPoint = Owner.Center;

            Owner.direction = MousePosition.X < Owner.Center.X ? -1 : 1;

            PunchDirection = PunchStartPoint.DirectTo(MousePosition, Owner.width + 16 * Range);

            projectile.damage = GetPunchDamage();
        }

        protected void UpdatePunch(StandState _)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Owner.direction = (Owner.Center + PunchDirection).X < Owner.Center.X ? -1 : 1;
        }

        protected void EndPunch(StandState _)
        {
            PunchStartPoint = PunchDirection = Vector2.Zero;
            projectile.damage = 0;
        }

        public override void HandleImmediateInputs(ImmediateInput input)
        {
            switch(input)
            {
                case ImmediateInput.Action1:
                case ImmediateInput.Action2:
                case ImmediateInput.Action3:
                case ImmediateInput.LeftClick:
                case ImmediateInput.RightClick:
                    if(CanPunch)
                        SetState(PunchState);
                    break;
                default:
                    return;
            }
        }

        protected virtual int PunchAnimationIDOffset()
        {
            return 0;
        }

        protected virtual int GetPunchDamage() => GetBaseDamage(DamageType.Melee, Owner);

        public Vector2 PunchStartPoint { get; set; }

        public Vector2 PunchDirection { get; set; }

        public virtual bool CanPunch => true;

        public float Range { get; set; } = 2.0f;

        public Projectile Barrage { get; set; }
    }
}
