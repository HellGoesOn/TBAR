using System.Reflection;
using TBAR.Attributes;
using Terraria.ModLoader;

namespace TBAR
{
    public static class TextureLoader
    {
        public static void Load()
        {
            foreach (PropertyInfo property in typeof(Textures).GetProperties())
            {
                LoadableAttribute attr = (LoadableAttribute)property.GetCustomAttribute(typeof(LoadableAttribute));

                if (attr == null)
                    continue;

                property.SetValue(null, ModContent.GetTexture(attr.Path));
            }
        }

        public static void Unload()
        {
            foreach (PropertyInfo property in typeof(Textures).GetProperties())
            {
                LoadableAttribute attr = (LoadableAttribute)property.GetCustomAttribute(typeof(LoadableAttribute));

                if (attr == null)
                    continue;

                property.SetValue(null, null);
            }
        }
    }
}
