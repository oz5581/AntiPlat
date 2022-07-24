using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Terraria.Localization;
using Terraria.ID;

namespace AntiPlatPlugin
{
    [ApiVersion(2, 1)]
    public class AntiPlat : TerrariaPlugin
    {
        public override string Author => "Ozz5581";

        public override string Description => "Prevents Duped Plat Stacks";

        public override string Name => "AntiPlat";

        public override Version Version => new Version(1, 0, 0, 0);

        private ulong UpdateCount = 0;

        public AntiPlat(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private IEnumerable<TSPlayer> GetLoggedInPlayers()
        {
            return TShock.Players.Where(p => p != null && p.IsLoggedIn);
        }

        private void CheckSlot(TSPlayer player, int slot)
        {
            Item itemToCheck = (
                (slot >= 220) ? player.TPlayer.bank4.item[slot - 220] :
                ((slot >= 180) ? player.TPlayer.bank3.item[slot - 180] : 
                ((slot >= 179) ? player.TPlayer.trashItem : 
                ((slot >= 139) ? player.TPlayer.bank2.item[slot - 139] : 
                ((slot >= 99) ? player.TPlayer.bank.item[slot - 99] : 
                ((slot >= 94f) ? player.TPlayer.miscDyes[slot - 94] : 
                ((slot >= 89) ? player.TPlayer.miscEquips[slot - 89] : 
                ((slot >= 79) ? player.TPlayer.dye[slot - 79] : 
                ((!(slot >= 59)) ? player.TPlayer.inventory[slot] : player.TPlayer.armor[slot - 59])))))))));

            if (itemToCheck.type == ItemID.PlatinumCoin &&
                itemToCheck.stack >= itemToCheck.maxStack - 5)
            {
                itemToCheck.SetDefaults(0);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            UpdateCount++;

            // Check for 999 platinum coins every 15 frames
            if (UpdateCount % 10 == 0)
            {
                foreach (TSPlayer plr in GetLoggedInPlayers())
                {
                    for (int i = 0; i < 260; i++)
                    {
                        CheckSlot(plr, i);
                    }
                }
            }
        }
    }
}
