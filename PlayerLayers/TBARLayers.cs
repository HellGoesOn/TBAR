using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TBAR.Helpers;
using TBAR.Players;
using TBAR.Players.Visuals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TBAR.PlayerLayers
{
    public static class TBARLayers
    {
        public static readonly PlayerLayer StandArrowUseLayer = new PlayerLayer("TBAR", "StandArrowUse", delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0)
                return;

            Texture2D texture = Textures.StandArrow;
            Player drawPlayer = drawInfo.drawPlayer;
            TBARPlayer tbr = TBARPlayer.Get(drawPlayer);

            int dir_m = drawPlayer.direction; // Direction multiplier

            Vector2 texCenter = new Vector2(texture.Width, texture.Height) * 0.5f;

            float progress = (tbr.ArrowProgress / (float)tbr.ArrowProgressMax) - 0.36f;

            float angle = 0.12f;

            angle = -MathHelper.Lerp(angle, MathHelper.Pi + 0.6f, progress) + MathHelper.PiOver2;

            float finalizedAngle = angle * dir_m;

            Vector2 temp = new Vector2(-24.0f, -12 * (0.76f - progress) * dir_m);

            Vector2 arrowPosition = temp.RotatedBy(finalizedAngle) * dir_m;

            DrawData drawData = new DrawData
            (
                texture,
                drawPlayer.Center + arrowPosition + new Vector2(-tbr.ArrowXOffset * dir_m, drawPlayer.gfxOffY) - Main.screenPosition,
                null,
                Color.White,
                finalizedAngle + MathHelper.PiOver4 + (dir_m == -1 ? MathHelper.Pi : 0) - (progress * dir_m) * 1.12f,
                texCenter,
                0.5f,
                SpriteEffects.None,
                1
                );

            int beamWidth = 20;
            int beamHeight = 1200;

            int p_beamWidth = (int)(beamWidth * (1 - progress));
            int p_beamHeight = (int)(beamHeight * (1 - progress));

            Main.playerDrawData.Add(drawData);
        }
        );

        public static readonly PlayerLayer BeamLayer = new PlayerLayer("TBAR", "Beam", delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            TBARPlayer tbr = TBARPlayer.Get(drawPlayer);

            foreach (BeamVisual b in tbr.BeamVisuals)
            {
                float progress = b.OpenTime / (float)(b.OpenTimeMax);

                int beamWidth = b.Width;
                int beamHeight = b.Height;

                int p_beamWidth = (int)(beamWidth * (1 - progress));
                int p_beamHeight = (int)(beamHeight * (1 - progress));

                Color b_color = b.BeamColor;

                DrawHelper.DrawRectangle(drawPlayer.Center - new Vector2(p_beamWidth * 1.5f * 0.5f, -(drawPlayer.height * 0.5f - beamHeight)) - Main.screenPosition, (int)(p_beamWidth * 1.5f), beamHeight, b_color * 0.25f, Main.spriteBatch);
                DrawHelper.DrawRectangle(drawPlayer.Center - new Vector2(p_beamWidth * 0.5f, -(drawPlayer.height * 0.5f - beamHeight)) - Main.screenPosition, p_beamWidth, beamHeight, b_color * 0.45f, Main.spriteBatch);
                DrawHelper.DrawRectangle(drawPlayer.Center - new Vector2(p_beamWidth * .5f * 0.5f, -(drawPlayer.height * 0.5f - beamHeight)) - Main.screenPosition, (int)(p_beamWidth * .5f), beamHeight, Color.White * 0.75f, Main.spriteBatch);
            }
        });
    }
}
