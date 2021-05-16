using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.ModCommands
{
    public class ListStandsCommand : ModCommand
    {
        public override string Command => "list";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            foreach (Stand st in StandLoader.Instance.Stands)
                Main.NewText(st.StandName);
        }
    }
}
