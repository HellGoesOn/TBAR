using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Components;
using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.Elements.StandCard
{
    public class StandCard : UIElement
    {
        public StandCard()
        {
            Width.Set(140, 0);
            Height.Set(200, 0);
        }

        public void SetStand(Stand stand)
        {
            Stand = stand;
            StandDisplayName = Stand.StandName;

            Idle = Stand.AlbumEntryAnimation();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = base.GetDimensions();
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);

            if (this.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Stand stand = TBARPlayer.Get().PlayerStand;

            bool existsAndMatches = stand != null && stand.StandName == StandDisplayName;

            Texture2D texture = existsAndMatches ? Textures.StandCardCurrent : Textures.StandCard;

            spriteBatch.Draw(texture, dims.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            /*if (!Unlocked)
            {
                spriteBatch.Draw(Textures.Instance.SCUnknown, dims.Position() + new Vector2(70), null, Color.White, 0f, new Vector2(Textures.SCUnknown.Width / 2, Textures.SCUnknown.Height / 2), 1f, SpriteEffects.None, 1f);
                Utils.DrawBorderString(spriteBatch, "???", dims.Position() + new Vector2(56, 160), Color.White);
            }

            if (Unlocked)
            {*/
                Vector2 anchor = dims.Position() + new Vector2(16, 160);
                spriteBatch.Draw(Idle.SpriteSheet, dims.Position() + new Vector2(70), Idle.FrameRect, Color.White, 0f, Idle.FrameCenter, 1f, SpriteEffects.FlipHorizontally, 1f);
                Utils.DrawBorderString(spriteBatch, StandDisplayName, anchor, Color.White, 1);
            //}
        }

        public string StandUnlocalizedName { get; private set; }

        public string StandDisplayName { get; private set; }

        public string CallPath { get; }

        public Animation2D Idle { get; private set; }

        public Stand Stand { get; private set; }
    }
}
