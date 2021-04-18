using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TBAR.Input;
using TBAR.Players;
using TBAR.Projectiles.Stands;
using Terraria;

namespace TBAR.Stands
{
    /// <summary>
    /// Class used as a base for stands that are represented by a singular entity
    /// Examples: Star Platinum, The Emperor, Aerosmith
    /// </summary>
    public abstract class SingleEntityStand : Stand
    {
        public SingleEntityStand(StandProjectile attachedStand, string name) : base(name)
        {
            AttachedStandProjectile = attachedStand;
        }

        public override void TryActivate(Player player)
        {
            int standIndex = Projectile.NewProjectile(player.Center, Vector2.Zero, TBAR.Instance.ProjectileType(AttachedStandProjectile.GetType().Name), 1, 1f, player.whoAmI);
            ActiveStandProjectile = (StandProjectile)Main.projectile[standIndex].modProjectile;
            IsActive = true;
        }

        public override void Update()
        {
            if (HasActiveStand && !ActiveStandProjectile.projectile.active)
                KillStand();
        }

        public override void HandleInputs(Player player, List<ComboInput> receivedInputs)
        {
            if (player.whoAmI != Main.myPlayer || receivedInputs.Count <= 0)
                return;

            if (IsActive)
            {
                foreach (StandCombo combo in NormalCombos)
                    if (combo.TryActivate(player, receivedInputs))
                        return;
            }

            foreach (StandCombo combo in GlobalCombos)
                if (combo.TryActivate(player, receivedInputs))
                    return;
        }

        public override void HandleImmediateInputs(Player player, ImmediateInput input)
        {
            ActiveStandProjectile?.HandleImmediateInputs(input);
        }

        public override void KillStand()
        {
            IsActive = false;

            ActiveStandProjectile?.projectile.Kill();

            ActiveStandProjectile = null;
        }

        public bool HasActiveStand => ActiveStandProjectile != null;

        public StandProjectile ActiveStandProjectile { get; set; }

        public StandProjectile AttachedStandProjectile { get; set; }
    }
}
