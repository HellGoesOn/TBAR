using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.Components;
using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.Elements.StandCard
{
    public class StandCard : UIElement
    {
        public StandCard(Stand stand)
        {
            Width.Set(140, 0);
            Height.Set(200, 0);

            Stand = stand;
            StandDisplayName = Stand.StandName;

            Idle = Stand.AlbumEntryAnimation();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (Main.GameUpdateCount % 1 == 0)
                Idle.Update();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = base.GetDimensions();
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);

            if (this.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Texture2D texture = TBARPlayer.Get().PlayerStand.StandName == StandDisplayName ? Textures.StandCardCurrent : Textures.StandCard;

            spriteBatch.Draw(texture, dims.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            /*if (!Unlocked)
            {
                spriteBatch.Draw(Textures.Instance.SCUnknown, dims.Position() + new Vector2(70), null, Color.White, 0f, new Vector2(Textures.SCUnknown.Width / 2, Textures.SCUnknown.Height / 2), 1f, SpriteEffects.None, 1f);
                Utils.DrawBorderString(spriteBatch, "???", dims.Position() + new Vector2(56, 160), Color.White);
            }

            if (Unlocked)
            {
                Vector2 anchor = dims.Position() + new Vector2(16, 160);
                spriteBatch.Draw(Idle.SpriteSheet, dims.Position() + new Vector2(70), Idle.FrameRect, Color.White, 0f, Idle.DrawOrigin, 1f, SpriteEffects.None, 1f);
                Utils.DrawBorderString(spriteBatch, StandDisplayName, anchor, Color.White, 1);
            }*/
        }

        public string StandUnlocalizedName { get; }

        public string StandDisplayName { get; }

        public string CallPath { get; }

        public SpriteAnimation Idle { get; }

        public Stand Stand { get; }
    }
}
