using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class StandAlbumLayer : GameInterfaceLayer
    {
        public StandAlbumLayer() : base("TBAR:Stand Album Layer", InterfaceScaleType.UI)
        {
            State = new StandAlbumUIState();
            State.Activate();
            Interface = new UserInterface();
            Interface.SetState(State);
        }

        public void Update(GameTime gameTime)
        {
            Interface.Update(gameTime); 
            State.Update(gameTime);
        }

        public void ToggleVisibility()
        {
            State.Visible = !State.Visible;

            if(State.Visible)
                State.OnOpen();
        }

        protected override bool DrawSelf()
        {
            if (State == null)
                return false;

            if (State.Visible)
                State.Draw(Main.spriteBatch);

            return true;
        }

        public UserInterface Interface { get; }

        public StandAlbumUIState State { get; }
    }
}
