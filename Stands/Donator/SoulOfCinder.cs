using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using TBAR.Projectiles.Stands.Donator.SoulOfCinder;
using Terraria;

namespace TBAR.Stands.Donator
{
    public class SoulOfCinder : SingleEntityStand<SoulOfCinderProjectile>
    {
        public SoulOfCinder() : base("Soul of Cinder")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Donator/SoulOfCinder/Idle", 10, 15, true);
        }

        public override void HandleImmediateInputs(Player player, ImmediateInput input)
        {
            base.HandleImmediateInputs(player, input);

            if(ActiveInstance != null)
                switch (input)
                {
                    case ImmediateInput.Action1:
                        if (ActiveInstance.swingCounter == 2 && (ActiveInstance.IsIdle || ActiveInstance.State == SOCStates.Swing.ToString()))
                        {
                            ActiveInstance.swingCounter = 0;
                            ActiveInstance.SetState(SOCStates.Ability1.ToString());
                        }
                        break;
                }
        }

        public override void InitializeCombos()
        {
            StandCombo ability1 = new StandCombo("Flame Path", ComboInput.Action1)
            {
                Description = "Sends a flaming snake along the ground.\nCan only be done if used a third swing in a swing chain."
            };

            StandCombo soulStream = new StandCombo("Soul Stream", ComboInput.Action2);
            soulStream.OnActivate += SoulStream_OnActivate;

            StandCombo barrage = new StandCombo("Barrage", ComboInput.Action3, ComboInput.Action2, ComboInput.Action3)
            {
                Style = 500
            };

            barrage.OnActivate += Barrage_OnActivate;
            AddNormalCombos(ability1, soulStream, barrage);
        }

        private void SoulStream_OnActivate(Player player)
        {
            if (ActiveInstance.IsIdle)
                ActiveInstance.SetState(SOCStates.SoulStream.ToString());
        }

        private void Barrage_OnActivate(Player player)
        {
            ActiveInstance.swingCounter = 0;
            if(ActiveInstance.IsIdle || ActiveInstance.State == SOCStates.Swing.ToString())
            ActiveInstance.SetState(SOCStates.Barrage.ToString());
        }

        public override bool CanAcquire(TBARPlayer player)
        {
            return false;
        }
    }
}
