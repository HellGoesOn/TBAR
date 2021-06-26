using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Helpers;
using TBAR.Players;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.Elements
{
    public class ComboTimeRunner : UIElement
    {
        private float fade;

        private bool isDragging;

        private Vector2 dragOffset;

        public ComboTimeRunner()
        {
            Width.Set(96, 0);
            Height.Set(20, 0);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (TBARConfig.runnerType != RunnerType.Static || !LongBool)
                return;

            isDragging = true;
            dragOffset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            if (TBARConfig.runnerType != RunnerType.Static || !LongBool)
                return;

            var end = evt.MousePosition;

            isDragging = false;

            Left.Set(end.X - dragOffset.X, 0);
            Top.Set(end.Y - dragOffset.Y, 0);
            Recalculate();
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

            if (isDragging)
            {
                if (!Main.mouseLeft)
                    isDragging = false;

                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Recalculate();
            }

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


            Left.Pixels = Utils.Clamp(Left.Pixels, 0, Main.screenWidth - Width.Pixels);
            Top.Pixels = Utils.Clamp(Top.Pixels, 0, Main.screenHeight - Height.Pixels);

            Recalculate();

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

        public bool LongBool => UIManager.Instance.StandAlbumLayer.State.Visible;
    }
}
