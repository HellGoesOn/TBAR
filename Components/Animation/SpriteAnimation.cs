using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Components
{
    public class SpriteAnimation : ICloneable
    {
        public event AnimationEventHandler AnimationPlay;

        public event AnimationEventHandler OnAnimationEnd;

        private readonly string pathForClone;

        public SpriteAnimation(string sheetPath, int frameCount, float fps = 5f, bool looping = false, string modName = "TBAR")
        {
            if(!Main.dedServ) // if we attempted to load a texture on a server we'd get a crash
                SpriteSheet = ModContent.GetTexture(modName +"/" + sheetPath);

            pathForClone = sheetPath;

            FramesPerSecond = fps;
            FrameCount = frameCount;
            Looping = looping;
        }

        public void UpdateEvent()
        {
            AnimationPlay?.Invoke(this);
        }

        public void Reset()
        {
            ElapsedTime = 0;
            CurrentFrame = IsReversed ? FrameCount - 1 : 0;
            Active = true;
        }

        public void Update()
        {
            if (!Active)
                return;

            ElapsedTime += 0.016f; // magic number


            if (ElapsedTime > TimePerFrame)
            {
                if (!IsReversed)
                    DefaultBehavior();
                else
                    ReversedBehavior();

                ElapsedTime -= TimePerFrame;
            }
        }

        public void DefaultBehavior()
        {
            if(CurrentFrame < FrameCount - 1)
                CurrentFrame++;
            else
            {
                if (!Looping)
                {
                    OnAnimationEnd?.Invoke(this);
                    return;
                }

                CurrentFrame = 0;
            }
        }

        public void ReversedBehavior()
        {
            if(CurrentFrame > 1)
                CurrentFrame--;
            else
            {
                if (!Looping)
                {
                    OnAnimationEnd?.Invoke(this);
                    return;
                }

                CurrentFrame = FrameCount;
            }
        }

        public object Clone()
        {
            return new SpriteAnimation(pathForClone, FrameCount, FramesPerSecond, Looping);
        }

        private bool isReversed;
        public bool IsReversed 
        {
            get => isReversed;
            set
            {
                isReversed = value;

                Reset();
            }
        }

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

        public Vector2 DrawOrigin => new Vector2(FrameSize.X * 0.5f, FrameSize.Y * 0.5f);
    }
}
