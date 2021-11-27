using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Components
{
    public class Animation2D : ICloneable
    {
        public event AnimationEventHandler AnimationPlay;

        public event AnimationEventHandler OnAnimationEnd;

        private readonly string pathForClone;

        private int _actualFrame;
        private int _currentFrame;

        private float _elapsedTime;

        /// <summary>
        /// Allows easier method of implementing animations
        /// <para>Only supports vertical spritesheets</para>
        /// </summary>
        /// <param name="sheetPath">Path towards spritesheet texture</param>
        /// <param name="frameCount">Amount of frames animation is supposed to have</param>
        /// <param name="fps">Determines animation speed</param>
        /// <param name="looping">Determines autolooping</param>
        /// <param name="modName">Use your mod's name in case you are not using base line assets</param>
        public Animation2D(string sheetPath, int frameCount, float fps = 10f, bool looping = false, string modName = "TBAR")
        {
            if(!Main.dedServ) // if we attempted to load a texture on a server we'd get a crash
                SpriteSheet = ModContent.GetTexture(modName +"/" + sheetPath);

            pathForClone = sheetPath;

            FPS = fps;
            FrameCount = frameCount;
            Looping = looping;
        }

        public void RunPlayEvent()
        {
            AnimationPlay?.Invoke(this);
        }

        /// <summary>
        /// Updates the animation
        /// <para>Make sure to call this outside in some Update</para>
        /// <para>Not recommended to call within UI as it will result in uncapped FPS</para>
        /// </summary>
        public void UpdateAnimation()
        {
            if (!Active)
                return;

            float timePerFrame = 1.0f / FPS;

            _elapsedTime += 0.016f; // magic number

            if (_elapsedTime > timePerFrame)
            {
                if (_currentFrame < FrameCount - 1)
                    _currentFrame++;
                else
                {
                    if (!Looping)
                    {
                        OnAnimationEnd?.Invoke(this);
                        return;
                    }

                    _currentFrame = 0;
                }

                _elapsedTime -= timePerFrame;
            }
            _actualFrame = IsReversed ? FrameCount - _currentFrame - 1 : _currentFrame;
        }

        /// <summary>
        /// Allows basic drawing of the animation
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position">Position of where you want to draw it. Accounts for ScreenPosition by default</param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void DrawBasic(SpriteBatch spriteBatch, Vector2 position, Color color = default, float rotation = 0f, Vector2 scale = default)
        {
            var cl = color == default ? Color.White : color;
            scale = scale == default ? Vector2.One : scale;

            spriteBatch.Draw(SpriteSheet, position - Main.screenPosition, FrameRect, cl, rotation, FrameCenter, scale, SpriteEffects.None, 1f);
        }

        public object Clone()
        {
            return new Animation2D(pathForClone, FrameCount, FPS, Looping);
        }

        internal void Reset()
        {
            _elapsedTime = 0;
            _currentFrame = 0;
            Active = true;
        }

        public bool AsReversed() => IsReversed = false;

        /// <summary>
        /// Determines whether or not animation should be played backwards
        /// </summary>
        public bool IsReversed { get; set; }

        /// <summary>
        /// Detemrines Frames Per Second
        /// </summary>
        public float FPS { get; set; }

        /// <summary>
        /// Determines autolooping
        /// </summary>
        public bool Looping { get; set; }

        /// <summary>
        /// Determines amount of frames animation is supposed to have
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Current frame of the animation
        /// </summary>
        public int CurrentFrame => _currentFrame;

        public bool Active { get; set; } = true;

        public Texture2D SpriteSheet { get; set; }

        /// <summary>
        /// Width & Height of a single frame
        /// </summary>
        public Vector2 FrameSize => new Vector2((int)SpriteSheet.Width, (int)(SpriteSheet.Height / FrameCount));

        /// <summary>
        /// Single frame's rectangle
        /// </summary>
        public Rectangle FrameRect => new Rectangle(0, (int)(FrameSize.Y * _actualFrame), (int)FrameSize.X, (int)FrameSize.Y);

        /// <summary>
        /// Center of a single frame
        /// </summary>
        public Vector2 FrameCenter => new Vector2(FrameSize.X * 0.5f, FrameSize.Y * 0.5f);

        /// <summary>
        /// Refers to single frame width
        /// </summary>
        public int Width => (int)FrameSize.X;

        /// <summary>
        /// Refers to single frame height
        /// </summary>
        public int Height => (int)FrameSize.Y;
    }
}
