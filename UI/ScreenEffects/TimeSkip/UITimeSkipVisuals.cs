﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Input;
using TBAR.UI.Elements;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class UITimeSkipVisuals : UIState
    {

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            TimeSkipManager.Instance.VFX.DrawEffect(spriteBatch);
        }

        public bool Visible { get; set; }
    }
}