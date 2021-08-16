using System;
using TBAR.Players;
using TBAR.Stands;
using Terraria.ModLoader;

namespace TBAR.ModCommands
{
    public class SetStyleRankCommand : ModCommand
    {
        public override string Command => "setrank";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            TBARPlayer plr = TBARPlayer.Get(caller.Player);

            int.TryParse(args[0], out int temp);

            plr.CurrentStyleRank = (StyleRank)temp;
        }
    }
}
