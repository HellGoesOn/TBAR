﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TBAR.Enums;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Stands;
using TBAR.Structs;
using Terraria;

namespace TBAR.Projectiles.Stands
{
    public abstract class PunchGhostProjectile : StandProjectile
    {
        public event OnHitEventHandler OnHit;

        protected PunchGhostProjectile(string name) : base(name)
        {
            PunchDirection = Vector2.Zero;
        }

        public override void SafeSetDefaults()
        {
            projectile.damage = 0;
            projectile.width = 60;
            projectile.height = 60;
            projectile.penetrate = 1;
            BaseDPS = -1;

            HitNPCs = new List<HitEntityData>();
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.penetrate++;

            OnHit?.Invoke(this, target);

            HitNPCs.Add(new HitEntityData(target.whoAmI, ElapsedTime, !NonTimedAttack));
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPCs.Count(x => x.Index == target.whoAmI) > 0)
                return false;

            return base.CanHitNPC(target);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            projectile.penetrate++;

            OnHit?.Invoke(this, target);
        }

        public override void PostAI()
        {
            base.PostAI();

            projectile.Center = projectile.Center.RetardedMethodName(Owner.Center, Range * 64f);

            if (BaseDPS == -1)
            {
                if (Owner.HeldItem.GetDamageType() == DamageType.Melee)
                    BaseDPS = Owner.HeldItem.GetDamageData(Owner).DPS;
                else
                    BaseDPS = 5;
            }

            ElapsedTime++;

            HitNPCs.RemoveAll(x => x.IsTimed && (x.TimeOfHit + AttackSpeed) < ElapsedTime);
        }

        protected abstract string PunchState { get; }

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
            projectile.Center = Vector2.Lerp(projectile.Center, MousePosition, 0.25f);
            Owner.direction = (Owner.Center + PunchDirection).X < Owner.Center.X ? -1 : 1;
            SpriteFX = Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        protected void EndPunch(StandState _)
        {
            PunchStartPoint = PunchDirection = Vector2.Zero;
            projectile.damage = 0;
            SetState("Idle");
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

        protected void ClearOnHitEffects()
        {
            if (OnHit != null)
            {
                foreach (var d in OnHit.GetInvocationList())
                    OnHit -= (d as OnHitEventHandler);
            }
        }

        protected virtual int GetPunchDamage() => 10 + BaseDPS;

        public virtual int GetBarrageDamage() => BaseDPS;

        public Vector2 PunchStartPoint { get; set; }

        public Vector2 PunchDirection { get; set; }

        public virtual bool CanPunch => true;

        public float Range { get; set; } = 2.0f;

        public Projectile Barrage { get; set; }

        public uint ElapsedTime { get; set; }

        public int AttackSpeed { get; set; } = 20;

        public bool NonTimedAttack { get; set; }

        public int BaseDPS { get; set; }

        public List<HitEntityData> HitNPCs { get; private set; }

        public bool IsIdle => State == "Idle";
    }
}
