using Terraria;
using TBAR.Input;
using Terraria.GameInput;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TBAR.Stands;
using Terraria.ID;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        private const int COMBO_TIME = 90;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TBARInputs.SummonStand.JustPressed && IsStandUser && !PlayerStand.IsActive)
            {
                PlayerStand.TryActivate(player);
            }

            if(IsStandUser)
            {
                if (TBARInputs.ComboButton1.JustPressed)
                    OnInput(ComboInput.Action1);

                if (TBARInputs.ComboButton2.JustPressed)
                    OnInput(ComboInput.Action2);

                if (TBARInputs.ComboButton3.JustPressed)
                    OnInput(ComboInput.Action3);

                if (HasPressedUp)
                    OnInput(ComboInput.Up);

                if (HasPressedDown)
                    OnInput(ComboInput.Down);

                if(HasPressedLeftClick)
                    PlayerStand.HandleImmediateInputs(player, ImmediateInput.LeftClick);

                if (HasPressedRightClick)
                    PlayerStand.HandleImmediateInputs(player, ImmediateInput.RightClick);

                OldUpButtonState = player.controlUp;
                OldDownButtonState = player.controlDown;
                OldLeftClickState = player.controlUseItem;
                OldRightClickState = player.controlUseTile;
            }
        }

        public void UpdateInputs()
        {
            if(ComboTimeExpired)
            {
                PlayerStand.HandleInputs(player, CurrentComboInputs);

                CurrentComboInputs.Clear();
            }
        }

        private void OnInput(ComboInput input)
        {
            ComboTime = COMBO_TIME;

            CurrentComboInputs.Add(input);

            PlayerStand.HandleImmediateInputs(player, (ImmediateInput)input);
        }

        public bool OldUpButtonState { get; private set; }
        public bool HasPressedUp => player.controlUp && !OldUpButtonState;

        public bool OldDownButtonState { get; private set; }
        public bool HasPressedDown => player.controlDown && !OldDownButtonState;

        public bool OldLeftClickState { get; private set; }
        public bool HasPressedLeftClick => player.controlUseItem && !OldLeftClickState;

        public bool OldRightClickState { get; private set; }
        public bool HasPressedRightClick => player.controlUseTile && !OldRightClickState;

        public int ComboTime { get; set; }

        public bool ComboTimeExpired => ComboTime <= 0;

        public List<ComboInput> CurrentComboInputs { get; private set; }
    }
}
