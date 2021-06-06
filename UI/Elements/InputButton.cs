using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class InputButton : UIElement
    {
        public InputButton(ComboInput input)
        {
            this.OverflowHidden = false;

            this.Width.Set(50, 0f);
            this.Height.Set(48, 0f);

            Text.VAlign = 0.35f;
            Text.HAlign = 0.5f;
            Text.TextColor = Color.Yellow;

            switch(input)
            {
                case ComboInput.Action1:
                    string s = TBARInputs.ComboButton1.GetAssignedKeys()[0].Replace("Oem", "");
                    Text.SetText(s);
                    break;
                case ComboInput.Action2:
                    s = TBARInputs.ComboButton2.GetAssignedKeys()[0].Replace("Oem", "");
                    Text.SetText(s);
                    break;
                case ComboInput.Action3:
                    s = TBARInputs.ComboButton3.GetAssignedKeys()[0].Replace("Oem", "");
                    Text.SetText(s);
                    break;
                case ComboInput.Up:
                    Text.SetText(Main.cUp);
                    break;
                case ComboInput.Down:
                    Text.SetText(Main.cDown);
                    break;
            }

            this.Append(Text);

            //Offset = (int)input;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // new Rectangle(0, 50 * Offset, 48, 48)
            spriteBatch.Draw(Textures.KeyboardInput, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        public UIText Text { get; } = new UIText("");

        //public int Offset { get; }
    }
}
