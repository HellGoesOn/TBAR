using System;
using Terraria;

namespace TBAR.Components
{
    public interface IGlobalEffect
    {
        void Update();

        Entity Owner();
    }
}
