using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public int StyleDecayTimer { get; set; }

        private int _stylePoints;
        public int StylePoints
        {
            get => _stylePoints;
            private set => _stylePoints = (int)MathHelper.Clamp(value, 0, 66666);
        }

        public string LastUsedCombo { get; set; }

        public int StyleHitCounter { get; set; }
        public int StyleHitCounterResetTimer { get; set; }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            StylePoints -= (short)(250 + (500 * StyleHitCounter));
            StyleHitCounterResetTimer = 180;
            StyleHitCounter++;

            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
        }

        public int PoolID { get; private set; }
        public ushort _repeatCount;
        public ushort RepeatCount
        {
            get => _repeatCount;
            set
            {
                PoolID = Main.rand.Next(3);

                _repeatCount = (ushort)MathHelper.Clamp(value, 0, 2);
            }
        }

        public int RepeatCountResetTimer { get; set; }

        public void ResetRepeatCount()
        {
            if (RepeatCountResetTimer > 0)
                RepeatCountResetTimer--;
            else
            {
                RepeatCount = 0;
                LastUsedCombo = "";
            }
        }

        public float RepeatModifier
        {
            get
            {
                if (RepeatCount < 1)
                    return 1f;

                if (RepeatCount < 2)
                    return 0.25f;

                return 0.0f;
            }
        }

        public StyleRank CurrentStyleRank
        {
            set
            {
                switch (value)
                {
                    case StyleRank.D:
                        StylePoints = 1999;
                        break;
                    case StyleRank.C:
                        StylePoints = 2000;
                        break;
                    case StyleRank.B:
                        StylePoints = 4000;
                        break;
                    case StyleRank.A:
                        StylePoints = 8000;
                        break;
                    case StyleRank.S:
                        StylePoints = 12000;
                        break;
                    case StyleRank.SS:
                        StylePoints = 14000;
                        break;
                    default:
                        StylePoints = 18000;
                        break;
                }

                StyleDecayTimer = 600;
            }
            get
            {
                if (StylePoints < 2000)
                    return StyleRank.D;

                if (StylePoints < 4000)
                    return StyleRank.C;
				
				if(StylePoints < 8000)
					return StyleRank.B;

                if (StylePoints < 12000)
                    return StyleRank.A;

                if (StylePoints < 14000)
                    return StyleRank.S;

                if (StylePoints < 18000)
                    return StyleRank.SS;

                return StyleRank.SSS;
            }
        }

        public void AddStylePoints(int value)
        {

            var resultValue = (int)(value * RepeatModifier);

            if(resultValue > 0)
                StyleDecayTimer = 600;

            StylePoints += resultValue;
        }
    }

    public enum StyleRank
    {
        D,
        C,
        B,
        A,
        S,
        SS,
        SSS,
    }
}
