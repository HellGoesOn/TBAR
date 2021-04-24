namespace TBAR.Structs
{
    public struct HitNPCData
    {
        public int Index { get; }

        public uint TimeOfHit { get; }

        public HitNPCData(int index, uint hitTime)
        {
            Index = index;
            TimeOfHit = hitTime;
        }
    }
}
