using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;
using System.Text;
using TBAR.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Input
{
    public static class TBARInputs
    {
        public static void Load(Mod mod)
        {
            SummonStand = mod.RegisterHotKey("Summon stand", "Z");
            ComboButton1 = mod.RegisterHotKey("Combo Button 1", "X");
            ComboButton2 = mod.RegisterHotKey("Combo Button 2", "C");
            ComboButton3 = mod.RegisterHotKey("Combo Button 3", "V");
        }

        public static void Unload()
        {
            SummonStand = null;
            ComboButton1 = null;
            ComboButton2 = null;
            ComboButton3 = null;
        }

        public static ModHotKey SummonStand { get; set; }

        public static ModHotKey ComboButton1 { get; set; }
        public static ModHotKey ComboButton2 { get; set; }
        public static ModHotKey ComboButton3 { get; set; }
    }
}
