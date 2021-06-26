using Microsoft.Xna.Framework;
using TBAR.Players;
using Terraria;
using Terraria.UI;

namespace TBAR.UI
{
    public class ResourceLayer : GameInterfaceLayer
    {
        public ResourceLayer() : base("TBAR:Resource Layer", InterfaceScaleType.UI)
        {
            State = new ResourceState();
            State.Activate();
            Interface = new UserInterface();
            Interface.SetState(State);
        }

        public void Update(GameTime gameTime)
        {
            Interface.Update(gameTime); 
            State.Update(gameTime);
        }

        protected override bool DrawSelf()
        {
            if (State == null)
                return false;

            State.Draw(Main.spriteBatch);

            return true;
        }

        public UserInterface Interface { get; }

        public ResourceState State { get; }
    }
}
