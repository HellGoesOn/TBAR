using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TBAR.Input;
using TBAR.Players;
using TBAR.UI.Elements;
using TBAR.UI.Elements.ComboPanel;
using TBAR.UI.Elements.StandCard;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class StandAlbumUIState : UIState
    {
        public override void OnInitialize()
        {
            MainPanel = new UIPanel();
            MainPanel.Width.Set(720, 0);
            MainPanel.Height.Set(440, 0);
            MainPanel.HAlign = 0.5f;
            MainPanel.VAlign = 0.5f;

            UIPanel standInfoPanel = new UIPanel();
            standInfoPanel.Width.Set(180, 0);
            standInfoPanel.Height.Set(400, 0);
            standInfoPanel.HAlign = 0.005f;
            standInfoPanel.VAlign = 0.5f;
            standInfoPanel.BackgroundColor = new Color(new Vector3(0.2f)) * 0.5f;

            UIPanel gridPanel = new UIPanel();
            gridPanel.Width.Set(ComboPanel.WIDTH + 68, 0);
            gridPanel.Height.Set(ComboPanel.CLOSED_HEIGHT * 3 + 40, 0);
            gridPanel.HAlign = 0.95f;
            gridPanel.VAlign = 0.5f;
            gridPanel.BackgroundColor = new Color(new Vector3(0.2f)) * 0.5f;

            UIScrollbar scrollBar = new UIScrollbar();
            scrollBar.Width.Set(24, 0);
            scrollBar.Height.Set(380, 0);
            scrollBar.HAlign = 0.985f;
            scrollBar.VAlign = 0.5f;

            Card = new StandCard();
            Card.HAlign = 0.5f;
            Card.VAlign = 0.015f;

            ComboGrid = new UIGrid();
            ComboGrid.Width.Set(ComboPanel.WIDTH + 40, 0);
            ComboGrid.Height.Set(ComboPanel.CLOSED_HEIGHT * 3 + 120, 0);
            ComboGrid.SetScrollbar(scrollBar);

            ComboGrid.HAlign = 0.5f;
            ComboGrid.VAlign = 0.5f;

            gridPanel.Append(ComboGrid);
            gridPanel.Append(scrollBar);

            MainPanel.Append(gridPanel);

            standInfoPanel.Append(Card);

            MainPanel.Append(standInfoPanel);
            base.Append(MainPanel);
        }

        public void OnOpen()
        {
            ComboGrid.Clear();

            TBARPlayer plr = TBARPlayer.Get();

            if(plr.IsStandUser)
            {
                Card.SetStand(plr.PlayerStand);

                List<StandCombo> combos = new List<StandCombo>(plr.PlayerStand.GlobalCombos.Count + plr.PlayerStand.NormalCombos.Count);

                combos.AddRange(plr.PlayerStand.GlobalCombos);
                combos.AddRange(plr.PlayerStand.NormalCombos);

                combos.Reverse();

                foreach (StandCombo c in combos)
                    ComboGrid.Add(new ComboPanel(c));
            }

            Recalculate();
            RecalculateChildren();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(MainPanel.ContainsPoint(new Vector2(Main.mouseX, Main.mouseY)))
                Main.LocalPlayer.mouseInterface = true;
        }

        public UIPanel MainPanel { get; set; }

        public StandCard Card { get; set; }

        public UIGrid ComboGrid { get; set; }

        public bool Visible { get; set; }
    }
}
