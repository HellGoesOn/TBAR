using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Helpers;
using TBAR.Players;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class ComboTimeRunner : UIDraggableElement
    {
        private float fade;

        public ComboTimeRunner()
        {
            Width.Set(96, 0);
            Height.Set(20, 0);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (TBARConfig.runnerType != RunnerType.Static || !LongBool)
                return;

            base.MouseDown(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            if (TBARConfig.runnerType != RunnerType.Static || !LongBool)
                return;

            base.MouseDown(evt);
        }

        public override void Update(GameTime gameTime)
        {
            if (TBARConfig.runnerType == RunnerType.Static && LongBool)
            {
                if (ContainsPoint(Main.MouseScreen))
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }

            if (LongBool)
                Main.isMouseLeftConsumedByUI = true;

            if (!Visible)
            {
                if (fade > 0)
                    fade -= 0.025f;
            }
            else
            {
                AchievedCombo = false;
                fade = 2.5f;
            }

            if (LongBool)
                fade = 2.5f;


            base.Update(gameTime);

        }

        public Color GetColor()
        {
            if(TBARPlayer.Get().ComboTimeExpired)
            {
                if (AchievedCombo)
                    return Color.Green;
                return Color.Red;
            }

            return Color.White;
        }

        public float GetProgress()
        {
            TBARPlayer plr = TBARPlayer.Get();

            float progress = (plr.ComboTime * 88) / TBARConfig.inputDelay;

            return progress;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 position = Vector2.Zero;

            switch (TBARConfig.runnerType)
            {
                case RunnerType.Default:
                    Vector2 off = new Vector2(0, 30 + Main.LocalPlayer.gfxOffY);
                    Vector2 src = new Vector2(Textures.RunnerBounds.Width / 2, Textures.RunnerBounds.Height / 2);
                    position = Main.LocalPlayer.Center - Main.screenPosition - src + off;
                    break;
                case RunnerType.Mouse:
                    position = Main.MouseScreen - new Vector2(Textures.RunnerBounds.Width / 2, Textures.RunnerBounds.Height / 2) + new Vector2(10, 30);
                    break;
                case RunnerType.Disabled:
                    return;
                case RunnerType.Static:
                    position = this.GetDimensions().Position();
                    break;
            }

            float progress = 88 - GetProgress();

            Vector2 offset = new Vector2(progress, 0);

            spriteBatch.Draw(Textures.Runner, position, null, GetColor() * fade, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(Textures.RunnerBounds, position, null, Color.White * fade, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);


            spriteBatch.Draw(Textures.Tooth, position + offset, null, Color.White * fade, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            if (LongBool)
                DrawHelper.DrawRectangle(position, (int)Width.Pixels, (int)Height.Pixels, Color.DeepSkyBlue * 0.66f, spriteBatch);
        }

        public bool Visible => TBARPlayer.Get().ComboTime > 0;

        public static bool AchievedCombo { get; set; }

        public bool LongBool => UIManager.Instance.StandAlbumLayer.State.Visible || TBAR.IsAdjustingUI;
    }
}
