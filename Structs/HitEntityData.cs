namespace TBAR.Structs
{
    public struct HitEntityData
    {
        public int Index { get; }

        public bool IsTimed { get; }

        public uint TimeOfHit { get; }

        public HitEntityData(int index, uint hitTime, bool timed = true)
        {
            Index = index;
            TimeOfHit = hitTime;
            IsTimed = timed;
        }
    }
}
