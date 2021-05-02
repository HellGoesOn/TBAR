using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.ScreenModifiers
{
    public class ShakeScreenModifier : ScreenModifier
    {
        public ShakeScreenModifier(Vector2 position, int intensity, float amount) : base(position)
        {
            BasePosition = position;
            Amount = amount;
            Intensity = intensity;
        }

        public ShakeScreenModifier(Vector2 position, int lifeTime, int intensity, float amount) : base(position, lifeTime)
        {
            BasePosition = position;
            Amount = amount;
            Intensity = intensity;
            Range = new Vector2(60, 60);
        }

        public ShakeScreenModifier(Vector2 position, Vector2 range, int lifeTime, int intensity, float amount) : this(position, lifeTime, intensity, amount)
        {
            Range = range;
        }

        public override void UpdateModifier(Player player)
        {
            base.UpdateModifier(player);

            Position = Vector2.Lerp(Position, BasePosition, Amount);

            if (NeedsToUpdate)
            {
                float x = Main.rand.NextFloat(-Range.X, Range.X);
                float y = Main.rand.NextFloat(-Range.Y, Range.Y);

                Position = BasePosition + new Vector2(x, y);
                Timer = 0;
            }
            else
                Timer++;
        }

        /// <summary>
        /// Determines how often screen shakes
        /// </summary>
        public int Intensity { get; }

        public Vector2 Range { get; set; }

        public int Timer { get; set; }

        public float Amount { get; set; }

        public bool NeedsToUpdate => Timer >= Intensity;

        /// <summary>
        /// Determines position
        /// </summary>
        public Vector2 BasePosition { get; set; }
    }
}
