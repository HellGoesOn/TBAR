using Microsoft.Xna.Framework;
using TBAR.Players;
using Terraria;

namespace TBAR.ScreenModifiers
{
    public class ScreenModifier
    {
        public static void AddModifiersToPlayer(Player player, params ScreenModifier[] modifiers)
        {
            foreach(ScreenModifier mod in modifiers)
            {
                TBARPlayer.Get(player).ScreenModifiers.Add(mod);
            }
        }
         
        public ScreenModifier(Vector2 position)
        {
            Position = position;
            LifeTime = 300;
        }

        public ScreenModifier(Vector2 position, int lifeTime) : this(position)
        {
            LifeTime = lifeTime;
        }

        public void UpdateScreenPosition(ref Vector2 screenPosition)
        {
            if (LifeTime <= 0)
                return;

            screenPosition = Position - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
        }

        public virtual void UpdateModifier(Player player) => LifeTime--;

        public Vector2 Position { get; set; }

        /// <summary>
        /// Determines how long this should exist
        /// </summary>
        public int LifeTime { get; set; }

        public bool LifeTimeEnded => LifeTime <= 0;

        public bool Active { get; set; } = true;
    }
}
