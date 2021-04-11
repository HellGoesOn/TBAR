using Terraria;
using TBAR.Input;
using Terraria.GameInput;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TBAR.Stands;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        private const int COMBO_TIME = 120;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TBARInputs.SummonStand.JustPressed && IsStandUser && !HasActiveStand)
            {
                int standIndex = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 1, 1f, player.whoAmI);
                ActiveStandProjectile = Main.projectile[standIndex];
            }

            if(HasActiveStand)
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

                OldUpButtonState = player.controlUp;
                OldDownButtonState = player.controlDown;
            }
        }

        public void UpdateInputs()
        {
            if (ComboTime > 0)
                ComboTime--;

            if(ComboTimeExpired)
            {
                Stand stand = (Stand)ActiveStandProjectile.modProjectile;

                stand.ReceivedInputs.AddRange(CurrentComboInputs);

                CurrentComboInputs.Clear();
            }
        }

        private void OnInput(ComboInput input)
        {
            ComboTime = COMBO_TIME;

            CurrentComboInputs.Add(input);
            Main.NewText("Added " + input.ToString());
            Main.NewText("Total " + CurrentComboInputs.Count);
        }

        public bool OldUpButtonState { get; private set; }

        public bool HasPressedUp => player.controlUp && !OldUpButtonState;

        public bool OldDownButtonState { get; private set; }

        public bool HasPressedDown => player.controlDown && !OldDownButtonState;

        public int ComboTime { get; set; }

        public bool ComboTimeExpired => ComboTime <= 0;

        public List<ComboInput> CurrentComboInputs { get; } = new List<ComboInput>(10);
    }
}
