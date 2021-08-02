using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private double _stamina;
        public double Stamina
        {
            get => _stamina;
            set
            {
                _stamina = (double)MathHelper.Clamp((float)value, 0, 1000);
            }
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

        public readonly Dictionary<StyleRank, double> StaminaGainModifiers = new Dictionary<StyleRank, double>();

        public void InitializeStaminaModifiers()
        {
            StaminaGainModifiers.Add(StyleRank.D, 0.1f);
            StaminaGainModifiers.Add(StyleRank.C, 0.5f);
            StaminaGainModifiers.Add(StyleRank.B, 1f);
            StaminaGainModifiers.Add(StyleRank.A, 1.5f);
            StaminaGainModifiers.Add(StyleRank.S, 2f);
            StaminaGainModifiers.Add(StyleRank.SS, 4f);
            StaminaGainModifiers.Add(StyleRank.SSS, 8f);
        }

        public StyleRank CurrentStyleRank
        {
            get
            {
                if (StylePoints < 2000)
                    return StyleRank.D;

                if (StylePoints < 4000)
                    return StyleRank.C;

                if (StylePoints < 8000)
                    return StyleRank.A;

                if (StylePoints < 12000)
                    return StyleRank.S;

                if (StylePoints < 16000)
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

        public bool CheckStaminaCost(double cost)
        {
            if (Stamina - cost >= 0)
            {
                Stamina -= cost;
                return true;
            }
            return false;
        }

        public void AddStamina(double value = 1.0)
        {
            Stamina += (double)(value * StaminaGainModifiers[CurrentStyleRank]);
            Main.NewText("Curretn Stamina: " + Stamina);
        }
    }

    public enum StyleRank
    {
        D,
        B,
        C,
        A,
        S,
        SS,
        SSS,
    }
}
