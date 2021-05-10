using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Input;
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

            this.Append(Text);

            Offset = (int)input;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.KeyboardInput, GetDimensions().Position(), new Rectangle(0, 50 * Offset, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        public UIText Text { get; } = new UIText("");

        public int Offset { get; }
    }
}
