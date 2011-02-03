using System;
using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum TradingSessionPhase
    {
        Unknown = 0,
        PreTrading = 1,         // przed otwarciem
        Opening = 2,            // na otwarcie
        Trading = 3,            // sesja (notowania ciągłe)
        Closing = 4,            // na zamknięcie
        PostTrading = 5,        // po zamknięciu
        IntradayAuction = 6,    // równoważenie w trakcie sesji
        MarketClosed = 7        // po całkowitym zamknięciu rynku
    }

    internal static class TrdgSesPhaseUtil
    {
        private static Dictionary<string, TradingSessionPhase> dict =
            new Dictionary<string, TradingSessionPhase> {
                    { "S",  TradingSessionPhase.Trading },
                    { "P",  TradingSessionPhase.PostTrading },  // 16:10,  16:30
                    { "O",  TradingSessionPhase.Unknown },      // 16:20a
                    { "R",  TradingSessionPhase.Closing },      // 16:20b
                    { "N",  TradingSessionPhase.PostTrading },  // 16:35
                    { "F",  TradingSessionPhase.MarketClosed }  // 16:55
            };

        public static TradingSessionPhase Read(XmlElement xml, string name)
        {
            string str = FixmlUtil.ReadString(xml, name, true);
            if (str == null) return TradingSessionPhase.Unknown;
            uint number;
            if (uint.TryParse(str, out number))
                if (Enum.IsDefined(typeof(TradingSessionPhase), (TradingSessionPhase)number))
                    return (TradingSessionPhase)number;
            if (!dict.ContainsKey(str))
                FixmlUtil.Error(xml, name, str, "- unknown TradingSessionPhase");
            return dict[str];
        }
    }
}
