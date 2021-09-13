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

        public int timeLeft;

        private StandState()
        {
            AssignedAnimations = new List<SpriteAnimation>();
        }

        public StandState(params SpriteAnimation[] animations) : this()
        {
            foreach (SpriteAnimation sa in animations)
            {
                AssignedAnimations.Add(sa);
            }
        }

        public StandState(string key, params SpriteAnimation[] animations) : this()
        {
            foreach (SpriteAnimation sa in animations)
            {
                AssignedAnimations.Add(sa);
            }

            Key = key;
        }

        public StandState(string sheetPath, int frameCount, float fps = 5f, bool looping = false) : this()
        {
            AssignedAnimations.Add(new SpriteAnimation(sheetPath, frameCount, fps, looping));
        }

        public StandState(string key, string sheetPath, int frameCount, float fps = 5f, bool looping = false) : this()
        {
            AssignedAnimations.Add(new SpriteAnimation(sheetPath, frameCount, fps, looping));
            Key = key;
        }

        public void BeginState()
        {
            timeLeft = _maxDuration;
            OnStateBegin?.Invoke(sender: this);
        }

        public void Update()
        {
            /*if (!Active && !hasDuration)
                return;*/

            if(HasDuration)
                timeLeft--;

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

        private int _maxDuration;
        public int Duration
        {
            get => timeLeft;
            set
            {
                _maxDuration = value;
            }
        }

        public bool Active => timeLeft > 0 || !HasDuration;

        public bool HasDuration => _maxDuration > 0;
    }
}
