using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands
{
    public class PunchBarrage : ModProjectile
    {
        public static Projectile CreateBarrage(string texturePath, Projectile parent, Vector2 destination, int damage, string altTexturePath = "")
        {
            int proj = Projectile.NewProjectile(parent.Center, destination, ModContent.ProjectileType<PunchBarrage>(), damage, 0, parent.owner);
            PunchBarrage barrage = (PunchBarrage)Main.projectile[proj].modProjectile;

            barrage.TexturePath = texturePath;
            barrage.AltTexturePath = altTexturePath;
            barrage.Parent = parent;

            return Main.projectile[proj];
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 60;
            projectile.penetrate = -1;
            projectile.timeLeft = 190;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.netUpdate = true;

            Punches = new List<FakePunchData>();
        }

        public override void AI()
        {
            if(projectile.timeLeft > 10)
            {
                LastPosition = Parent.Center;
                Main.player[projectile.owner].heldProj = projectile.whoAmI;

                if(Punches.Count < 10)
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            foreach (FakePunchData data in Punches)
                data.Draw(spriteBatch, projectile.Center, projectile.velocity);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Parent.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Parent = Main.projectile[reader.ReadInt32()];
        }

        public List<FakePunchData> Punches { get; private set; }

        public string TexturePath { get; private set; }

        public string AltTexturePath { get; private set; }

        public override string Texture => "TBAR/Textures/EmptyPixel";

        public Projectile Parent { get; set; }

        public Vector2 LastPosition { get; set; }
    }
}
