using Microsoft.Xna.Framework.Graphics;
using TBAR.Attributes;

namespace TBAR
{
    public static class Textures
    {
        [Loadable("TBAR/Textures/KeyboardInput")]
        public static Texture2D KeyboardInput { get; internal set; }

        [Loadable("TBAR/Textures/EmptyPixel")]
        public static Texture2D Emptiness { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/StandCard")]
        public static Texture2D StandCard { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/Current")]
        public static Texture2D StandCardCurrent { get; internal set; }

        [Loadable("TBAR/Items/Tools/StandArrow")]
        public static Texture2D StandArrow { get; internal set; }
    }
}
