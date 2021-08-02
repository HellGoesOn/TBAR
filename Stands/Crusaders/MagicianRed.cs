using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.Projectiles.Stands.Crusaders.Chicken;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Stands.Crusaders
{
    public sealed class MagicianRed : SingleEntityStand<MagicianRedProjectile>
    {
        public MagicianRed() : base("Magician's Red")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Crusaders/Chicken/Idle", 7, 15, true);
        }

        public override void InitializeCombos()
        {
            StandCombo falconPunch = new StandCombo("Fist of Flaming Fury", ComboInput.Action1, ComboInput.Action1, ComboInput.Action2);

            falconPunch.OnActivate += FalconPunch_OnActivate;
            falconPunch.Style = 1000;

            StandCombo zxcursed = new StandCombo("Fireraze", ComboInput.Action1, ComboInput.Action2, ComboInput.Action3)
            {
                Description = "Releases Pillars of Flames in the direction you are facing"
            };

            zxcursed.OnActivate += Zxcursed_OnActivate;
            zxcursed.Style = 500;
            StandCombo desrucxz = new StandCombo("Firequake", ComboInput.Action1, ComboInput.Action2, ComboInput.Action1)
            {
                Description = "Releases Pillars of Flames in both directions"
            };

            desrucxz.OnActivate += Desrucxz_OnActivate;
            desrucxz.Style = 250;

            AddNormalCombos(falconPunch, zxcursed, desrucxz);
        }

        private void Desrucxz_OnActivate(Player player)
        {
            ActiveInstance?.SetState(MRStates.Desruc.ToString());
        }

        private void Zxcursed_OnActivate(Player player)
        {
            ActiveInstance?.SetState(MRStates.Cursed.ToString());
        }

        private void FalconPunch_OnActivate(Player player)
        {
            ActiveInstance?.SetState(MRStates.FalconPunch.ToString());
        }

        public override bool CanAcquire(TBARPlayer player)
        {
            return true;
        }

        public override DamageType StandDamageType => DamageType.Magic;

        public override string GetDamageScalingText => "12 + 140%(75%) DPS";

        public override string GetEffectiveRangeText => "2m(melee)";
    }
}
