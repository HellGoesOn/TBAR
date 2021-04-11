using Microsoft.Xna.Framework;
using Terraria;

namespace TBAR.TimeSkip
{
    public class FakeTileData
    {
        public FakeTileData(int tileid, Vector2 position, Rectangle frame)
        {
            TileID = tileid;

            Position = position;

            TileFrame = frame;
        }

        public FakeTileData(int tileid, Vector2 position, Rectangle frame, Color color)
        {
            TileID = tileid;

            Position = position;

            TileFrame = frame;

            Color = color;
        }

        public void SetData()
        {
            int mult = Main.rand.NextBool() ? -1 : 1;
            Velocity = new Vector2(Main.rand.Next(36) * 0.1f * mult, -Main.rand.Next(46) * 0.1f);
            RotationDirection = Main.rand.NextBool() ? -1 : 1;
            RotationSpeed = (float)(Main.rand.Next(12, 64)) * 0.001f;
        }

        public void Update()
        {
            Position += Velocity;
            Rotation += RotationSpeed * RotationDirection;
            Opacity -= 0.0075f;
            Color = Color.Lerp(Color, Color.White, 0.04f);
        }

        public float Rotation { get; set; }

        public int RotationDirection { get; set; } = 1;

        public float RotationSpeed { get; set; } = 0.012f;

        public float Opacity { get; set; } = 1f;

        public int TileID { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 VFXOffset { get; set; }

        public Vector2 Velocity { get; set; }

        public Color Color { get; set; }

        public Rectangle TileFrame { get; set; }
    }
}
