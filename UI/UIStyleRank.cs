using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Helpers;
using TBAR.Players;
using Terraria;
using Terraria.UI;

namespace TBAR.UI
{
    public class UIStyleRank : UIDraggableElement
    {
        private float scale = 0.8f;

        private int delay;

        private readonly int[] bumpDelays = new int[] { 160, 120, 60, 40, 20, 20, 20 };

        private readonly float[] pulseSpeeds = new float[] { 0.02f, 0.025f, 0.03f, 0.035f, 0.04f, 0.04f, 0.05f };

        private readonly string[] noRepeats = new string[] { "Good!", "Superb!", "Rocking!" };

        private readonly string[] singleRepeat = new string[] { "Stagnating", "A Step Back", "An Old One" };

        private readonly string[] twoRepeats = new string[] { "Copycat", "One-Trick Pony", "Awful"};

        private readonly Dictionary<int, string[]> texts = new Dictionary<int, string[]>();

        private readonly Color[] repeatColors = new Color[] { Color.Lime, Color.Gold, Color.Crimson };

        private readonly Color[] colors = new Color[] { Color.Brown, Color.Cyan, Color.Lime, Color.Red, Color.Goldenrod, Color.Gold, Color.LightGoldenrodYellow };

        public UIStyleRank()
        {
            this.Width.Set(99, 0);
            this.Height.Set(64, 0);
            
            /// no repeats pool
            texts.Add(0, new string[] { "Good!", "Superb!", "Rocking!" });
            texts.Add(1, new string[] { "Stagnating", "A Step Back", "An Old One" });
            texts.Add(2, new string[] { "Copycat", "One-Trick Pony", "Awful" });
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gameMenu)
                return;

            if (LongBool)
                Main.isMouseLeftConsumedByUI = true;


            TBARPlayer plr = TBARPlayer.Get();

            if (delay <= 0 && (scale += pulseSpeeds[(int)plr.CurrentStyleRank]) > 1.4f)
            {
                delay = bumpDelays[(int)plr.CurrentStyleRank];
                scale = 0.8f;
            }

            delay--;

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            TBARPlayer plr = TBARPlayer.Get();

            int frame = (int)plr.CurrentStyleRank;

            Vector2 position = this.GetDimensions().Position();

            ///shit solution but i don't give a fuck
            int killMe = (int)MathHelper.Clamp(plr.RepeatCount, 0, 2);

            Rectangle rect = new Rectangle(0, 42 * frame, 99, 42);

            Color clr = plr.StylePoints > 0 ? Color.White : Color.White * 0f;
            Color pulseClr = delay <= 0 ? Color.White * 0.35f : Color.White * 0;

            float offXvideos = Main.fontMouseText.MeasureString(plr.StylePoints.ToString()).X / 3;

            Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, "Style Points: " + plr.StylePoints, position.X - offXvideos, position.Y + 44, colors[(int)plr.CurrentStyleRank], Color.Black, Vector2.Zero);

            string text = texts[plr.RepeatCount][plr.PoolID];

            /*switch(plr.RepeatCount)
            {
                case 0:
                    text = noRepeats[plr.PoolID];
                    break;
                case 1:
                    text = singleRepeat[plr.PoolID];
                    break;
                case 2:
                    text = twoRepeats[plr.PoolID];
                    break;
            }*/

            float off = Main.fontMouseText.MeasureString("Style: ").Y;
            float offX = Main.fontMouseText.MeasureString(text).X / 2;

            if(plr.LastUsedCombo != "")
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, "Quality: " + text, position.X - offX + 10, position.Y + 40 + off, repeatColors[killMe], Color.Black, Vector2.Zero);

            spriteBatch.Draw(Textures.StyleRanks, position + new Vector2(49.5f, 21), rect, clr, 0f, new Vector2(49.5f, 21), 1f, SpriteEffects.None, 1);

            if(plr.StylePoints > 0)
            spriteBatch.Draw(Textures.StyleRanks, position + new Vector2(49.5f, 21), rect, pulseClr, 0f, new Vector2(49.5f, 21), scale, SpriteEffects.None, 1);

            if (LongBool)
                DrawHelper.DrawRectangle(position, (int)Width.Pixels, (int)Height.Pixels, Color.DeepSkyBlue * 0.66f, spriteBatch);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (!LongBool)
                return;

            base.MouseDown(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            if (!LongBool)
                return;

            base.MouseDown(evt);
        }


        public bool LongBool => UIManager.Instance.StandAlbumLayer.State.Visible || TBAR.IsAdjustingUI;
    }
}
