using Microsoft.Xna.Framework;
using TBAR.Helpers;
using TBAR.Input;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TBAR.UI.Elements.ComboPanel
{
    public class ComboPanel : UIPanel
    {
        public const float WIDTH = 420, CLOSED_HEIGHT = 120, OPEN_HEIGHT = 300;

        public ComboPanel(StandCombo combo)
        {
            this.BackgroundColor = new Color(new Vector3(0.4f)) * 0.5f;
            Name = new UIText(combo.ComboName, 1f);
            Name.VAlign = Name.HAlign = 0.5f;

            DescriptionPanel = new UIPanel();
            DescriptionPanel.Width.Set(WIDTH, 0);
            DescriptionPanel.Height.Set(OPEN_HEIGHT / 2 + 20, 0);
            DescriptionPanel.HAlign = 0.5f;
            DescriptionPanel.VAlign = 1f;
            DescriptionPanel.BackgroundColor = new Color(new Vector3(0.4f)) * 0.5f;

            UIText desc = new UIText("This combo lacks description.", 0.85f)
            {
                HAlign = 0.5f,
                VAlign = 0.05f
            };

            if (combo.Description != null)
                desc.SetText(combo.Description.SpliceText(40));

            DescriptionPanel.Append(desc);
            DescriptionPanel.RecalculateChildren();

            Width.Set(WIDTH, 0);
            Height.Set(CLOSED_HEIGHT, 0);

            UIPanel namePanel = new UIPanel();
            namePanel.Width.Set(WIDTH, 0);
            namePanel.Height.Set(CLOSED_HEIGHT * 0.25f, 0);
            namePanel.BackgroundColor = new Color(new Vector3(0.4f)) * 0.5f;

            namePanel.Append(Name);

            OnClick += new MouseEvent(Expand);

            for(int i = 0; i < combo.RequiredInputs.Count; i++)
            {
                ComboInput c = combo.RequiredInputs[i];
                InputButton button = new InputButton(c);

                button.Top.Set(CLOSED_HEIGHT * 0.33f, 0);
                button.Left.Set(2 + ((button.Width.Pixels + 4) * i), 0);

                this.Append(button);
            }

            this.Append(namePanel);
        }

        private void Expand(UIMouseEvent evt, UIElement listeningElement)
        {
            OnClick -= new MouseEvent(Expand);
            OnClick += new MouseEvent(Close);

            Height.Set(OPEN_HEIGHT, 0);
            this.Append(DescriptionPanel);

            Recalculate();
        }

        private void Close(UIMouseEvent evt, UIElement listeningElement)
        {
            OnClick -= new MouseEvent(Close);
            OnClick += new MouseEvent(Expand);

            Height.Set(CLOSED_HEIGHT, 0);
            this.RemoveChild(DescriptionPanel);
            Recalculate();
        }

        public UIPanel DescriptionPanel { get; set; }

        public UIText Name { get; set; }

        public bool Unrolled { get; set; }
    }
}
