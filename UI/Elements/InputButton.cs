using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class InputButton : UIElement
    {
        public InputButton(string text)
        {
            Name = text;
            this.OverflowHidden = false;

            this.Width.Set(50, 0f);
            this.Height.Set(48, 0f);

            Text.VAlign = 0.35f;
            Text.HAlign = 0.5f;
            Text.TextColor = Color.Yellow;

            this.Append(Text);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Instance.KeyboardInput, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        public UIText Text { get; } = new UIText("");

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                Text.SetText(name);
            }
        }
    }
}
