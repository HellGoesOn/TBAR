using Terraria.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.UI
{
    public abstract class UIDraggableElement : UIElement
    {
        private bool isDragging;

        private Vector2 dragOffset;

        public override void MouseDown(UIMouseEvent evt)
        {
            isDragging = true;
            dragOffset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            var end = evt.MousePosition;

            isDragging = false;

            Left.Set(end.X - dragOffset.X, 0);
            Top.Set(end.Y - dragOffset.Y, 0);
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isDragging)
            {
                if (!Main.mouseLeft)
                    isDragging = false;

                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Recalculate();
            }

            Left.Pixels = Utils.Clamp(Left.Pixels, 0, Main.screenWidth - Width.Pixels);
            Top.Pixels = Utils.Clamp(Top.Pixels, 0, Main.screenHeight - Height.Pixels);

            Recalculate();
        }
    }
}
