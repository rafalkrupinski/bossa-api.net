using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    // typ prowizji
    public enum OrdCommisionType
    {
        PerUnit = '1',  // na jednostkę
        Percent = '2',  // procent
        Absolute = '3'  // wartość absolutna
    }

    internal static class OrdCommTypeUtil
    {
        public static OrdCommisionType? Read(XmlElement xml, string name, bool optional)
        {
            char? ch = FixmlUtil.ReadChar(xml, name, optional);
            if (ch == null) return null;
            if (!Enum.IsDefined(typeof(OrdCommisionType), (OrdCommisionType)ch))
                FixmlUtil.Error(xml, name, ch, "- unknown OrdCommisionType");
            return (OrdCommisionType)ch;
        }
    }
}
