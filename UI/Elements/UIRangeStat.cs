using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Enums;
using TBAR.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class UIRangeStat : UIElement
    {
        public UIRangeStat(bool hasText = true)
        {
            this.Width.Set(32, 0);
            this.Height.Set(32, 0);

            if (hasText)
            {
                RangeText = new UIText("???");
                RangeText.Left.Set(36, 0);
                RangeText.VAlign = 0.5f;

                this.Append(RangeText);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int widthBonus = RangeText != null ? (int)(Main.fontMouseText.MeasureString(RangeText.Text).X) + 8 : 0;

            DrawHelper.DrawBorderedRectangle(this.GetDimensions().Position() - new Vector2(4), 40 + widthBonus, 40, new Color(new Vector3(0.4f)) * 0.5f, Color.Black * 0.75f, spriteBatch);

            spriteBatch.Draw(Textures.RangeIcon, base.GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            if (IsMouseHovering)
            {
                string text = "Stand's effective range";
                Vector2 mpos = this.Parent.GetDimensions().Position() + new Vector2(16, 8 + this.Parent.Height.Pixels);
                DrawHelper.DrawBorderedRectangle(mpos - new Vector2(8), (int)Main.fontMouseText.MeasureString(text).X + 16, 36, new Color(new Vector3(0.4f)) * 0.5f, Color.Black * 0.75f, spriteBatch);
                Utils.DrawBorderString(spriteBatch, text, mpos, Color.White);
            }
        }

        public UIText RangeText { get; set; }
    }
}
