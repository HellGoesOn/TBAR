using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBAR.TimeSkip
{
    public class TimeSkipData
    {
        public TimeSkipData(Vector2 pos, Vector2 vel, float scale, float rot, Rectangle frame, int dir, float[] ai)
        {
            Position = pos;
            Scale = scale;
            Rotation = rot;
            Frame = frame;
            Direction = dir;
            AI = ai;
            Velocity = vel;
        }

        public Vector2 Position { get; }
        public float Scale { get; }
        public float Rotation { get; }
        public Rectangle Frame { get; }
        public int Direction { get; }
        public float[] AI { get; }
        public Vector2 Velocity { get; }
    }
}
