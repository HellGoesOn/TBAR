using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using TBAR.TimeStop;
using TBAR.UI;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace TBAR
{
	public partial class TBAR : Mod
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
                TextureLoader.Load();

                Ref<Effect> screenRef = new Ref<Effect>(GetEffect("Effects/ShockwaveEffect")); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();
            }


            On.Terraria.Main.Update += Main_Update;

            StandFactory.Instance.Load();
            TBARInputs.Load(this);
        }

        private void Main_Update(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            orig.Invoke(self, gameTime);
            TimeStopManager.Instance.Update();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UIManager.Instance.Update(gameTime);
        }

        public override void Unload()
        {
            base.Unload();

            StandFactory.Unload();
            TextureLoader.Unload();
            UIManager.Unload();
            TimeStopManager.Unload();

            TBARInputs.Unload();

            Instance = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(UIManager.Instance.TimeSkipLayer);
        }

        public void PlayVoiceLine(string SoundPath)
        {
            if (!Instance.VoiceLinesEnabled)
                return;

            Main.PlaySound(Instance.GetLegacySoundSlot(SoundType.Custom, SoundPath));
        }

        public void PlaySound(string SoundPath)
        {
            Main.PlaySound(Instance.GetLegacySoundSlot(SoundType.Custom, SoundPath));
        }

        public bool VoiceLinesEnabled { get; set; } = true;
    }
}