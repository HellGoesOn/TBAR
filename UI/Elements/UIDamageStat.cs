using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Enums;
using TBAR.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class UIDamageStat : UIElement
    {
        public UIDamageStat(DamageType type, bool hasHoverText = true, bool hasText = true)
        {
            DamageType = type;

            this.Width.Set(32, 0);
            this.Height.Set(32, 0);

            if (hasText)
            {
                DamageScaleText = new UIText("???");
                DamageScaleText.Left.Set(36, 0);
                DamageScaleText.VAlign = 0.5f;

                this.Append(DamageScaleText);
            }

            HasHoverText = hasHoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int widthBonus = DamageScaleText != null ? (int)(Main.fontMouseText.MeasureString(DamageScaleText.Text).X) + 8: 0;

            DrawHelper.DrawBorderedRectangle(this.GetDimensions().Position() - new Vector2(4), 40 + widthBonus, 40, new Color(new Vector3(0.4f)) * 0.5f, Color.Black * 0.75f, spriteBatch);

            Color color = DamageType == DamageType.Any ? Main.DiscoColor : Color.White;

            spriteBatch.Draw(Textures.DamageTypeIcon, base.GetDimensions().Position(), new Rectangle(0, 34 * (int)DamageType, 32, 34), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            if (IsMouseHovering && HasHoverText)
            {
                string text = $"This Stand benefits from {DamageType} weaponary.\nDPS is equal to your weapon's estimated DPS.";
                Vector2 mpos = this.Parent.GetDimensions().Position() + new Vector2(16, 8 + this.Parent.Height.Pixels);
                DrawHelper.DrawBorderedRectangle(mpos - new Vector2(8), (int)Main.fontMouseText.MeasureString(text).X + 16, 64, new Color(new Vector3(0.4f)) * 0.5f, Color.Black * 0.75f, spriteBatch);
                Utils.DrawBorderString(spriteBatch, text, mpos, Color.White);
            }
        }

        public bool HasHoverText { get; set; }

        public UIText DamageScaleText { get; set; }

        public DamageType DamageType { get; set; }
    }
}
