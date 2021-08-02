using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TBAR.Enums;
using TBAR.Helpers;
using TBAR.Players;
using TBAR.UI.Elements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.Input
{
    public class StandCombo
    {
        public event ComboEventHandler OnActivate;

        public StandCombo(string Name, params ComboInput[] inputs)
        {
            ComboName = Name;
            
            foreach(ComboInput i in inputs)
                RequiredInputs.Add(i);
        }

        public void ForceActivate(Player player)
        {
            OnActivate?.Invoke(player);
        }

        public bool TryActivate(Player player, List<ComboInput> inputs)
        {
            // if received input count is lower, we won't be able to activate the combo
            if (inputs.Count < RequiredInputs.Count || !TBARPlayer.Get(player).CheckStaminaCost(Cost))
                return false;

            if (SteamHelper.AmIBanned)
            {
                Main.NewText("Your massive amounts of cringe caused " + TBARPlayer.Get(player).PlayerStand.StandName + " to refuse moving even a muscle for you");
                Main.NewText("It states that you: " + SteamHelper.BanReason);
                return false;
            }

            // find diffrence so we only check the last "answer"
            // e.g: we press A1-A1-Up-Down; combo is just Up-Down
            // it will skip over A1-A1 and only check the last 2 inputs
            int dif = Math.Abs(RequiredInputs.Count - inputs.Count);

            for (int i = 0 + dif; i < inputs.Count; i++)
            {
                if (inputs[i] != RequiredInputs[i-dif])
                    return false;
            }

            DrawHelper.CircleDust(player.Center, Vector2.Zero, 6, 8, 8, 1.85f);

            OnActivate?.Invoke(player);

            SendPacket(player, ComboName, player.whoAmI);

            if (!Main.dedServ)
                ComboTimeRunner.AchievedCombo = true;

            return true;
        }

        public static void SendPacket(Player player, string name, int fromWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = TBAR.Instance.GetPacket();
            packet.Write((byte)PacketType.UsedCombo);
            packet.Write((byte)player.whoAmI);
            packet.Write(name);
            packet.Send(-1, fromWho);
        }

        public int Style { get; set; }

        public string Description { get; set; }

        public string ComboName { get; }

        public double Cost { get; set; }

        public List<ComboInput> RequiredInputs { get; } = new List<ComboInput>();
    }
}
