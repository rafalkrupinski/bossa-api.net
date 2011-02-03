using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum MDRejectReason
    {
        UnknownSymbol = '0',            // nieznany walor
        DuplicateRequestId = '1',       // duplikat MDReqID
        UnsupportedRequestType = '4',   // błąd w polu SubReqTyp
        UnsupportedMarketDepth = '5',   // niewspierana liczba ofert
        UnsupportedEntryType = '8'      // niewspierane notowania
    }

    internal static class MDRejResnUtil
    {
        public static MDRejectReason Read(XmlElement xml, string name)
        {
            char ch = FixmlUtil.ReadChar(xml, name);
            if (!Enum.IsDefined(typeof(MDRejectReason), (MDRejectReason)ch))
                FixmlUtil.Error(xml, name, ch, "- unknown MDRejectReason");
            return (MDRejectReason)ch;
        }
    }
}
