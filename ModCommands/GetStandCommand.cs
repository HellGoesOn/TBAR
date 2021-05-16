using TBAR.Players;
using TBAR.Stands;
using Terraria.ModLoader;

namespace TBAR.ModCommands
{
    public class GetStandCommand : ModCommand
    {
        public override string Command => "getStand";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            TBARPlayer plr = TBARPlayer.Get(caller.Player);

            string tempInput = input.Remove(0, "/getStand".Length);
            tempInput = tempInput.Trim();
            
            Stand stand = StandLoader.Instance.Get(tempInput);

            if (stand != null)
                plr.PlayerStand = stand;
        }
    }
}
