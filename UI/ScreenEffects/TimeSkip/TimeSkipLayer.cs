using Terraria;
using Terraria.UI;

namespace TBAR.UI.ScreenEffects.TimeSkip
{
    public class TimeSkipLayer : GameInterfaceLayer
    {
        public TimeSkipLayer() : base("TBAR:Time Skip Layer", InterfaceScaleType.None)
        {
            State = new UITimeSkipVisuals();
            State.Activate();
        }

        protected override bool DrawSelf()
        {
            if (State == null)
                return false;

            if (State.Visible)
                State.Draw(Main.spriteBatch);

            return true;
        }

        public UITimeSkipVisuals State { get; }
    }
}
