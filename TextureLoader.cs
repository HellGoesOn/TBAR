﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TBAR.Attributes;
using Terraria.ModLoader;

namespace TBAR
{
    public static class TextureLoader
    {
        public static void Load()
        {
            foreach (PropertyInfo property in Textures.Instance.GetType().GetProperties())
            {
                LoadableAttribute attr = (LoadableAttribute)property.GetCustomAttribute(typeof(LoadableAttribute));

                if (attr == null)
                    continue;

                property.SetValue(Textures.Instance, ModContent.GetTexture(attr.Path));
            }
        }

        public static void Unload()
        {
            foreach (PropertyInfo property in Textures.Instance.GetType().GetProperties())
            {
                LoadableAttribute attr = (LoadableAttribute)property.GetCustomAttribute(typeof(LoadableAttribute));

                if (attr == null)
                    continue;

                property.SetValue(Textures.Instance, null);
            }

            Textures.Unload();
        }
    }
}