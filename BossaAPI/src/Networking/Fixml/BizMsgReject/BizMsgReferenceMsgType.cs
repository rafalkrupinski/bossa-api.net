using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum BizMsgReferenceMsgType
    {
        UserRequest,                    // logowanie/wylogowanie
        NewOrderSingle,                 // nowe zlecenie
        OrderCancelRequest,             // anulata zlecenia
        OrderCancelReplaceRequest,      // modyfikacja zlecenia
        OrderStatusRequest,             // status zlecenia
        MarketDataRequest,              // notowania online
        TradingSessionStatusRequest     // status oraz faza sesji
    }

    internal static class BizRefMsgTypeUtil
    {
        private static Dictionary<string, BizMsgReferenceMsgType> dict =
            new Dictionary<string, BizMsgReferenceMsgType> {
                    { "BE", BizMsgReferenceMsgType.UserRequest },
                    { "D",  BizMsgReferenceMsgType.NewOrderSingle },
                    { "F",  BizMsgReferenceMsgType.OrderCancelRequest },
                    { "G",  BizMsgReferenceMsgType.OrderCancelReplaceRequest },
                    { "H",  BizMsgReferenceMsgType.OrderStatusRequest },
                    { "V",  BizMsgReferenceMsgType.MarketDataRequest },
                    { "g",  BizMsgReferenceMsgType.TradingSessionStatusRequest }
            };

        public static BizMsgReferenceMsgType Read(XmlElement xml, string name)
        {
            string str = FixmlUtil.ReadString(xml, name);
            if (!dict.ContainsKey(str)) 
                FixmlUtil.Error(xml, name, str, "- unknown BizReferenceMsgType");
            return dict[str];
        }
    }
}
