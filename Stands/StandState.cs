using System.Collections.Generic;
using TBAR.Components;
using Terraria;

namespace TBAR.Stands
{
    public class StandState
    {
        public event StandStateEventHandler OnStateBegin;

        public event StandStateEventHandler OnStateUpdate;

        public event StandStateEventHandler OnStateEnd;

        private StandState()
        {
            AssignedAnimations = new List<SpriteAnimation>();
        }

        public StandState(params SpriteAnimation[] animations) : this()
        {
            foreach(SpriteAnimation sa in animations)
            {
                AssignedAnimations.Add(sa);
            }
        }
        
        public StandState(string key, params SpriteAnimation[] animations) : this()
        {
            foreach(SpriteAnimation sa in animations)
            {
                AssignedAnimations.Add(sa);
            }

            Key = key;
        }

        public StandState(string sheetPath, int frameCount, float fps = 5f, bool looping = false, int loopTime = -1) : this()
        {
            AssignedAnimations.Add(new SpriteAnimation(sheetPath, frameCount, fps, looping, loopTime));
        }

        public StandState(string key, string sheetPath, int frameCount, float fps = 5f, bool looping = false, int loopTime = -1) : this()
        {
            AssignedAnimations.Add(new SpriteAnimation(sheetPath, frameCount, fps, looping, loopTime));
            Key = key;
        }

        public void BeginState()
        {
            OnStateBegin?.Invoke(sender: this);
        }

        public void Update()
        {
            OnStateUpdate?.Invoke(sender: this);
            CurrentAnimation.Update();
            CurrentAnimation.UpdateEvent();
        }

        public void EndState()
        {
            OnStateEnd?.Invoke(sender: this);

            foreach (SpriteAnimation a in AssignedAnimations)
                a.Reset();
        }

        public int CurrentAnimationID { get; set; }

        public SpriteAnimation CurrentAnimation => AssignedAnimations[CurrentAnimationID];

        public List<SpriteAnimation> AssignedAnimations { get; }

        public string Key { get; set; }
    }
}
