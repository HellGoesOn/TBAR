using Microsoft.Xna.Framework;
using TBAR.Components;
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
            projectile.width = 40;
            projectile.height = 40;
        }

        protected abstract int PunchState();

        protected void BeginPunch(StandState sender)
        {
            sender.CurrentAnimationID = PunchAnimationIDOffset();

            PunchStartPoint = Owner.Center;

            Owner.direction = MousePosition.X < Owner.Center.X ? -1 : 1;

            PunchDirection = PunchStartPoint.DirectTo(MousePosition, Owner.width + 16 * Range);

            projectile.damage = GetPunchDamage();
        }

        protected void UpdatePunch(StandState sender)
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center + PunchDirection, 0.35f);
            Owner.direction = (Owner.Center + PunchDirection).X < Owner.Center.X ? -1 : 1;
        }

        protected void EndPunch(StandState sender)
        {
            PunchStartPoint = PunchDirection = Vector2.Zero;
            projectile.damage = 0;
        }

        public override void HandleImmediateInputs(ComboInput input)
        {
            switch(input)
            {
                case ComboInput.Action1:
                case ComboInput.Action2:
                case ComboInput.Action3:
                    SetState(PunchState());
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

        public float Range { get; set; } = 2.0f;
    }
}
