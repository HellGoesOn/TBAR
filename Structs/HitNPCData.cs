namespace TBAR.Structs
{
    public struct HitNPCData
    {
        public int Index { get; }

        public bool IsTimed { get; }

        public uint TimeOfHit { get; }

        public HitNPCData(int index, uint hitTime, bool timed = true)
        {
            Index = index;
            TimeOfHit = hitTime;
            IsTimed = timed;
        }
    }
}
