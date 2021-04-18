using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Visual
{
    public class TimeStopVFX : ModProjectile
    {
        private const int
               RIPPLE_COUNT = 1,
               RIPPLE_SIZE = 1;

        private const float
            RIPPLE_SPEED = 2.75f,
            DISTORT_STRENGTH = 75000f;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.light = 0.9f;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.friendly = true;
            projectile.tileCollide = false;
            aiType = 24;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;

            projectile.Center = Owner.Center;

            if (!Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(RIPPLE_COUNT, RIPPLE_SIZE, RIPPLE_SPEED).UseTargetPosition(projectile.Center);
            }

            float progress = (240f - projectile.timeLeft) / 60f;

            Filters.Scene["Shockwave"].GetShader().UseTargetPosition(projectile.Center);
            Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(DISTORT_STRENGTH * (progress / 3f));
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            Filters.Scene["Shockwave"].Deactivate();
        }

        public Player Owner => Main.player[projectile.owner];

        public bool IsNativelyImmuneToTimeStop() => true;

        public override string Texture => "TBAR/Textures/EmptyPixel";
    }
}
