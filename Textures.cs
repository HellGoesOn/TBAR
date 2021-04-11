using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TBAR
{
    // Using Singleton instead of static class here so it never exists on the server
    public class Textures
    {
        public void Load()
        {
            KeyboardInput = LoadTexture("KeyboardInput");
            Emptiness = LoadTexture("EmptyPixel");
        }

        public void Unload()
        {
            KeyboardInput = null;
            Emptiness = null;
        }

        public Texture2D KeyboardInput { get; private set; }

        public Texture2D Emptiness { get; private set; }

        private Texture2D LoadTexture(string name)
        {
            return ModContent.GetTexture("TBAR/Textures/" + name);
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
