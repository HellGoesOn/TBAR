using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Input;
using TBAR.Stands;
using TBAR.UI;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace TBAR
{
	public class TBAR : Mod
	{
        public static TBAR Instance { get; private set; }

        public TBAR()
        {
            Instance = this;
        }

        public override void Load()
        {
            base.Load();

            if(!Main.dedServ)
            {
                UIManager.Instance.Initialize();
                Textures.Instance.Load();

                Ref<Effect> screenRef = new Ref<Effect>(GetEffect("Effects/ShockwaveEffect")); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();
            }

            StandFactory.Instance.Load();
            TBARInputs.Load(this);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UIManager.Instance.Update(gameTime);
        }

        public override void Unload()
        {
            base.Unload();

            StandFactory.Instance.Unload();
            Textures.Instance.Unload();

            TBARInputs.Unload();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(UIManager.Instance.TimeSkipLayer);
        }
    }
}