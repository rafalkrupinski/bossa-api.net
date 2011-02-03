using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum OrderType
    {
        PKC = '1',          // zlecenie "po każdej cenie"
        Limit = 'L',        // zlecenie "z limitem ceny"
        StopLoss = '3',     // "PKC" aktywowany dopiero po osiągnięciu wskazanej ceny
        StopLimit = '4',    // "Limit" aktywowany dopiero po osiągnięciu wskazanej ceny
        PCR_PCRO = 'K'      // PCR (po cenie rynkowej) lub PCRO (na otwarcie/zamknięcie, patrz TimeInForce)
    }

    internal static class OrderTypeUtil
    {
        public static OrderType? Read(XmlElement xml, string name, bool optional)
        {
            char? ch = FixmlUtil.ReadChar(xml, name, optional);
            if (ch == null) return null;
            if (!Enum.IsDefined(typeof(OrderType), (OrderType)ch))
                FixmlUtil.Error(xml, name, ch, "- unknown OrderType");
            return (OrderType)ch;
        }

        public static string Write(OrderType value)
        {
            return ((char)value).ToString();
        }
    }
}
