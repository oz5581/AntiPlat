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

        public override string Description => "Prevents 999 Plat Stacks";

        public override string Name => "AntiPlat";

        public override Version Version => new Version(1, 0, 0, 0);

        public AntiPlat(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            }
            base.Dispose(disposing);
        }

        private IEnumerable<TSPlayer> GetLoggedInPlayers()
        {
            return TShock.Players.Where(p => p != null && p.IsLoggedIn);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            foreach (TSPlayer plr in GetLoggedInPlayers())
            {
                for (int i = 0; i < 58; i++)
                {
                    if (plr.TPlayer.inventory[i].type == ItemID.PlatinumCoin &&
                        plr.TPlayer.inventory[i].stack == 999)
                    {
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, plr.Index, (float)i);
                    }
                }
            }
        }
    }
}