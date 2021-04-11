using Microsoft.Xna.Framework.Graphics;

namespace TBAR.UI.ScreenEffects
{
    public class UIVisualEffect
    {
        public virtual void DrawEffect(SpriteBatch spriteBatch)
        {
            /// TO-DO: Implement default behaviour
        }

        public virtual void Update() { }

        public bool Active { get; set; } = true;
    }
}
