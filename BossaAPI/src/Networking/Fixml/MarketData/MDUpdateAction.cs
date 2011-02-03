using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum MDUpdateAction
    {
        New = '0',
        Change = '1', 
        Delete = '2'
    }

    internal static class MDUpdateActionUtil
    {
        public static MDUpdateAction Read(XmlElement xml, string name)
        {
            char ch = FixmlUtil.ReadChar(xml, name);
            if (!Enum.IsDefined(typeof(MDUpdateAction), (MDUpdateAction)ch))
                FixmlUtil.Error(xml, name, ch, "- unknown MDUpdateAction");
            return (MDUpdateAction)ch;
        }
    }
}
