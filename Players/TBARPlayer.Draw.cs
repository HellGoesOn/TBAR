using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TBAR.Items.Tools;
using TBAR.PlayerLayers;
using TBAR.ScreenModifiers;
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
                        ScreenModifiers.Add(new ShakeScreenModifier(player.Center, 120, 3, 0.25f));
                        BeganPiercing = true;
                    }
                }
                else
                {
                    if (ArrowXOffset < 12f)
                        ArrowXOffset += 1.2f;
                }
            }
        }

        public bool BeganPiercing { get; set; }

        public float ArrowXOffset { get; set; }

        public int ArrowProgress { get; set; }

        public bool IsUsingArrow => player.HeldItem.type == ModContent.ItemType<StandArrow>() && player.itemAnimation > 0;

        public int ArrowProgressMax { get; } = 80;
    }
}
