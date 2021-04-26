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

                SkyManager.Instance["TBA:TimeStopInvert"] = new PerfectlyNormalSky();
                Filters.Scene["TBA:TimeStopInvert"] = new Filter(new ScreenShaderData("FilterInvert"), EffectPriority.High);

                Filters.Scene["TBA:FreezeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(.7f, .7f, .7f), EffectPriority.VeryHigh);
                SkyManager.Instance["TBA:FreezeSky"] = new FreezeSky();
                AddEquipTexture(null, EquipType.Head, "DiavoloHead", "TBAR/Items/Vanity/VinegarDisguise/DiavoloHead_Head");
                AddEquipTexture(null, EquipType.Body, "DiavoloBody", "TBAR/Items/Vanity/VinegarDisguise/DiavoloChest_Body", "TBAR/Items/Vanity/VinegarDisguise/DiavoloChest_Arms");
            }

            OnEdits.Instance.LoadEdits();

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

            StandFactory.Unload();
            TextureLoader.Unload();
            UIManager.Unload();
            TimeStopManager.Unload();

            TBARInputs.Unload();
            OnEdits.Instance.UnloadEdits();

            OnEdits.EndLife();

            Instance = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(UIManager.Instance.TimeSkipLayer);
            layers.Insert(0, UIManager.Instance.StandAlbumLayer);
        }

        public void PlayVoiceLine(string SoundPath)
        {
            if (!Instance.VoiceLinesEnabled || SoundPath == "")
                return;

            Main.PlaySound(Instance.GetLegacySoundSlot(SoundType.Custom, SoundPath));
        }

        public void PlaySound(string SoundPath)
        {
            if (SoundPath == "")
                return;

            Main.PlaySound(Instance.GetLegacySoundSlot(SoundType.Custom, SoundPath));
        }

        public bool VoiceLinesEnabled { get; set; } = true;
    }
}