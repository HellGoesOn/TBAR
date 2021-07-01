using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TBAR.Components;
using Terraria.ModLoader;

namespace TBAR.Helpers
{
    public static class SteamHelper
    {
        public static void Initialize()
        {
            SoftBanned = new HashSet<User>();

            try
            {
                string rawID = typeof(ModLoader).GetProperty("SteamID64", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null).ToString();

                if(!string.IsNullOrEmpty(rawID))
                {
                    MySteamID = long.Parse(rawID);
                }
            }
            catch (Exception)
            {

            };

            SoftBanned.Add(new User(76561198419312509, "Complained about a very niche feature way too much"));
        }

        public static bool AmIBanned => SoftBanned.Count(x => x.SteamID == MySteamID) > 0;

        public static long MySteamID { get; private set; }

        public static string BanReason
        {
            get
            {
                if (AmIBanned)
                    return SoftBanned.First(x => x.SteamID == MySteamID).Reason;

                return "";
            }
        }

        public static HashSet<User> SoftBanned { get; private set; }
    }
}
