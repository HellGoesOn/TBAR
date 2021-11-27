namespace TBAR.Stands
{
    public class StandState
    {
        public event StandStateEventHandler OnStateBegin;

        public event StandStateEventHandler OnStateUpdate;

        public event StandStateEventHandler OnStateEnd;

        public int TimeLeft { get; set; }

        internal StandState(int duration = 0)
        {
            MaxDuration = duration;
            TimeLeft = MaxDuration;
        }

        public void BeginState()
        {
            TimeLeft = MaxDuration;
            OnStateBegin?.Invoke(sender: this);
        }

        public void Update()
        {
            if(HasDuration)
                TimeLeft--;

            OnStateUpdate?.Invoke(sender: this);
        }

        public void EndState()
        {
            OnStateEnd?.Invoke(sender: this);
        }

        public int MaxDuration { get; set; }
        public bool Active => TimeLeft > 0 || !HasDuration;

        public bool HasDuration => MaxDuration > 0;
    }
}
