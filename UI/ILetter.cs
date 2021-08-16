using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TBAR.UI
{
    interface ILetter
    {
        void Draw(SpriteBatch sb);

        void Update(GameTime gameTime);
    }
}
