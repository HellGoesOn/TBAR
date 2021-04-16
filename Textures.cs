﻿using Microsoft.Xna.Framework.Graphics;
using TBAR.Attributes;

namespace TBAR
{
    public class Textures
    {
        [Loadable("TBAR/Textures/KeyboardInput")]
        public Texture2D KeyboardInput { get; internal set; }

        [Loadable("TBAR/Textures/EmptyPixel")]
        public Texture2D Emptiness { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/StandCard")]
        public Texture2D StandCard { get; internal set; }

        [Loadable("TBAR/UI/Elements/StandCard/Current")]
        public Texture2D StandCardCurrent { get; internal set; }


        public static void Unload()
        {
            instance = null;
        }

        private static Textures instance;
        public static Textures Instance
        {
            get
            {
                if (instance == null)
                    instance = new Textures();

                return instance;
            }
        }

        private Textures() { }

    }
}
