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
    public abstract class SingleEntityStand<T> : Stand
        where T : StandProjectile
    {
        public SingleEntityStand(string name) : base(name)
        {
            AttachedStandProjectile = (T)Activator.CreateInstance(typeof(T));
        }

        public override void TryActivate(Player player)
        {
            int standIndex = Projectile.NewProjectile(player.Center, Vector2.Zero, TBAR.Instance.ProjectileType(AttachedStandProjectile.GetType().Name), 0, 1f, player.whoAmI);
            ActiveInstance = (T)Main.projectile[standIndex].modProjectile;
            IsActive = true;
        }

        public override void Update()
        {
            if (HasActiveStand && !ActiveInstance.projectile.active)
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
            ActiveInstance?.HandleImmediateInputs(input);
        }

        public override void KillStand()
        {
            IsActive = false;

            ActiveInstance?.projectile.Kill();

            ActiveInstance = null;
        }

        public bool HasActiveStand => ActiveInstance != null;

        public T ActiveInstance { get; set; }

        public T AttachedStandProjectile { get; set; }
    }
}
