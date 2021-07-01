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
        
        [Loadable("TBAR/Textures/DMGLogo")]
        public static Texture2D DamageTypeIcon { get; internal set; }

        [Loadable("TBAR/Textures/Range")]
        public static Texture2D RangeIcon { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/StandCard")]
        public static Texture2D StandCard { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/Current")]
        public static Texture2D StandCardCurrent { get; internal set; }

        [Loadable("TBAR/Items/Tools/StandArrow")]
        public static Texture2D StandArrow { get; internal set; }

        [Loadable("TBAR/Textures/Runner")]
        public static Texture2D Runner { get; internal set; }
        
        [Loadable("TBAR/Textures/RunnerBounds")]
        public static Texture2D RunnerBounds { get; internal set; }

        [Loadable("TBAR/Textures/Tooth")]
        public static Texture2D Tooth { get; internal set; }

        [Loadable("TBAR/Textures/Space")]
        public static Texture2D FancySky { get; internal set; }

        public static string EmptinessPath => "TBAR/Textures/EmptyPixel";
    }
}
