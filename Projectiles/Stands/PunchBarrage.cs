using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands
{
    public class PunchBarrage : ModProjectile
    {
        public static int CreateBarrage(string texturePath, Projectile parent, Vector2 destination, int damage, string altTexturePath = "")
        {
            int proj = Projectile.NewProjectile(parent.Center, destination, ModContent.ProjectileType<PunchBarrage>(), damage, 0, Main.myPlayer, parent.whoAmI);

            PunchBarrage barrage = (PunchBarrage)Main.projectile[proj].modProjectile;
            barrage.TexturePath = texturePath;
            barrage.AltTexturePath = altTexturePath;

            return proj;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 60;
            projectile.penetrate = -1;
            projectile.timeLeft = 190;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.netUpdate = true;
            projectile.netUpdate2 = true;

            Punches = new List<FakePunchData>();
        }

        public override void AI()
        {
            if (projectile.timeLeft > 10)
            {
                LastPosition = Parent.Center;
                Main.player[projectile.owner].heldProj = projectile.whoAmI;

                if(Punches.Count < 10 && TexturePath != null)
                    Punches.Add(new FakePunchData(TexturePath, AltTexturePath));
            }
            else
                projectile.damage = 0;

            projectile.Center = LastPosition + projectile.velocity;

            for (int i = Punches.Count - 1; i >= 0; i--)
            {
                Punches[i].Update();

                if (!Punches[i].Active)
                    Punches.RemoveAt(i);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            if (TexturePath != null)
                writer.Write(TexturePath);

            if (AltTexturePath != null)
                writer.Write(AltTexturePath);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            string piss = reader.ReadString();
            string shit = reader.ReadString();

            if (piss != null)
                TexturePath = piss;

            if(shit != null)
                AltTexturePath = shit;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            foreach (FakePunchData data in Punches)
                data.Draw(spriteBatch, projectile.Center, projectile.velocity);
        }

        public List<FakePunchData> Punches { get; private set; }

        public string TexturePath { get; private set; }

        public string AltTexturePath { get; private set; }

        public override string Texture => "TBAR/Textures/EmptyPixel";

        public Projectile Parent => Main.projectile[(int)projectile.ai[0]];

        public Vector2 LastPosition { get; set; }
    }
}
