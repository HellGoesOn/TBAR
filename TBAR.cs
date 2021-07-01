using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TBAR.Components;
using TBAR.Helpers;
using TBAR.Input;
using TBAR.Stands;
using TBAR.TimeSkip;
using TBAR.TimeStop;
using TBAR.UI;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace TBAR
{
    public partial class TBAR : Mod
	{
#if DEBUG
        internal static bool DebugBuild = true;
#else
    internal static bool DebugBuild = false;
#endif
        public static TBAR Instance { get; private set; }

        public TBAR()
        {
            Tracks = new List<TBARMusic>();
            TimeStopManager = new TimeStopManager();
            TimeSkipManager = new TimeSkipManager();
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

            SteamHelper.Initialize();

            OnEdits.Instance.LoadEdits();

            StandLoader.Instance.Load();
            TBARInputs.Load(this);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UIManager.Instance.Update(gameTime);
        }

        public override void Unload()
        {
            base.Unload();

            StandLoader.Unload();
            TextureLoader.Unload();
            UIManager.Unload();

            TBARInputs.Unload();
            OnEdits.Instance.UnloadEdits();

            TimeStopManager = null;
            TimeSkipManager = null;

            OnEdits.EndLife();

            Instance = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(UIManager.Instance.TimeSkipLayer);
            layers.Insert(0, UIManager.Instance.StandAlbumLayer);
            layers.Insert(0, UIManager.Instance.ResourceLayer);
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

        public bool DisableTileDraw => TimeSkipManager.EffectCount > 0;
    }
}