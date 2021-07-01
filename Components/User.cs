namespace TBAR.Components
{
    public struct User
    {
        public long SteamID { get; }

        public string Reason { get; }

        public User(long steamId, string reason ="")
        {
            SteamID = steamId;
            Reason = reason;
        }
    }
}
