using TBAR.NPCs;
using TBAR.TimeStop;
using Terraria;

namespace TBAR
{
    public class OnEdits
    {
        private static OnEdits _instance;
        public static OnEdits Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new OnEdits();

                return _instance;
            }
        }


        public void LoadEdits()
        {
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            On.Terraria.Main.DrawBlack += Main_DrawBlack;

            On.Terraria.Main.DrawWalls += Main_DrawWalls;

            On.Terraria.NPC.VanillaAI += NPC_VanillaAI;

            On.Terraria.NPC.UpdateNPC += NPC_UpdateNPC;

            On.Terraria.NPC.VanillaHitEffect += NPC_VanillaHitEffect;

            On.Terraria.Dust.UpdateDust += Dust_UpdateDust;

            On.Terraria.Dust.NewDust += Dust_NewDust;

            On.Terraria.Gore.Update += Gore_Update;

            On.Terraria.Projectile.Update += Projectile_Update;

            On.Terraria.Liquid.UpdateLiquid += Liquid_UpdateLiquid;
        }

        public void UnloadEdits()
        {
            On.Terraria.Main.DrawTiles -= Main_DrawTiles;
            On.Terraria.Main.DrawBlack -= Main_DrawBlack;
            On.Terraria.Main.DrawWalls -= Main_DrawWalls;

            On.Terraria.NPC.VanillaAI -= NPC_VanillaAI;

            On.Terraria.NPC.UpdateNPC -= NPC_UpdateNPC;

            On.Terraria.NPC.VanillaHitEffect -= NPC_VanillaHitEffect;

            On.Terraria.Dust.UpdateDust -= Dust_UpdateDust;

            On.Terraria.Dust.NewDust -= Dust_NewDust;

            On.Terraria.Gore.Update -= Gore_Update;

            On.Terraria.Projectile.Update -= Projectile_Update;

            On.Terraria.Liquid.UpdateLiquid -= Liquid_UpdateLiquid;
        }

        public static void EndLife() => _instance = null;

        private void Liquid_UpdateLiquid(On.Terraria.Liquid.orig_UpdateLiquid orig)
        {
            if (!TimeStopped)
                orig.Invoke();
        }

        private void Projectile_Update(On.Terraria.Projectile.orig_Update orig, Projectile self, int i)
        {
            bool ownerStoppedTime = TBAR.TimeStopManager.HaveITimeStopped(Main.player[self.owner]);

            if (!TimeStopped || ownerStoppedTime)
                orig.Invoke(self, i);
        }

        private void Gore_Update(On.Terraria.Gore.orig_Update orig, Gore self)
        {
            if (!TimeStopped)
                orig.Invoke(self);
        }


        private int Dust_NewDust(On.Terraria.Dust.orig_NewDust orig, Microsoft.Xna.Framework.Vector2 Position, int Width, int Height, int Type, float SpeedX, float SpeedY, int Alpha, Microsoft.Xna.Framework.Color newColor, float Scale)
        {
            if (!TimeStopped)
                return orig.Invoke(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor, Scale);

            return 0;
        }

        private void Dust_UpdateDust(On.Terraria.Dust.orig_UpdateDust orig)
        {
            if (!TimeStopped)
                orig.Invoke();
        }

        private void NPC_VanillaHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            if (!TimeStopped || TimeStopStockOwner(self))
                orig.Invoke(self, hitDirection, dmg);
            else
            {
                TBARGlobalNPC.Get(self).AccumulatedDamage += (int)dmg;
                self.life += (int)dmg;
            }
        }

        private void NPC_VanillaAI(On.Terraria.NPC.orig_VanillaAI orig, NPC self)
        {
            if (!TimeStopped || TimeStopStockOwner(self))
                orig.Invoke(self);
        }

        private void Main_DrawWalls(On.Terraria.Main.orig_DrawWalls orig, Main self)
        {
            if (!TBAR.Instance.DisableTileDraw)
                orig.Invoke(self);
        }

        private void Main_DrawBlack(On.Terraria.Main.orig_DrawBlack orig, Main self, bool force)
        {
            if (!TBAR.Instance.DisableTileDraw)
                orig.Invoke(self, force);
        }

        private void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
        {
            if (!TBAR.Instance.DisableTileDraw)
                orig.Invoke(self, solidOnly, waterStyleOverride);
        }

        private void NPC_UpdateNPC(On.Terraria.NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (!TimeStopped || TimeStopStockOwner(self))
                orig.Invoke(self, i);
            else
            {
                for (int k = 0; k < self.immune.Length; k++)
                {
                    if (self.immune[k] > 0)
                        self.immune[k]--;
                }
            }
        }

        public bool TimeStopStockOwner(Entity e) => TBAR.TimeStopManager.HaveITimeStopped(e);

        public bool TimeStopped => TBAR.TimeStopManager.IsTimeStopped;
    }
}
