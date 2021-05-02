using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TBAR.Helpers;
using TBAR.Items.Tools;
using TBAR.PlayerLayers;
using TBAR.ScreenModifiers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public const int BODY_HANDS_UP = 56;
        public const int BODY_HANDS_MID = 2 * 56;
        public const int BODY_HANDS_DOWN = 3 * 56;

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int armIndex = layers.FindIndex(x => x.Name.Equals("Arms"));

            if (IsUsingArrow)
            {
                if (ArrowProgress > 0)
                    layers.Insert(armIndex, ArrowUseLayer.StandArrowUseLayer);
                else
                    layers.Insert(0, ArrowUseLayer.StandArrowUseLayer);

                if (ArrowProgress >= ArrowProgressMax * 0.65f)
                    player.bodyFrame.Y = TBARPlayer.BODY_HANDS_UP;
                else if (ArrowProgress >= ArrowProgressMax * 0.2f)
                    player.bodyFrame.Y = TBARPlayer.BODY_HANDS_MID;
                else
                    player.bodyFrame.Y = TBARPlayer.BODY_HANDS_DOWN;
            }
            else
            {
                UsePosition = Vector2.Zero;
                ArrowProgress = 0;
                ArrowXOffset = 0;
                BeganPiercing = false;
            }
        }

        public void UpdateArrowUseProgress()
        {
            if (ArrowProgress > 0)
                ArrowProgress--;

            if (ArrowProgress == 0 && IsUsingArrow)
            {
                if (!BeganPiercing)
                {
                    ArrowXOffset -= 0.6f;

                    if (ArrowXOffset <= -6f)
                    {
                        ScreenModifiers.Add(new SmoothStepScreenModifier(player.Center, player.Center - new Vector2(0, 64), 0.025f));
                        ScreenModifiers.Add(new ShakeScreenModifier(player.Center - new Vector2(0, 64), 120, 3, 0.25f));
                        ScreenModifiers.Add(new SmoothStepScreenModifier(player.Center - new Vector2(0, 64), player.Center, 0.025f));
                        BeganPiercing = true;
                    }
                }
                else
                {
                    Lighting.AddLight(player.Center, Color.PaleGoldenrod.ToVector3() * 2f);
                    for (int i = 0; i < 3; i++)
                    {
                        var dust = Dust.NewDust(UsePosition + new Vector2(-player.width, 0), 40, 0, DustID.AmberBolt, 0, -Main.rand.Next(10, 100), 0, default, 1.1f);
                        Main.dust[dust].noGravity = true;
                        var dust2 = Dust.NewDust(UsePosition + new Vector2(-player.width, 0), 40, 0, DustID.AmberBolt, Main.rand.Next(-3, 4), -Main.rand.Next(2, 7), 0, default, 1.1f);
                        Main.dust[dust2].noGravity = true;
                    }

                    if (ArrowXOffset < 12f)
                        ArrowXOffset += 1.2f;
                }
            }
        }

        public Vector2 UsePosition { get; set; }

        public bool BeganPiercing { get; set; }

        public float ArrowXOffset { get; set; }

        public int ArrowProgress { get; set; }

        public bool IsUsingArrow => player.HeldItem.type == ModContent.ItemType<StandArrow>() && player.itemAnimation > 0;

        public int ArrowProgressMax { get; } = 80;
    }
}
