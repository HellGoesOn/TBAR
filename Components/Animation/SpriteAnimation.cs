using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Components
{
    public class SpriteAnimation
    {
        public event AnimationEventHandler AnimationPlay;

        public event AnimationEventHandler OnAnimationEnd;

        public SpriteAnimation(string sheetPath, int frameCount, float fps = 5f, bool looping = false, int loopTime = -1)
        {
            if(!Main.dedServ) // if we attempted to load a texture on a server we'd get a crash
                SpriteSheet = ModContent.GetTexture("TBAR/" + sheetPath);

            FramesPerSecond = fps;
            FrameCount = frameCount;
            Looping = looping;
            LoopTime = loopTime;
        }

        public void UpdateEvent()
        {
            AnimationPlay?.Invoke(this);
        }

        public void Reset()
        {
            ElapsedTime = 0;
            CurrentFrame = 0;
            Active = true;
        }

        public void Update()
        {
            if (!Active)
                return;

            ElapsedTime += 0.016f; // magic number

            if (LoopTime != -1)
            {
                LoopTimer += 1;

                if (LoopTimer >= LoopTime - 1)
                    Looping = false;
            }

            if(ElapsedTime > TimePerFrame)
            {
                CurrentFrame++;

                if(CurrentFrame >= FrameCount)
                {
                    CurrentFrame = 0;

                    if (!Looping)
                    {
                        OnAnimationEnd?.Invoke(this);

                        if (LoopTime != -1)
                        {
                            LoopTimer = 0;
                            Looping = true;
                        }

                        Active = false;
                    }
                }

                ElapsedTime -= TimePerFrame;
            }
        }

        public int LoopTimer { get; set; }

        public int LoopTime { get; }

        public float ElapsedTime { get; set; }

        public float FramesPerSecond { get; set; }

        public bool Looping { get; set; }

        public int FrameCount { get; set; }

        public int CurrentFrame { get; set; }

        public bool Active { get; set; } = true;

        public Texture2D SpriteSheet { get; set; }

        public float TimePerFrame => 1.0f / FramesPerSecond;

        public Vector2 FrameSize => new Vector2((int)SpriteSheet.Width, (int)(SpriteSheet.Height / FrameCount));

        public Rectangle FrameRect => new Rectangle(0, (int)(FrameSize.Y * CurrentFrame), (int)FrameSize.X, (int)FrameSize.Y);

        public Vector2 DrawOrigin => new Vector2((int)(FrameSize.X * 0.5f), (int)(FrameSize.Y * 0.5f));
    }
}
