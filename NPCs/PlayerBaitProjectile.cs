using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.NPCs
{
    public class PlayerBaitNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MISSINGNO.");
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 20;
            npc.width = npc.height = 20;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.dontTakeDamage = true;
            npc.dontTakeDamageFromHostiles = true;
        }

        public override void AI()
        {
            if(!TBAR.TimeSkipManager.IsTimeSkipped)
            {
                npc.life = 0;
                npc.checkDead();
            }
        }

        public override string Texture => Textures.EmptinessPath;
    }

    public class PlayerBaitProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MISSINGNO.");
        }

        public override void SetDefaults()
        {
            projectile.tileCollide = false;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Owner.tankPetReset = false;
            Owner.tankPet = projectile.whoAmI;
        }

        public override void Kill(int timeLeft)
        {
            for(int i = 0; i < Main.maxNPCs; i++)
            {
                Main.npc[i].AddBuff(BuffID.Confused, Global.SecondsToTicks(4));
            }
        }

        public override string Texture => Textures.EmptinessPath;

        public Player Owner => Main.player[projectile.owner];
    }
}
