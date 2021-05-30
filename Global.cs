namespace TBAR
{
    public static class Global
    {
        public const int TPS = 60;

        public const int TILE_SIZE = 16;

        /// <returns>Value in seconds</returns>
        public static int SecondsToTicks(int value) => value * TPS;

        public static int TicksToSeconds(int value) => value / TPS;
    }
}
